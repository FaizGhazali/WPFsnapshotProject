using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFsnapshot.model;

namespace WPFsnapshot.services
{
    public class SelectedProjectService : INotifyPropertyChanged
    {
        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProject)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
