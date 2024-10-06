﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Base;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionAttributesTest
    {
        [TestMethod]
        public void WhenSetStringIsCalledThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string memberName = "_name";
            string memberValue = "value";
            attributes.SetString(memberName, memberValue);

            string key = "name";
            IReadOnlyDictionary<string, string> data = attributes.Data;
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(memberValue, data[key]);
        }

        [TestMethod]
        public void WhenSetIntIsCalledThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string memberName = "_name";
            int memberValue = 7;
            attributes.SetInt(memberName, memberValue);

            string key = "name";
            IReadOnlyDictionary<string, string> data = attributes.Data;
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(memberValue.ToString(), data[key]);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWithNotNullThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string memberName = "_name";
            int memberValue = 7;
            attributes.SetNullableInt(memberName, memberValue);

            string key = "name";
            IReadOnlyDictionary<string, string> data = attributes.Data;
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(memberValue.ToString(), data[key]);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWitNullThenValueIsNotInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string memberName = "_name";
            attributes.SetNullableInt(memberName, null);

            IReadOnlyDictionary<string, string> data = attributes.Data;
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWitMultipleValuesThenAllValuesAreInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string memberName1 = "_name1";
            string memberValue1 = "value1";
            attributes.SetString(memberName1, memberValue1);

            string memberName2 = "_name2";
            int memberValue2 = 7;
            attributes.SetInt(memberName2, memberValue2);

            string key1 = "name1";
            string key2 = "name2";
            IReadOnlyDictionary<string, string> data = attributes.Data;
            Assert.AreEqual(2, data.Count);
            Assert.IsTrue(data.ContainsKey(key1));
            Assert.AreEqual(memberValue1, data[key1]);
            Assert.IsTrue(data.ContainsKey(key2));
            Assert.AreEqual(memberValue2.ToString(), data[key2]);
        }

        [TestMethod]
        public void SetListIntTest()
        {
            ActionAttributes atts = new ActionAttributes();
            atts.SetListInt("_empty", new List<int> { } );
            atts.SetListInt("_single", new List<int> {0});
            atts.SetListInt("_five", new List<int> {1,2,3,4,5});
            atts.SetListInt("_dups", new List<int> {5,5,5,5});
            atts.SetListInt("_many", new List<int> {int.MaxValue,1,0,-1,int.MinValue});

            IReadOnlyDictionary<string, string> data = atts.Data;
            Assert.AreEqual(5, data.Count);
            Assert.AreEqual(atts.Data["empty"], "");
            Assert.AreEqual(atts.Data["single"], "0");
            Assert.AreEqual(atts.Data["five"], "1,2,3,4,5");
            Assert.AreEqual(atts.Data["dups"], "5,5,5,5");
            Assert.AreEqual(atts.Data["many"], "2147483647,1,0,-1,-2147483648");
        }
    }
}
