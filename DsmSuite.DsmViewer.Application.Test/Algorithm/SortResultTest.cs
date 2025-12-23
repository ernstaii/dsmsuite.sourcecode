// SPDX-License-Identifier: GPL-3.0-or-later
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Sorting;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class SortResultTest
    {
        [TestMethod]
        public void WhenSortResultConstructedWithZeroSizeThenItIsInvalid()
        {
            SortResult result = new SortResult(0);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithNonZeroSizeThenItIsValid()
        {
            SortResult result = new SortResult(4);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(2, result.GetIndex(2));
            Assert.AreEqual(3, result.GetIndex(3));
        }

        [TestMethod]
        public void WhenSortResultConstructedWithEmptyListThenItIsInvalid()
        {
            SortResult result = new SortResult(new List<int>());
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithSingleNumberStringThenItIsValid()
        {
            SortResult result = new SortResult( new List<int> {0} );
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.GetIndex(0));
        }

        [TestMethod]
        public void WhenSortResultConstructedWithInvalidListThenItIsInvalid()
        {
            SortResult result = new SortResult( new List<int>() {3, 1, 0} );
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithListThenItIsValid()
        {
            SortResult result = new SortResult( new List<int>() {2, 1, 0} );
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }

        [TestMethod]
        public void SortResultConstructedWithList()
        {
            List<int> input = new List<int>() {3, 2, 1, 0};
            SortResult result = new SortResult(input);
            CollectionAssert.AreEqual(input, result.GetOrder());
        }

        [TestMethod]
        public void WhenSwapWithValidArgumentThenTheOrderIsChanged()
        {
            SortResult result = new SortResult( new List<int>() {2, 1, 0} );
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));

            result.Swap(0, 1);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.GetIndex(0));
            Assert.AreEqual(2, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }

        [TestMethod]
        public void WhenSwapWithOutOfBoundArgumentThenExceptionIsThrown()
        {
            SortResult result = new SortResult( new List<int>() { 2, 1, 0} );
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));

            Assert.Throws<ArgumentOutOfRangeException>(() => result.Swap(0, 3));
        }

        [TestMethod]
        public void WhenInvertOrderThenTheOrderIsChanged()
        {
            SortResult result = new SortResult( new List<int>() { 2, 0, 1} );
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));

            result.InvertOrder();

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.GetIndex(0));
            Assert.AreEqual(2, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }

        [TestMethod]
        public void WhenInvertOrderIsCalledTwiceThenTheOrderIsUnchanged()
        {
            SortResult result = new SortResult(new List<int>() { 2, 0, 1 });
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));

            result.InvertOrder();
            result.InvertOrder();

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));
        }

        [TestMethod]
        public void WhenGetIndexWithOutOfBoundArgumentThenExceptionIsThrown()
        {
            SortResult result = new SortResult(new List<int>() { 2, 1, 0 });
            Assert.IsTrue(result.IsValid);
            Assert.Throws<ArgumentOutOfRangeException>(() => result.GetIndex(3));
        }
    }
}
