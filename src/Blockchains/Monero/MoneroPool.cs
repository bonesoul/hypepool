using System;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Coins;
using Hypepool.Common.Daemon;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Native;
using Hypepool.Common.Pools;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Time;
using Hypepool.Monero.Daemon.Responses;
using Hypepool.Monero.Stratum;
using Hypepool.Monero.Stratum.Responses;
using Newtonsoft.Json;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {

        private string _poolAddress = "9z9PQi2NFS43RnNDUQo5oucHUpvJDi5RUaDuLoHtG5dJ1v2AMjKawziKfWdRY5mVuANs2dr2k6hsSDZCQJNL38LqD6xQCHX";
        private readonly uint _poolAddressBase58Prefix;

        public MoneroPool(IPoolContext poolContext, IServerFactory serverFactory)
            : base(poolContext, serverFactory)
        {
            _logger = Log.ForContext<MoneroPool>();

            _poolAddressBase58Prefix = LibCryptonote.DecodeAddress(_poolAddress);

            if (_poolAddressBase58Prefix == 0)
                _logger.Error($"Unable to decode pool-address {_poolAddress}");
        }

        public override void Initialize()
        {
            var jobManager = new MoneroJobManager();
            var daemonClient = new DaemonClient();
            var stratumServer = ServerFactory.GetStratumServer();

            PoolContext.Attach(daemonClient, jobManager, stratumServer);
        }

        protected override async Task<bool> IsDaemonConnectionHealthy()
        {
            var response =
              await PoolContext.DaemonClient.ExecuteCommandAsync<GetInfoResponse>(MoneroRpcCommands.GetInfo);

            if (response.Error?.InnerException?.GetType() == typeof(DaemonException))
            {
                var exception = (DaemonException) response.Error.InnerException;
                if (exception.Code == HttpStatusCode.Unauthorized)
                    _logger.Fatal("Wallet daemon reported invalid credentials");
            }

            return response.Error == null;
        }

        protected override Task<bool> IsDaemonConnectedToNetwork()
        {
            throw new NotImplementedException();
        }

        protected override Task EnsureDaemonSynchedAsync()
        {
            throw new NotImplementedException();
        }

        protected override WorkerContext CreateClientContext()
        {
            return new MoneroWorkerContext();
        }

        public override async Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest)
        {
            var request = timeStampedRequest.Value;
            var context = client.GetContextAs<MoneroWorkerContext>();

            switch (request.Method)
            {
                case MoneroStratumMethods.Login:
                    OnLogin(client, timeStampedRequest);
                    break;
                case MoneroStratumMethods.GetJob:
                    OnGetJob(client, timeStampedRequest);
                    break;
                case MoneroStratumMethods.Submit:
                    await OnSubmitAsync(client, timeStampedRequest);
                    break;
                case MoneroStratumMethods.KeepAlive:
                    context.LastActivity = new StandardClock().Now; // recognize activity.
                    break;
                default:
                    _logger.Debug($"[{client.ConnectionId}] Unsupported RPC request: {JsonConvert.SerializeObject(request, Globals.JsonSerializerSettings)}");
                    client.RespondError(StratumError.Other, $"Unsupported request {request.Method}", request.Id);
                    break;
            }
        }

        private void OnLogin(IStratumClient client, Timestamped<JsonRpcRequest> tsRequest)
        {
            var request = tsRequest.Value;
            var context = client.GetContextAs<MoneroWorkerContext>();

            if (request.Id == null)
            {
                client.RespondError(StratumError.MinusOne, "missing request id", request.Id);
                return;
            }

            var loginRequest = request.ParamsAs<MoneroLoginRequest>();

            if (string.IsNullOrEmpty(loginRequest?.Login))
            {
                client.RespondError(StratumError.MinusOne, "missing login", request.Id);
                return;
            }

            // extract worker & miner.
            var split = loginRequest.Login.Split('.');
            context.MinerName = split[0];
            context.WorkerName = split.Length > 1 ? split[1] : null;

            // set useragent.
            context.UserAgent = loginRequest.UserAgent;

            // set payment-id if any.
            var index = context.MinerName.IndexOf('#');
            if (index != -1)
            {
                context.MinerName = context.MinerName.Substring(0, index);
                context.PaymentId = context.MinerName.Substring(index + 1);
            }

            var hasValidAddress = ValidateAddress(context.MinerName);
            context.IsAuthorized = context.IsSubscribed = hasValidAddress;

            if (!context.IsAuthorized)
            {
                client.RespondError(StratumError.MinusOne, "invalid login", request.Id);
                return;
            }

            var loginResponse = new MoneroLoginResponse
            {
                Id = client.ConnectionId,
                Job = new MoneroJobParams()
                {
                    Blob = "a",
                    JobId = "b",
                    Target = "c"
                }
            };

            client.Respond(loginResponse, request.Id);

            // log association
            _logger.Information($"[{client.ConnectionId}] = {loginRequest.Login} = {client.RemoteEndpoint.Address}");

            // recognize activity.
            context.LastActivity = new StandardClock().Now;
        }

        private void OnGetJob(IStratumClient client, Timestamped<JsonRpcRequest> tsRequest)
        {

        }

        private async Task OnSubmitAsync(IStratumClient client, Timestamped<JsonRpcRequest> tsRequest)
        {
        }

        private bool ValidateAddress(string address)
        {
            // check address length.
            if (address.Length != MoneroConstants.AddressLength[CoinType.XMR])
                return false;

            var addressPrefix = LibCryptonote.DecodeAddress(address);
            if (addressPrefix != _poolAddressBase58Prefix)
                return false;

            return true;
        } 
    }
}