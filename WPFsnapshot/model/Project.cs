using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.model
{
    public class Project
    {
        [Column("Guid")]
        public Guid Guid { get; set; }
        public string Name { get; set; }


        //how do i make this can exist or not exist
        public ObservableCollection<Contractor>? Contractors { get; set; }
    }
    
    
}
