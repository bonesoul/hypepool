using System;
using System.Reflection;
using Hypepool.Core.Internals.Bootstrap;
using Hypepool.Core.Internals.Factories.Core;

namespace Hypepool.Cli
{
    public class Program
    {
        static void Main(string[] args)
        {
            PrintBanner();

            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run(); // run bootstrapper.
            bootstrapper.Container.Verify(); // verify container

            var coreFactory = bootstrapper.Container.GetInstance<ICoreFactory>(); // get core object factory.
            var engine = coreFactory.GetEngine(); // get engine.

            engine.Initialize();
            engine.Start();

            while (true)
            {                
            }
        }

        private static void PrintBanner()
        {
            Console.WriteLine($@"

 ██░ ██▓██   ██▓ ██▓███  ▓█████  ██▓███   ▒█████   ▒█████   ██▓    
▓██░ ██▒▒██  ██▒▓██░  ██▒▓█   ▀ ▓██░  ██▒▒██▒  ██▒▒██▒  ██▒▓██▒    
▒██▀▀██░ ▒██ ██░▓██░ ██▓▒▒███   ▓██░ ██▓▒▒██░  ██▒▒██░  ██▒▒██░    
░▓█ ░██  ░ ▐██▓░▒██▄█▓▒ ▒▒▓█  ▄ ▒██▄█▓▒ ▒▒██   ██░▒██   ██░▒██░    
░▓█▒░██▓ ░ ██▒▓░▒██▒ ░  ░░▒████▒▒██▒ ░  ░░ ████▓▒░░ ████▓▒░░██████▒
 ▒ ░░▒░▒  ██▒▒▒ ▒▓▒░ ░  ░░░ ▒░ ░▒▓▒░ ░  ░░ ▒░▒░▒░ ░ ▒░▒░▒░ ░ ▒░▓  ░
 ▒ ░▒░ ░▓██ ░▒░ ░▒ ░      ░ ░  ░░▒ ░       ░ ▒ ▒░   ░ ▒ ▒░ ░ ░ ▒  ░
 ░  ░░ ░▒ ▒ ░░  ░░          ░   ░░       ░ ░ ░ ▒  ░ ░ ░ ▒    ░ ░   
 ░  ░  ░░ ░                 ░  ░             ░ ░      ░ ░      ░  ░
        ░ ░                                                        
");
            Console.WriteLine($" v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            Console.WriteLine($" https://github.com/bonesoul/hypepool");
            Console.WriteLine();
        }
    }
}
