using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.model
{
    public class Contractor
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Guid ProjectGuid { get; set; }
    }
}
