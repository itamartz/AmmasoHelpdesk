using HelpDesk.Commands;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpDesk.UserControls
{
    /// <summary>
    /// Interaction logic for ActiveDirectory.xaml
    /// </summary>
    public partial class ActiveDirectory : UserControl
    {
        public static event Action<bool, ActiveDirectoryObject> OnCheckActiveDirectoryObject = delegate { };

        public ActiveDirectory()
        {
            InitializeComponent();
        }
        private void UCActiveDirectory_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void UCActiveDirectory_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveChange();
        }

        private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveChange();
        }

        private void load()
        {
            txt_AdminPassword.Password = Utility.AdminPassword;   //Properties.Settings.Default.txt_AdminPassword;
        }

        private void SaveChange()
        {
            Utility.AdminPassword = txt_AdminPassword.Password;
        }

        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox c = sender as CheckBox;
            ActiveDirectoryObject ado = (ActiveDirectoryObject)Enum.Parse(typeof(ActiveDirectoryObject), c.Tag.ToString());
            OnCheckActiveDirectoryObject((bool)c.IsChecked, ado);

        }
    }

   
}
