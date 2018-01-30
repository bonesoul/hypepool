#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion

using System;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Coins;
using Hypepool.Common.Daemon;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Native;
using Hypepool.Common.Pools;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Helpers.Time;
using Hypepool.Monero.Daemon.Requests;
using Hypepool.Monero.Daemon.Responses;
using Hypepool.Monero.Stratum;
using Hypepool.Monero.Stratum.Requests;
using Hypepool.Monero.Stratum.Responses;
using Newtonsoft.Json;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {
        private ulong _poolAddressBase58Prefix;

        public MoneroPool(IServerFactory serverFactory)
            : base(serverFactory)
        {
            _logger = Log.ForContext<MoneroPool>();
        }

        public override async Task Initialize()
        {
            _logger.Information($"initializing pool..");

            try
            {
                PoolContext = new MoneroPoolContext();

                var miningDaemon = new DaemonClient("127.0.0.1", 28081, "user", "pass", MoneroConstants.DaemonRpcLocation);
                var wallDaemon = new DaemonClient("127.0.0.1", 28085, "user", "pass", MoneroConstants.DaemonRpcLocation);
                var jobManager = new MoneroJobManager();
                var stratumServer = ServerFactory.GetStratumServer();

                ((MoneroPoolContext)PoolContext).Configure(miningDaemon, wallDaemon, jobManager, stratumServer); // configure the pool context.
                PoolContext.JobManager.Configure(PoolContext);

                await RunPreInitChecksAsync(); // any pre-init checks.

                PoolContext.Daemon.Initialize(); // initialize mining daemon.
                ((MoneroPoolContext)PoolContext).WalletDaemon.Initialize(); // initialize wallet daemon.
                await WaitDaemonConnection(); // wait for coin daemon connection.
                await EnsureDaemonSynchedAsync(); // ensure the coin daemon is synced to network.

                await RunPostInitChecksAsync(); // run any post init checks required by the blockchain.
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
            }
        }

        public override async Task Start()
        {
            try
            {
                var test = PoolContext.JobManager.Start();
                PoolContext.StratumServer.Start(this);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
            }
        }

        protected override async Task RunPreInitChecksAsync()
        {
            // decode configured pool address.
            _poolAddressBase58Prefix = LibCryptonote.DecodeAddress(PoolContext.PoolAddress);
            if (_poolAddressBase58Prefix == 0)
                throw new PoolStartupAbortedException("unable to decode configured pool address!");
        }

        protected override async Task RunPostInitChecksAsync()
        {
            var infoResponse = await PoolContext.Daemon.ExecuteCommandAsync(MoneroRpcCommands.GetInfo);
            var addressResponse = await ((MoneroPoolContext)PoolContext).WalletDaemon.ExecuteCommandAsync<GetAddressResponse>(MoneroWalletCommands.GetAddress);

            // ensure pool owns wallet
            if (addressResponse.Response?.Address != PoolContext.PoolAddress)
                throw new PoolStartupAbortedException("pool wallet does not own the configured pool address!");
        }

        public async Task WaitDaemonConnection()
        {
            while (!await IsDaemonConnectionHealthyAsync())
            {
                _logger.Information("waiting for wallet daemon connectivity..");
                await Task.Delay(TimeSpan.FromSeconds(5000));
            }

            _logger.Information("established coin daemon connection..");

            while (!await IsDaemonConnectedToNetworkAsync())
            {
                _logger.Information("waiting for coin daemon to connect peers..");
                await Task.Delay(TimeSpan.FromSeconds(5000));
            }

            _logger.Information("coin daemon do have peer connections..");
        }

        protected override async Task<bool> IsDaemonConnectionHealthyAsync()
        {
            var response = await PoolContext.Daemon.ExecuteCommandAsync<GetInfoResponse>(MoneroRpcCommands.GetInfo); // getinfo.

            // check if we are free of any errors.
            if (response.Error == null) // if so we,
                return true; // we have a healthy connection.

            if (response.Error.InnerException?.GetType() != typeof(DaemonException)) // if it's a generic exception
                _logger.Warning($"daemon connection problem: {response.Error.InnerException?.Message}");
            else // else if we have a daemon exception.
            {
                var exception = (DaemonException)response.Error.InnerException;

                _logger.Warning(exception.Code == HttpStatusCode.Unauthorized // check for credentials errors.
                    ? "daemon connection problem: invalid rpc credentials."
                    : $"daemon connection problem: {exception.Code}");
            }

            return false;
        }

        protected override async Task<bool> IsDaemonConnectedToNetworkAsync()
        {
            var response = await PoolContext.Daemon.ExecuteCommandAsync<GetInfoResponse>(MoneroRpcCommands.GetInfo); // getinfo.

            return response.Error == null && response.Response != null && // check if coin daemon have any incoming + outgoing connections.
                   (response.Response.OutgoingConnectionsCount + response.Response.IncomingConnectionsCount) > 0;
        }

        protected override async Task EnsureDaemonSynchedAsync()
        {
            var request = new GetBlockTemplateRequest
            {
                WalletAddress = PoolContext.PoolAddress,
                ReserveSize = MoneroConstants.ReserveSize
            };

            while (true) // loop until sync is complete.
            {
                var blockTemplateResponse = await PoolContext.Daemon.ExecuteCommandAsync<GetBlockTemplateResponse>(MoneroRpcCommands.GetBlockTemplate, request);

                var isSynched = blockTemplateResponse.Error == null || blockTemplateResponse.Error.Code != -9; // is daemon synced to network?

                if (isSynched) // break out of the loop once synched.
                    break;

                var infoResponse = await PoolContext.Daemon.ExecuteCommandAsync<GetInfoResponse>(MoneroRpcCommands.GetInfo); // getinfo.
                var currentHeight = infoResponse.Response.Height;
                var totalBlocks = infoResponse.Response.TargetHeight;
                var percent = (double) currentHeight / totalBlocks * 100;

                _logger.Information($"waiting for blockchain sync [{percent:0.00}%]..");

                await Task.Delay(5000); // stay awhile and listen!
            }        
            
            _logger.Information("blockchain is synched to network..");
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