using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hypepool.Cli.Utils
{
    public static class ConsoleExtensions
    {
        /// <summary>
        /// Prints an info banner.
        /// </summary>
        internal static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
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
            Console.WriteLine();
        }

        /// <summary>
        /// Prints a copyright banner.
        /// </summary>
        internal static void PrintLicense()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" Copyright (C) 2013 - 2018, hypepool project - Hüseyin Uslu");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(" https://github.com/bonesoul/hypepool");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(" hypepool comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" You can contribute the development of the project by donating;");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ETH : 0x61aa3e0709e20bcb4aedc2607d4070f1db72e69b");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
