using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.services
{
    public class PropertyChangeAction<T> : IUndoableAction
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;
        private readonly T _oldValue;
        private readonly T _newValue;

        public PropertyChangeAction(Func<T> getter, Action<T> setter, T oldValue, T newValue)
        {
            _getter = getter;
            _setter = setter;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Undo() => _setter(_oldValue);
        public void Redo() => _setter(_newValue);
        public string Description => $"Changed from {_oldValue} to {_newValue}";
    }
}
