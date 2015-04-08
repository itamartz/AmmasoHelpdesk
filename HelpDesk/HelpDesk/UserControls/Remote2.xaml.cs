using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpDesk.UserControls
{
    /// <summary>
    /// Interaction logic for Remote2.xaml
    /// </summary>
    public partial class Remote2 : System.Windows.Controls.UserControl
    {
        public static event Action<bool> NewRemoteSoftware;
        public static event Action<bool> ConfigurationFileLocation;
        XMLApi xml = new XMLApi();
        ObservableCollection<RemoteSoftware> obRemoteSoftware = new ObservableCollection<RemoteSoftware>();

        public Remote2()
        {
            InitializeComponent();
            GetRemoteSoftware();
        }

        private async void GetRemoteSoftware()
        {

            List<RemoteSoftware> listRemoteSoftware = await xml.GetXMLButtons();
            obRemoteSoftware = new ObservableCollection<RemoteSoftware>(listRemoteSoftware);
            this.DataContext = obRemoteSoftware;
            tctConfigurationPath.Text = Properties.Settings.Default.XmlConfigurationFileLocation;
            if (string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
                App.XmlConfigurationFileLocation = tctConfigurationPath.Text;

        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            obRemoteSoftware.Add(new RemoteSoftware()
                {
                    Name = "Default Name" + obRemoteSoftware.Count,
                    Index = obRemoteSoftware.Count
                });
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button b = (sender as System.Windows.Controls.Button);
            if (obRemoteSoftware.Count - 1 != 0)
            {
                //int index = Convert.ToInt32(b.Tag);
                RemoteSoftware sof = (b.Tag as RemoteSoftware);
                //RemoteSoftware sof = obRemoteSoftware[index];
                sof.Isremove = true;
                obRemoteSoftware.Remove(sof);

            }
            //SetIndex();
        }
        private void SetIndex()
        {
            foreach (RemoteSoftware item in obRemoteSoftware)
            {
                if (!item.Isremove)
                {
                    item.Index = obRemoteSoftware.IndexOf(item);

                }
            }
        }
        private void btnBrows_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Controls.Button b = (sender as System.Windows.Controls.Button);
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            RemoteSoftware re = (b.Tag as RemoteSoftware);
            dlg.Title = string.Format("Brows for {0} exe", re.Name);


            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                //TextBox te = FindVisualChildren<TextBox>(this).Where(t => t.Text == re.ProgramPath).FirstOrDefault();
                //te.Text = dlg.FileName;
                re.ProgramPath = dlg.FileName;
            }
        }
      
        IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveAllRemoteSoftware();
            if (NewRemoteSoftware != null)
                NewRemoteSoftware(true);
            obRemoteSoftware.Clear();
            obRemoteSoftware = null;
        }

        private void SaveAllRemoteSoftware()
        {
            xml.Save(obRemoteSoftware);
        }

        private void RadioDefault_Checked(object sender, RoutedEventArgs e)
        {
            RemoteSoftware remo = ((sender as System.Windows.Controls.RadioButton).Tag) as RemoteSoftware;
            remo.Default = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            dialog.Description = "Configuration file location";
            //Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tctConfigurationPath.Text = dialog.SelectedPath;
                App.XmlConfigurationFileLocation = dialog.SelectedPath;
                Properties.Settings.Default.XmlConfigurationFileLocation = App.XmlConfigurationFileLocation;
                Properties.Settings.Default.Save();
                if (ConfigurationFileLocation != null)
                    ConfigurationFileLocation(true);
                GetRemoteSoftware();
            }
        }


    }
}
