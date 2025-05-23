using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.services
{
    public class UndoRedoService : INotifyPropertyChanged, IUndoRedoService
    {
        private int _undoCount;
        private int _redoCount;

        public int UndoCount
        {
            get => _undoCount;
             set
            {
                if (_undoCount != value)
                {
                    _undoCount = value;
                    OnPropertyChanged(nameof(UndoCount));
                }
            }
        }

        public int RedoCount
        {
            get => _redoCount;
            set
            {
                if (_redoCount != value)
                {
                    _redoCount = value;
                    OnPropertyChanged(nameof(RedoCount));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
