// SPDX-License-Identifier: GPL-3.0-or-later
using System;

namespace DsmSuite.Analyzer.DotNet.Test.Data
{
    public class MainClient
    {
        MainClient()
        {
            MainType main = new MainType();
            main.GenericEvent += HandleEvent;
        }

        private void HandleEvent(object sender, EventsArgsGenericParameter e)
        {
            throw new NotImplementedException();
        }
    }
}
