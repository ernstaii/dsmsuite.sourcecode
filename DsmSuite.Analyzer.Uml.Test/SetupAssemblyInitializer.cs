// SPDX-License-Identifier: GPL-3.0-or-later
using System.Reflection;
using DsmSuite.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Uml.Test
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);
        }
    }
}
