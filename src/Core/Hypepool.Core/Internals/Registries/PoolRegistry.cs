﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hypepool.Common.Pools;
using SimpleInjector;

namespace Hypepool.Core.Internals.Registries
{
    public class PoolRegistry : IRegistry
    {
        private readonly Container _container;

        public PoolRegistry(Container container)
        {
            _container = container;
        }

        public void Run()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;

            var assemnblies =
                from file in new DirectoryInfo(root).GetFiles()
                where file.Extension.ToLower() == ".dll"
                select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));

            _container.RegisterCollection<IPool>(assemnblies);
        }
    }
}
