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

namespace WPFsnapshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<Project>? Projects { get; set; }
       
        public event PropertyChangedEventHandler PropertyChanged;
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
    }
}