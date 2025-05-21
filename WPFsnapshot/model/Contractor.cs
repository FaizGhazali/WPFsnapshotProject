using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.model
{
    public class Contractor : INotifyPropertyChanged
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
        private Guid _projectGuid;
        public Guid ProjectGuid
        {
            get => _projectGuid;
            set { _projectGuid = value; OnPropertyChanged(); }
        }

        public Contractor Clone()
        {
            return new Contractor
            {
                Guid = this.Guid,
                Name = this.Name,
                ProjectGuid = this.ProjectGuid
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
