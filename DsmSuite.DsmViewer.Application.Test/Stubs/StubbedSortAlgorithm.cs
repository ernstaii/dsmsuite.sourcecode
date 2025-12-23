// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.DsmViewer.Application.Sorting;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Stubs
{
    public class StubbedSortAlgorithm : ISortAlgorithm
    {
        private List<int> SortResult = new List<int>() {2,0,1};

        public StubbedSortAlgorithm(object[] args) { }

        public string Name => "Stub";

        public SortResult Sort()
        {
            return new SortResult(SortResult);
        }
    }
}
