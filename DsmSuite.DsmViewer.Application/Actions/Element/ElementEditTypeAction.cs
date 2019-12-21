﻿using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditTypeAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly string _old;
        private readonly string _new;

        public ElementEditTypeAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _old = attributes.GetString(nameof(_old));
            _new = attributes.GetString(nameof(_new));
       }

        public ElementEditTypeAction(IDsmModel model, IDsmElement element, string type) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);

            _old = _element.Type;
            _new = type;
        }

        public override string ActionName => nameof(ElementEditNameAction);
        public override string Title => "Edit element type";
        public override string Description => $"element={_element.Fullname} type={_old}->{_new}";

        public override void Do()
        {
            Model.EditElementType(_element, _new);
        }

        public override void Undo()
        {
            Model.EditElementType(_element, _old);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_element), _element.Id);
            attributes.SetString(nameof(_old), _old);
            attributes.SetString(nameof(_new), _new);
            return attributes.GetData();
        }
    }
}