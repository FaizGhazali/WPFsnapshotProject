using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFsnapshot.model
{
    public class Game : INotifyPropertyChanged
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

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public Game Clone()
        {
            return new Game
            {
                Guid = this.Guid,
                Name = this.Name,
                Description = this.Description
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
