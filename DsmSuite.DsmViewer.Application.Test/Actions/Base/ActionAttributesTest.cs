// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.DsmViewer.Application.Actions.Base;

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
            atts.SetListInt("_empty", new List<int> { });
            atts.SetListInt("_single", new List<int> { 0 });
            atts.SetListInt("_five", new List<int> { 1, 2, 3, 4, 5 });
            atts.SetListInt("_dups", new List<int> { 5, 5, 5, 5 });
            atts.SetListInt("_many", new List<int> { int.MaxValue, 1, 0, -1, int.MinValue });

            IReadOnlyDictionary<string, string> data = atts.Data;
            Assert.AreEqual(5, data.Count);
            Assert.AreEqual("", atts.Data["empty"]);
            Assert.AreEqual("0", atts.Data["single"]);
            Assert.AreEqual("1,2,3,4,5", atts.Data["five"]);
            Assert.AreEqual("5,5,5,5", atts.Data["dups"]);
            Assert.AreEqual("2147483647,1,0,-1,-2147483648", atts.Data["many"]);
        }

        [TestMethod]
        public void SetListCompactTest()
        {
            ActionAttributes atts = new ActionAttributes();
            atts.SetListIntCompact("_empty", new List<int> { });
            atts.SetListIntCompact("_single", new List<int> { 0 });
            atts.SetListIntCompact("_five", new List<int> { 1, 2, 3, 4, 5 });
            atts.SetListIntCompact("_fivehole", new List<int> { 1, 2, 4, 5 });
            atts.SetListIntCompact("_sevenhole", new List<int> { 1, 2, 3, 5, 6, 7 });
            atts.SetListIntCompact("_middle", new List<int> { 1, 3, 4, 5, 7 });
            atts.SetListInt("_dups", new List<int> { 5, 5, 5, 5 });


            IReadOnlyDictionary<string, string> data = atts.Data;
            Assert.AreEqual(7, data.Count);
            Assert.AreEqual("", atts.Data["empty"]);
            Assert.AreEqual("0", atts.Data["single"]);
            Assert.AreEqual("1-5", atts.Data["five"]);
            Assert.AreEqual("1,2,4,5", atts.Data["fivehole"]);
            Assert.AreEqual("1-3,5-7", atts.Data["sevenhole"]);
            Assert.AreEqual("1,3-5,7", atts.Data["middle"]);
            Assert.AreEqual("5,5,5,5", atts.Data["dups"]);
        }
    }
}
