// SPDX-License-Identifier: GPL-3.0-or-later
using System.Reflection;

namespace DsmSuite.Analyzer.Cpp.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                // Assemblies in build\Release\net8.0 or 
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, @"..\..\..\..\DsmSuite.Analyzer.Cpp.Test.Data"));
            }
        }
    }
}
