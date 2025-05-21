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
using System.Collections.Generic;

namespace WPFsnapshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int CounterSnapshot =0;
        public bool CounterSnapshotSwitch = true;

        //stack
        private Stack<Project> _projectUndoStack = new();
        private Stack<Project> _projectRedoStack = new();
        private Stack<Project> _undoStack = new();
        private Stack<Project> _redoStack = new();

        private Stack<(int, object)>_snapshotUndo = new();
        private Stack<(int, object)>_snapshotRedo = new();
        private bool startRedo;
        private bool startUndo;
        //
        public ObservableCollection<Project>? Projects { get; set; }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                //_projectUndoStack.Clear();
                //_projectRedoStack.Clear();
                TakeSnapshot();
                OnPropertyChanged();
            }
        }

        private int _undoCount;
        public int UndoCount
        {
            get => _undoCount;
            set
            {
                _undoCount = value;
                OnPropertyChanged(nameof(UndoCount));
                OnPropertyChanged(nameof(UndoButtonText)); // update button text
            }
        }
        public string UndoButtonText => $"Undo ({UndoCount})";

        public string RedoButtonText => $"Redo ({RedoCount})";

        private int _redoCount;
        public int RedoCount
        {
            get => _redoCount;
            set
            {
                _redoCount = value;
                OnPropertyChanged(nameof(RedoCount));
                OnPropertyChanged(nameof(RedoButtonText)); // update button text
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
        private void Undo_Click2(object sender, RoutedEventArgs e)
        {
            if (_projectUndoStack.Count > 1)
            {
                var firstpop = _projectUndoStack.Pop();
                _projectRedoStack.Push(firstpop);
                var last = _projectUndoStack.Pop();
                
                SelectedProject.Name = last.Name;
                // Restore other fields too...
            }
        }
        private void Undo_Click3(object sender, RoutedEventArgs e)
        {
            if (_snapshotUndo.Count >0)
            {
                //if (startUndo = true)
                //{
                //    UndoCount = RedoCount;
                //}
                //var (lastCounter, lastProject) = _snapshotUndo.Peek();
                var snapshotUndoArray = _snapshotUndo.Reverse().ToArray();
                var (lastCounter, lastProject) = snapshotUndoArray[UndoCount-1];
                if (lastProject is Project lastProj){
                    SelectedProject.Name = lastProj.Name;
                }
                UndoCount = lastCounter;
                startRedo = true;
                startUndo = false;
                //UndoCount++;
                //SelectedProject.Name = lastProject.;

            }
        }
        private void Redo_Click2(object sender, RoutedEventArgs e)
        {
            if (_projectRedoStack.Count > 0)
            {
                var firstpop = _projectRedoStack.Pop();
                _projectUndoStack.Push(firstpop);
                var last = _projectRedoStack.Pop();

                SelectedProject.Name = last.Name;
            }
        }

        private void Redo_Click3(object sender, RoutedEventArgs e)
        {
            if (_snapshotRedo.Count > 0)
            {
                
                //var (lastCounter, lastProject) = _snapshotUndo.Peek();
                if(RedoCount!= UndoCount && startRedo ==true)
                {
                    RedoCount = UndoCount;
                }
                var snapshotRedoArray = _snapshotRedo.Reverse().ToArray();
                //RedoCount = UndoCount;
                var (lastCounter, lastProject) = snapshotRedoArray[RedoCount ];
                if (lastProject is Project lastProj)
                {
                    SelectedProject.Name = lastProj.Name;
                }
                
                RedoCount++;
                UndoCount = RedoCount;
                startRedo = false;
                startUndo = true;

            }
        }
        private void TakeSnapshot()
        {
            if (SelectedProject != null)
            {
                _undoStack.Push(SelectedProject.Clone());
                _redoStack.Clear();

            }
        }
        private void Undo()
        {
            if (_undoStack.Count == 0) return;

            var current = SelectedProject.Clone();
            _redoStack.Push(current); // Save current to redo stack

            var previous = _undoStack.Pop(); // Get previous state
            RestoreProject(previous);
        }

        private void Redo()
        {
            if (_redoStack.Count == 0) return;

            var current = SelectedProject.Clone();
            _undoStack.Push(current); // Save current to undo stack

            var next = _redoStack.Pop();
            RestoreProject(next);
        }
        private void RestoreProject(Project project)
        {
            if (SelectedProject == null) return;

            SelectedProject.Name = project.Name;
            // restore other properties too...
        }
        private void Undo_Click(object sender, RoutedEventArgs e) => Undo();

        private void Redo_Click(object sender, RoutedEventArgs e) => Redo();

        private void GuidBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void GuidBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //if(_snapshotUndo.Count == 0|| this.GuidTextbox.Text != SelectedProject.Guid.ToString())
            //{
            //    _snapshotUndo.Push((CounterSnapshot, SelectedProject.Clone()));
            //    UndoCount += 1;
            //}

            var newSnapshot = (CounterSnapshot, SelectedProject.Clone());

            if (_snapshotUndo.Count > 0)
            {
                var (lastCounter, lastProject) = _snapshotUndo.Peek();
                if (lastProject is Project lastProj && SelectedProject is Project selectedProj)
                {
                    if (lastProj.Guid == selectedProj.Guid)
                    {
                        bool isSame =
                            lastCounter == newSnapshot.CounterSnapshot &&
                            lastProj.Guid == SelectedProject.Guid &&
                            lastProj.Name == SelectedProject.Name;

                        if (isSame)
                            return; // Same as last, skip push
                    }
                }

                
            }

            // New or different snapshot — push
            _snapshotUndo.Push(newSnapshot);
            UndoCount++;


        }
        

        private void GuidBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedProject == null)
                return;
            //if (this.GuidTextbox.Text !=SelectedProject.Guid.ToString())
            //{
                var newSnapshot = (CounterSnapshot, SelectedProject.Clone());
                if (_snapshotRedo.Count > 0)
                {
                    var (lastCounter, lastProject) = _snapshotRedo.Peek();
                    if (lastProject is Project lastProj && SelectedProject is Project selectedProj)
                    {
                        if (lastProj.Guid == selectedProj.Guid)
                        {
                            bool isSame =
                              lastProj.Guid == SelectedProject.Guid &&
                              lastProj.Name == SelectedProject.Name;
                            if (isSame)
                            {
                                return; // Same as last snapshot — skip
                            }
                        }
                    }
               
                        
                }

                _snapshotRedo.Push((CounterSnapshot, SelectedProject.Clone()));
                CounterSnapshot += 1;
                RedoCount += 1;
            //}
        }
        private void SnapshotCounter()
        {
            if (CounterSnapshotSwitch == true)
            {
                CounterSnapshotSwitch = false;
                //cari max index of the snapshot
            }
        }
        private void SnapshotProcess()
        {

        }
    }
}