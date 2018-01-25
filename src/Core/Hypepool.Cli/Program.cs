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
using System.Reflection;
using System.Runtime.InteropServices;
using Hypepool.Cli.Utils.Extensions;
using Hypepool.Cli.Utils.Runtime;
using Hypepool.Core.Internals.Bootstrap;
using Hypepool.Core.Internals.Factories.Core;
using Serilog;
using Stashbox;

namespace Hypepool.Cli
{
    public class Program
    {
        private static ILogger _logger;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Catch any unhandled exceptions if we are in release mode.

            ConsoleExtensions.PrintBanner(); // print banner.
            ConsoleExtensions.PrintLicense(); // print license.

            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run(); // run bootstrapper.

            var coreFactory = bootstrapper.Container.Resolve<ICoreFactory>(); // get core object factory.
            var engine = coreFactory.GetEngine(); // get engine.

            // create logger to be used later.
            _logger = Log.ForContext<Program>();

            _logger.Information($"hypepool warming-up: v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            _logger.Information($"running on {RuntimeInfo.OperatingSystem.Name}-{RuntimeInformation.ProcessArchitecture.ToString().ToLower()}");
            _logger.Information($"os: {RuntimeInformation.OSDescription}");
            _logger.Information($"dotnet core: {RuntimeInfo.DotNetCoreVersion}, framework: {RuntimeInformation.FrameworkDescription}");
            _logger.Information($"running over {Environment.ProcessorCount} core system");

            //engine.Start();

            while (true)
            {
            }
        }

        /// <summary>
        /// Unhandled exception emitter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception exception)) // if we can't get the exception, whine about it.
            {
                _logger.Error("can't get exception object from UnhandledExceptionEventArgs");
                throw new ArgumentNullException(nameof(e));
            }

            if(!e.IsTerminating)
                _logger.Fatal(exception, $"terminating because of unhandled exception: {exception.Message}");
            else
            {
                _logger.Error(exception, $"caught unhandled exception: {exception.Message}");
#if !DEBUG                 
                Environment.Exit(-1); // prevent console window from being closed when we are in development mode.
#endif
            }
        }
    }
}
