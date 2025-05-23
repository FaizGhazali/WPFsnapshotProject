using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.services
{
    public interface IUndoableAction
    {
        void Undo();
        void Redo();
        string Description { get; }
    }
}
