﻿using System;
using System.Linq;
using System.Collections.Generic;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionManager
    {
        private readonly Stack<IAction> _undoActionStack;
        private readonly Stack<IAction> _redoActionStack;

        public event EventHandler ActionPerformed;

        public ActionManager()
        {
            _undoActionStack = new Stack<IAction>();
            _redoActionStack = new Stack<IAction>();
        }

        public void Execute(IAction action)
        {
            _undoActionStack.Push(action);
            _redoActionStack.Clear();
            action.Do();
            ActionPerformed?.Invoke(this, EventArgs.Empty);
            Logger.LogInfo("Do :{action.Description}");
        }

        public void Add(IAction action)
        {
            _undoActionStack.Push(action);
        }

        public bool CanUndo()
        {
            return _undoActionStack.Count > 0;
        }

        public IAction GetCurrentUndoAction()
        {
            IAction action = null;
            if (_undoActionStack.Count > 0)
            {
                action = _undoActionStack.Peek();
            }
            return action;
        }

        public void Undo()
        {
            if (_undoActionStack.Count > 0)
            {
                IAction action = _undoActionStack.Pop();
                if (action != null)
                {
                    _redoActionStack.Push(action);
                    action.Undo();
                    ActionPerformed?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Undo :{action.Description}");
                }
            }
        }

        public bool CanRedo()
        {
            return _redoActionStack.Count > 0;
        }

        public IAction GetCurrentRedoAction()
        {
            IAction action = null;
            if (_redoActionStack.Count > 0)
            {
                action = _redoActionStack.Peek();
            }
            return action;
        }

        public void Redo()
        {
            if (_redoActionStack.Count > 0)
            {
                IAction action = _redoActionStack.Pop();
                if (action != null)
                {
                    _undoActionStack.Push(action);
                    action.Do();
                    ActionPerformed?.Invoke(this, EventArgs.Empty);
                    Logger.LogInfo("Redo :{action.Description}");
                }
            }
        }

        public void Clear()
        {
            _undoActionStack.Clear();
            _redoActionStack.Clear();
        }

        public IEnumerable<IAction> GetUndoActions()
        {
            return _undoActionStack;
        }
    }
}
