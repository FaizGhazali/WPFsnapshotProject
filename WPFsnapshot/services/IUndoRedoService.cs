using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.services
{
    public interface IUndoRedoService
    {
        int UndoCount { get; set; }
        int RedoCount { get; set; }
    }
}
