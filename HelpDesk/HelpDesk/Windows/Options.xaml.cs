using HelpDesk.UserControls;
using HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HelpDesk.Windows
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        OptionsViewModel vm;
        public Options()
        {
            InitializeComponent();
            vm = (OptionsViewModel)this.DataContext;
        }


        private void RemoteTreeView_Selected(object sender, RoutedEventArgs e)
        {
            ContentControlPanel.Content = new Remote2();

        }

        private void ActiveDirectoryTree_Selected(object sender, RoutedEventArgs e)
        {
            ContentControlPanel.Content = new ActiveDirectory2();

        }

        private void OptionWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            vm.Unloaded();
        }

        private void Users_Selected(object sender, RoutedEventArgs e)
        {
            ContentControlPanel.Content = new UsersView();
        }

    }
}
