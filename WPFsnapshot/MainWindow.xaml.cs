﻿using Microsoft.Extensions.DependencyInjection;
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
using WPFsnapshot.view;
using ActiproSoftware.Windows.Controls.Docking;
using WPFsnapshot.services;
using WPFsnapshot.factories;
using System.Threading.Tasks;
using WPFsnapshot.viewModel;
using System.Windows.Threading;
using System.Diagnostics;

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
        private Contractor _selectedContractor;
        public Contractor SelectedContractor
        {
            get => _selectedContractor;
            set
            {
                _selectedContractor = value;
                //_projectUndoStack.Clear();
                //_projectRedoStack.Clear();
                //TakeSnapshot();
                OnPropertyChanged();
            }
        }

       
        public string UndoButtonText => $"Undo ({_undoRedoService.UndoCount})";

        public string RedoButtonText => $"Redo ({_undoRedoService.RedoCount})";
        public String UndoCount => $"Undo ({_undoRedoService.UndoCount})"; 
        public String RedoCount => $"Redo ({_undoRedoService.RedoCount})"; 

        

        private TabUCVM _currentTabVM;
        private UndoRedoService _undoRedoService;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //DI
        private readonly SelectedProjectService _selectedProjectService;
        private readonly ITabUCVMFactory _tabFactory;

        //ICommand
        public ICommand SelectProjectCommand { get; }


        public MainWindow(SelectedProjectService sps, ITabUCVMFactory itf, UndoRedoService urs)
        {
            
            InitializeComponent();
            DataContext = this;

            _selectedProjectService = sps;
            _tabFactory = itf;
            SelectProjectCommand = new RelayCommand<Project>(SelectProject);
            _undoRedoService = urs;

            dockSite.WindowActivated += DockSite_WindowActivated!;

            _undoRedoService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoService.UndoCount))
                    OnPropertyChanged(nameof(UndoCount));
                if (e.PropertyName == nameof(UndoRedoService.RedoCount))
                    OnPropertyChanged(nameof(RedoCount));
            };

            var dbLink = App.ServiceProvider!.GetRequiredService<IDBconnection>();
            var oriProjects =dbLink.GetAllRecords<Project>("Project");
            var oriContractors = dbLink.GetAllRecords<Contractor>("Contractor");

            Projects = new ObservableCollection<Project>(
                oriProjects.Select(project =>
                {
                    // Get contractors linked to this project
                    var linkedContractors = oriContractors
                        .Where(c => c.ProjectGuid == project.Guid)
                        .Select(c =>
                        {
                            c.ParentProject = project; // <-- Set the parent reference
                            return c;
                        })
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
        private void SelectProject(Project project)
        {
            _selectedProjectService.SelectedProject = project;
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Project project)
            {
                //SelectedProject = project;

                if (this.SelectProjectCommand.CanExecute(project))
                {
                    this.SelectProjectCommand.Execute(project);
                }
                SelectedProject = _selectedProjectService.SelectedProject;
                AddNewTab(_selectedProjectService.SelectedProject);
            }
            else if (e.NewValue is Contractor contractor)
            {
                SelectedContractor = contractor;
                var parent = contractor.ParentProject;
                if (parent != null)
                {
                    SelectedProject = parent;
                    Console.WriteLine($"Parent project: {parent.Name}");
                }
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
            //UndoCount++;


        }
        

        private void GuidBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
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


        //----------------------------------------------tab test----------------------------------------
        private void AddNewTab(Project project)
        {

            var existingTab = dockSite.DocumentWindows.FirstOrDefault(w => w.Tag is Guid g && g == project.Guid);

            if (existingTab != null)
            {
                Debug.WriteLine(existingTab.Content?.GetType().FullName);
                // Tab already exists, just focus it
                existingTab.Activate();
                var scrollViewer = existingTab.Content as ScrollViewer;
                var userControl = scrollViewer?.Content as UserControl;
                var vm = userControl?.DataContext as TabUCVM;
                vm?.UpdateUndoRedoService();

                return;
            }
            var tabUCviewModel = _tabFactory.Create(project);
            var tabUC = _tabFactory.Create();
            tabUC.DataContext = tabUCviewModel;
            
            var scrollable = new ScrollViewer
            {
                Content = tabUC,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                

            };
            
            var document = new DocumentWindow(dockSite)
            {
                Title = $"Project ({project.Name})",
                Content = scrollable,
                CanClose = true,
                Tag = project.Guid
            };

            
            // Optionally set MDI host
            document.Activate(); // Opens the tab and focuses it
            document.Focus();
        }
        private void DockSite_WindowActivated(object sender, DockingWindowEventArgs e)
        {
            if (e.Window is DocumentWindow window &&
                window.Content is ScrollViewer scroll &&
                scroll.Content is TabUC tabUC &&
                tabUC.DataContext is TabUCVM vm)
            {
                _currentTabVM = vm;
            }
            else
            {
                _currentTabVM = null!;
            }
        }
        private void Undo_Click12(object sender, RoutedEventArgs e)
        {
            _currentTabVM?.DoUndo();
        }

        private void Redo_Click12(object sender, RoutedEventArgs e)
        {
            _currentTabVM?.DoRedo();
        }

        

    }
}