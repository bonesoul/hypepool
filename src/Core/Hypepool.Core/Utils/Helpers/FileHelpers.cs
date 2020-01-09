﻿#region license
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hypepool.Core.Utils.Helpers
{
    public static class FileHelpers
    {
        public static string AssemblyRoot => AppDomain.CurrentDomain.BaseDirectory;

        public static DirectoryInfo GetCurrentDirectory()
        {
            var entryAssembly = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var entryAssemblyFileInfo = new FileInfo(entryAssembly.AbsolutePath);

            return entryAssemblyFileInfo.Directory;
        }

        public static string GetAbsolutePath(string file)
        {
            var path = Path.Combine(AssemblyRoot, file); // first get the path as *unix paths.
            path = path.Replace('/', '\\'); // replace to windows-native paths.

            return path;
        }

        public static List<string> GetFilesByExtension(string directory, string expectedExtension)
        {
            var files = new List<string>(); // Store results in the file results list.

            try
            {
                var topDir = GetAbsolutePath(directory);
                var dirInfo = new DirectoryInfo(topDir);

                files.AddRange(from fileInfo in dirInfo.GetFiles()
                    where string.Compare(fileInfo.Extension, expectedExtension, StringComparison.OrdinalIgnoreCase) == 0
                    select $"{directory}/{fileInfo.Name}");
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

            return files;
        }
    }

}
