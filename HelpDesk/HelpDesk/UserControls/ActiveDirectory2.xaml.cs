using HelpDesk.Commands;
using HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HelpDesk.UserControls
{
    /// <summary>
    /// Interaction logic for ActiveDirectory2.xaml
    /// </summary>
    public partial class ActiveDirectory2 : UserControl
    {
        public static event Action<bool, ActiveDirectoryObject> OnCheckActiveDirectoryObject = delegate { };
        ActiveDirectoryViewModel vm;
        public ActiveDirectory2()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm = (ActiveDirectoryViewModel)this.DataContext;
            if(vm !=null)
            {
                txt_AdminPassword.Password = vm.UserPassword;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Save();
        }

       
        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            ActiveDirectoryObject ado = (ActiveDirectoryObject)Enum.Parse(typeof(ActiveDirectoryObject), c.Tag.ToString());
            //ActiveDirectoryObject ado = (ActiveDirectoryObject)c.Tag;

            OnCheckActiveDirectoryObject((bool)c.IsChecked, ado);
        }

        private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private void Save()
        {
            vm.UserPassword = txt_AdminPassword.Password;

            vm.Save(txt_AdminPassword.Password);
        }
       
    }
}
