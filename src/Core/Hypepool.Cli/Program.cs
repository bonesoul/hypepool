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

namespace Hypepool.Cli
{
    public class Program
    {


        public static void Main(string[] args)
        {
            ConsoleExtensions.PrintBanner(); // print banner.
            ConsoleExtensions.PrintLicense(); // print license.

            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run(); // run bootstrapper.
            bootstrapper.Container.Verify(); // verify container

            var coreFactory = bootstrapper.Container.GetInstance<ICoreFactory>(); // get core object factory.
            var engine = coreFactory.GetEngine(); // get engine.

            var logger = Log.ForContext<Program>();
            logger.Information($"hypepool warming-up: v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            logger.Information($"running on {RuntimeInfo.OperatingSystem.Name}-{RuntimeInformation.ProcessArchitecture.ToString().ToLower()}");
            logger.Information($"os: {RuntimeInformation.OSDescription}");
            logger.Information($"dotnet core: {RuntimeInfo.DotNetCoreVersion}, framework: {RuntimeInformation.FrameworkDescription}");
            logger.Information($"running over {Environment.ProcessorCount} core system");

            engine.Start();

            while (true)
            {
            }
        }
    }
}
