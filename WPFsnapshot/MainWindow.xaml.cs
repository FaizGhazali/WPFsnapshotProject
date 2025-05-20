using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SnapShotHelper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WPFsnapshot.model;
using System;
using System.Runtime.CompilerServices;

namespace WPFsnapshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //undo stack
        private Stack<Project> _projectUndoStack = new();
        //
        public ObservableCollection<Project>? Projects { get; set; }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                _projectUndoStack.Clear();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = this;

            var dbLink = App.ServiceProvider!.GetRequiredService<IDBconnection>();
            var oriProjects =dbLink.GetAllRecords<Project>("Project");
            var oriContractors = dbLink.GetAllRecords<Contractor>("Contractor");

            Projects = new ObservableCollection<Project>(
                 oriProjects.Select(project =>
                 {
                     // Get contractors linked to this project
                     var linkedContractors = oriContractors
                         .Where(c => c.ProjectGuid == project.Guid)
                         .ToList();

                     // Create a new Project with Contractors collection
                     return new Project
                     {
                         Guid = project.Guid,
                         Name = project.Name,
                         Contractors = new ObservableCollection<Contractor>(linkedContractors)
                     };
                 })
             );
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if (DataContext is MainViewModel vm)
            //    vm.SelectedPerson = e.NewValue as Person;
            if (e.NewValue is Project project)
            {
                SelectedProject = project;
            }

        }
        private string _lastSnapshotName = "";

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedProject == null)
                return;

            // Only snapshot if something actually changed
            if (SelectedProject.Name != _lastSnapshotName)
            {
                _projectUndoStack.Push(SelectedProject.Clone());
                _lastSnapshotName = SelectedProject.Name;
            }
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (_projectUndoStack.Count > 1)
            {
                _projectUndoStack.Pop();
                var last = _projectUndoStack.Pop();
                SelectedProject.Name = last.Name;
                // Restore other fields too...
            }
        }
    }
}