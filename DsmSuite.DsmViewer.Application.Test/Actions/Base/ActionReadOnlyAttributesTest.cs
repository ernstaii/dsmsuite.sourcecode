using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionReadOnlyAttributesTest
    {
        [TestMethod]
        public void GivenStringValueInDictionaryWhenGetStringIsCalledThenValueIsReturned()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            string memberValue = "value";
            data[key] = memberValue;

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(model.Object, data);
            Assert.AreEqual(memberValue, attributes.GetString(memberName));
        }

        [TestMethod]
        public void GivenIntValueInDictionaryWhenGetIntIsCalledThenValueIsReturned()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            int memberValue = 7;
            data[key] = memberValue.ToString();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(model.Object, data);
            Assert.AreEqual(7, attributes.GetInt(memberName));
        }

        [TestMethod]
        public void GivenNullableIntInDictionaryWhenGetNullableIntIsCalledThenValueIsReturned()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            int memberValue = 7;
            data[key] = memberValue.ToString();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(model.Object, data);
            int? readValue = attributes.GetNullableInt(memberName);
            Assert.IsTrue(readValue.HasValue);
            Assert.AreEqual(7, readValue.Value);
        }

        [TestMethod]
        public void GivenNullableIntNotInDictionaryWhenGetNullableIntIsCalledThenNullValueIsReturned()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(model.Object, data);
            int? readValue = attributes.GetNullableInt(memberName);
            Assert.IsFalse(readValue.HasValue);
        }

        [TestMethod]
        public void GivenMultipleValuesInDictionaryWhenGetValueIsCalledThenAllAreReturned()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key1 = "name1";
            string memberValue1 = "some_value1";
            data[key1] = memberValue1;

            string key2 = "name2";
            int memberValue2 = 7;
            data[key2] = memberValue2.ToString();

            string memberName1 = "_name1";
            string memberName2 = "_name2";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(model.Object, data);
            Assert.AreEqual(memberValue1, attributes.GetString(memberName1));
            Assert.AreEqual(memberValue2, attributes.GetInt(memberName2));
        }

        [TestMethod]
        public void GetListIntTest()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["empty"] = "";
            data["single"] = "1";
            data["five"] = "1,2,3,4,5";
            data["dups"] = "5,5,5,5";
            data["many"] = "2147483647,1,0,-1,-2147483648";
            ActionReadOnlyAttributes atts = new ActionReadOnlyAttributes(model.Object, data);
            CollectionAssert.AreEqual(new List<int>(), atts.GetListInt("_empty"));
            CollectionAssert.AreEqual(new List<int>() {1}, atts.GetListInt("_single"));
            CollectionAssert.AreEqual(new List<int>() {1,2,3,4,5}, atts.GetListInt("_five"));
            CollectionAssert.AreEqual(new List<int>() {5,5,5,5}, atts.GetListInt("_dups"));
            CollectionAssert.AreEqual(new List<int>() {int.MaxValue,1,0,-1,int.MinValue}, atts.GetListInt("_many"));


        }

        [TestMethod]
        public void GetListIntCompactTest()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["empty"] = "";
            data["single"] = "1";
            data["five"] = "1-5";
            data["fivehole"] = "1,2,4,5";
            data["sevenhole"] = "1-3,5-7";
            data["middle"] = "1,3-5,7";
            data["dups"] = "5,5,5,5";
            ActionReadOnlyAttributes atts = new ActionReadOnlyAttributes(model.Object, data);
            CollectionAssert.AreEqual(new List<int>(), atts.GetListIntCompact("_empty"));
            CollectionAssert.AreEqual(new List<int>() {1}, atts.GetListIntCompact("_single"));
            CollectionAssert.AreEqual(new List<int>() {1,2,3,4,5}, atts.GetListIntCompact("_five"));
            CollectionAssert.AreEqual(new List<int>() {1,2,3,5,6,7}, atts.GetListIntCompact("_sevenhole"));
            CollectionAssert.AreEqual(new List<int>() {1,3,4,5,7}, atts.GetListIntCompact("_middle"));
            CollectionAssert.AreEqual(new List<int>() {5,5,5,5}, atts.GetListIntCompact("_dups"));
        }
    }
}
