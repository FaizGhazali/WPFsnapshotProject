using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.model
{
    public class Project : INotifyPropertyChanged
    {
        private Guid _guid;
        public Guid Guid
        {
            get => _guid;
            set { _guid = value; OnPropertyChanged(); } 
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public Project Clone()
        {
            return new Project
            {
                Guid = this.Guid,
                Name = this.Name,
                Contractors = this.Contractors
            };
        }



        public ObservableCollection<Contractor>? Contractors { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    
}
