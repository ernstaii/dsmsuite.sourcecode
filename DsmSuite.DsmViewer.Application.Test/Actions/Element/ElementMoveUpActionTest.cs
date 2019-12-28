﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Actions.Element;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementMoveUpActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;
        private Mock<IDsmElement> _previousElement;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();
            _previousElement = new Mock<IDsmElement>();
            _element.Setup(x => x.Id).Returns(1);
            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString()
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementIsRemovedFromDataModel()
        {
            _model.Setup(x => x.PreviousSibling(_element.Object)).Returns(_previousElement.Object);

            ElementMoveUpAction action = new ElementMoveUpAction(_model.Object, _element.Object);
            action.Do();

            _model.Verify(x => x.Swap(_element.Object, _previousElement.Object), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementIsRestoredInDataModel()
        {
            _model.Setup(x => x.NextSibling(_element.Object)).Returns(_previousElement.Object);

            ElementMoveUpAction action = new ElementMoveUpAction(_model.Object, _element.Object);
            action.Undo();

            _model.Verify(x => x.Swap(_previousElement.Object, _element.Object), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, _data };
            ElementMoveUpAction action = new ElementMoveUpAction(args);

            Assert.AreEqual(1, action.Data.Count);
            Assert.AreEqual(ElementId.ToString(), _data["element"]);
        }
    }
}
