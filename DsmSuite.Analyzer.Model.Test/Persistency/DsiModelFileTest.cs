﻿using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DsmSuite.Analyzer.Model.Test.Persistency
{
    [TestClass]
    public class DsiModelFileTest : IDsiModelFileCallback
    {
        private readonly List<IDsiElement> _elements = new List<IDsiElement>();
        private readonly List<IDsiRelation> _relations = new List<IDsiRelation>();
        private readonly Dictionary<string, List<IDsiMetaDataItem>> _metaData = new Dictionary<string, List<IDsiMetaDataItem>>();

        [TestInitialize]
        public void TestInitialize()
        {
            _elements.Clear();
            _relations.Clear();
            _metaData.Clear();
        }

        [TestMethod]
        public void TestLoadModel()
        {
            string inputFilename = "DsmSuite.Analyzer.Model.Test.Input.dsi";

            DsiModelFile modelFile = new DsiModelFile(inputFilename, this);
            modelFile.Load(null);

            Assert.AreEqual(2, _metaData.Count);
            Assert.AreEqual(2, _metaData["group1"].Count);
            Assert.AreEqual("item1", _metaData["group1"][0].Name);
            Assert.AreEqual("value1", _metaData["group1"][0].Value);
            Assert.AreEqual("item2", _metaData["group1"][1].Name);
            Assert.AreEqual("value2", _metaData["group1"][1].Value);
            Assert.AreEqual(2, _metaData["group2"].Count);
            Assert.AreEqual("item3", _metaData["group2"][0].Name);
            Assert.AreEqual("value3", _metaData["group2"][0].Value);
            Assert.AreEqual("item4", _metaData["group2"][1].Name);
            Assert.AreEqual("value4", _metaData["group2"][1].Value);

            Assert.AreEqual(3, _elements.Count);
            Assert.AreEqual(1, _elements[0].Id);
            Assert.AreEqual("a.a1", _elements[0].Name);
            Assert.AreEqual("elementtype1", _elements[0].Type);
            Assert.AreEqual("source1", _elements[0].Source);

            Assert.AreEqual(2, _elements[1].Id);
            Assert.AreEqual("a.a2", _elements[1].Name);
            Assert.AreEqual("elementtype2", _elements[1].Type);
            Assert.AreEqual("source2", _elements[1].Source);

            Assert.AreEqual(3, _elements[2].Id);
            Assert.AreEqual("b.b1", _elements[2].Name);
            Assert.AreEqual("elementtype3", _elements[2].Type);
            Assert.AreEqual("source3", _elements[2].Source);

            Assert.AreEqual(2, _relations.Count);
            Assert.AreEqual(1, _relations[0].Consumer);
            Assert.AreEqual(2, _relations[0].Provider);
            Assert.AreEqual("relationtype1", _relations[0].Type);
            Assert.AreEqual(100, _relations[0].Weight);

            Assert.AreEqual(2, _relations[1].Consumer);
            Assert.AreEqual(3, _relations[1].Provider);
            Assert.AreEqual("relationtype2", _relations[1].Type);
            Assert.AreEqual(200, _relations[1].Weight);
        }

        [TestMethod]
        public void TestSaveModel()
        {
            string inputFilename = "DsmSuite.Analyzer.Model.Test.Input.dsi";
            string outputFilename = "DsmSuite.Analyzer.Model.Test.Output.dsi";

            FillModelData();

            DsiModelFile modelFile = new DsiModelFile(outputFilename, this);
            modelFile.Save(false, null);

            Assert.IsTrue(File.ReadAllBytes(outputFilename).SequenceEqual(File.ReadAllBytes(inputFilename)));
        }

        private void FillModelData()
        {
            _metaData["group1"] = new List<IDsiMetaDataItem>
            {
                new DsiMetaDataItem("item1", "value1"),
                new DsiMetaDataItem("item2", "value2")
            };

            _metaData["group2"] = new List<IDsiMetaDataItem>
            {
                new DsiMetaDataItem("item3", "value3"),
                new DsiMetaDataItem("item4", "value4")
            };

            _elements.Add(new DsiElement(1, "a.a1", "elementtype1", "source1"));
            _elements.Add(new DsiElement(2, "a.a2", "elementtype2", "source2"));
            _elements.Add(new DsiElement(3, "b.b1", "elementtype3", "source3"));

            _relations.Add(new DsiRelation(1, 2, "relationtype1", 100));
            _relations.Add(new DsiRelation(2, 3, "relationtype2", 200));
        }

        public void ImportMetaDataItem(string groupName, string itemName, string itemValue)
        {
            if (!_metaData.ContainsKey(groupName))
            {
                _metaData[groupName] = new List<IDsiMetaDataItem>();
            }

            DsiMetaDataItem dsiMetaDataItem = new DsiMetaDataItem(itemName, itemValue);
            _metaData[groupName].Add(dsiMetaDataItem);
        }

        public void ImportElement(int elementId, string name, string type, string source)
        {
            DsiElement element = new DsiElement(elementId, name, type, source);
            _elements.Add(element);
        }

        public void ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            DsiRelation relation = new DsiRelation(consumerId, providerId, type, weight);
            _relations.Add(relation);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaData.Keys;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            if (_metaData.ContainsKey(groupName))
            {
                return _metaData[groupName];
            }
            else
            {
                return new List<IDsiMetaDataItem>();
            }
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            return _elements;
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            return _relations;
        }
    }
}
