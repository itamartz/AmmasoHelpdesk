using Microsoft.Win32;
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

using System.IO;
using System.Diagnostics;


namespace HelpDesk.UserControls
{
    /// <summary>
    /// Interaction logic for Remote.xaml
    /// </summary>
    public partial class Remote : UserControl
    {
        private string XmlFileName = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Setting.xml");
        public Remote()
        {
            InitializeComponent();
            load();
        }



        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button)
            {
                Button b = e.Source as Button;
                BrowsForPath(b.Tag.ToString());
            }
        }

        private void BrowsForPath(string Title)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = string.Format("Brows for {0} exe", Title);


            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                switch (Title)
                {
                    case "Teamviewer":
                        txt_TeamviewerPath.Text = dlg.FileName;
                        break;
                    case "RemoteAssist":
                        txt_Remote_Assist_Path.Text = dlg.FileName;
                        break;
                    case "Vnc":
                        txt_Vnc_Path.Text = dlg.FileName;
                        break;
                    case "Dameware":
                        txt_Dameware_Path.Text = dlg.FileName;
                        break;
                    case "WOL":
                        txt_WOL.Text = dlg.FileName;
                        break;
                    default:
                        break;
                }

            }

        }

        private void load()
        {

            txt_TeamviewerPath.Text = Properties.Settings.Default.txt_TeamviewerPath;
            txt_TeamviewerOptions.Text = Properties.Settings.Default.txt_TeamviewerOption;

            txt_Remote_Assist_Path.Text = Properties.Settings.Default.txt_Remote_Assist_Path;
            txt_Remote_Assist_Options.Text = Properties.Settings.Default.txt_Remote_Assist_Option;

            txt_Vnc_Path.Text = Properties.Settings.Default.txt_Vnc_Path;
            txt_Vnc_Options.Text = Properties.Settings.Default.txt_Vnc_Option;

            txt_Dameware_Path.Text = Properties.Settings.Default.txt_Dameware_Path;
            txt_Dameware_Options.Text = Properties.Settings.Default.txt_Dameware_Option;

            txt_WOL.Text = Properties.Settings.Default.txt_WOL;

            string radioDefault = Properties.Settings.Default.RadioDefault;
            foreach (Grid items in StackRemotePrograms.Children)
            {

                foreach (var item in items.Children)
                {
                    if (item is RadioButton)
                    {
                        RadioButton i = (RadioButton)item;
                        string defaultRadio = Properties.Settings.Default.RadioDefault;
                        if (i.Name == defaultRadio)
                            i.IsChecked = true;
                    }
                }


            }


        }

        private void SaveChange()
        {
            Properties.Settings.Default.txt_TeamviewerPath = txt_TeamviewerPath.Text; ;
            Properties.Settings.Default.txt_TeamviewerOption = txt_TeamviewerOptions.Text;


            Properties.Settings.Default.txt_Remote_Assist_Path = txt_Remote_Assist_Path.Text;
            Properties.Settings.Default.txt_Remote_Assist_Option = txt_Remote_Assist_Options.Text;

            Properties.Settings.Default.txt_Vnc_Path = txt_Vnc_Path.Text;
            Properties.Settings.Default.txt_Vnc_Option = txt_Vnc_Options.Text;

            Properties.Settings.Default.txt_Dameware_Path = txt_Dameware_Path.Text;
            Properties.Settings.Default.txt_Dameware_Option = txt_Dameware_Options.Text;

            Properties.Settings.Default.txt_WOL = txt_WOL.Text;

            Properties.Settings.Default.Save();
        }

        private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveChange();
        }

        private void UCRemote_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveChange();
        }

        private void RadioButtons_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = e.Source as RadioButton;
            Properties.Settings.Default.RadioDefault = radio.Name;
           // Debug.WriteLine(radio.Name);
        }

        private void btn_Remote_Assist_Brows_Click(object sender, RoutedEventArgs e)
        {

        }






    }
}
