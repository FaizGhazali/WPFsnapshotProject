using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFsnapshot.viewModel;

namespace WPFsnapshot.view
{
    /// <summary>
    /// Interaction logic for TabUC.xaml
    /// </summary>
    public partial class TabUC : UserControl
    {
        public TabUC()
        {
            InitializeComponent();
            
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as TabUCVM;
            if (vm != null)
            {
                vm.RenameProject();
            }
            var target = this.dummyTextbox;
            target.Focus();
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as TabUCVM;
            if (vm != null)
            {
                vm.HandleClone();
            }
        }

        private void TextBox_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as TabUCVM;
            if (vm != null)
            {
                vm.HandleCloneGame();
            }
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as TabUCVM;
            if (vm != null)
            {
                vm.RenameGame();
            }
            var target = this.dummyTextbox;
            target.Focus();
        }
    }
}
