using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.services
{
    public class UndoRedoManager
    {
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();

        public  int UndoCount ;
        public int RedoCount;

        public void Execute(IUndoableAction action)
        {
            action.Redo();
            _undoStack.Push(action);
            _redoStack.Clear();
            UndoCount = _undoStack.Count;
            RedoCount = _redoStack.Count;
        }

        public void Undo()
        {
            if (_undoStack.Any())
            {
                
                var action = _undoStack.Pop();
                action.Undo();
                _redoStack.Push(action);
                UndoCount = _undoStack.Count;
                RedoCount = _redoStack.Count;
            }
        }

        public void Redo()
        {
            if (_redoStack.Any())
            {
                var action = _redoStack.Pop();
                action.Redo();
                _undoStack.Push(action);
                RedoCount = _redoStack.Count;
                UndoCount = _undoStack.Count;
            }
        }
    }
}
