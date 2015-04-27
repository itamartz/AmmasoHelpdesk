using HelpDesk.Windows;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using WPFAutoCompleteTextbox;
using System.IO;
using HelpDesk.UserControls;
using System.Threading.Tasks;
using System.Reflection;
using System.Deployment;
using System.Xml;
using System.Configuration;
using HelpDesk.ViewModel;
using HelpDesk.Commands;

namespace HelpDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComputerCommands _ComputerCommandsviewModel;
        private UserCommands _UserCommandsviewModel;
        private PrinterCommands _PrinterCommandsviewModel;

        private ImessageBus MessageBus;
        public MainWindow()
        {
            InitializeComponent();

            _ComputerCommandsviewModel = (ComputerCommands)this.Resources["ComputerCommandviewModel"];
            _UserCommandsviewModel = (UserCommands)this.Resources["UserCommandsviewModel"];
            _PrinterCommandsviewModel = (PrinterCommands)this.Resources["PrinterCommandsviewModel"];
            MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();
           
        }
        
        #region MenuItemFunctions
        private void MenuOption_Click(object sender, RoutedEventArgs e)
        {
            Options window = new Options();
            #region BeginAnimation
            ////window.WindowStyle = WindowStyle.None;
            ////window.AllowsTransparency = true;
            ////window.Background = Brushes.Green;
            //DoubleAnimation animFadeIn = new DoubleAnimation();
            //animFadeIn.From = 0;
            //animFadeIn.To = 0.5;
            //animFadeIn.Duration = new Duration(TimeSpan.FromSeconds(2));
            //window.BeginAnimation(Window.OpacityProperty, animFadeIn); 
            #endregion

            try
            {
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "MenuOption_Click");
            }

        }

        #region close
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
        }
        private void CloseWindows()
        {
            this.Close();
        }

        #endregion
        #endregion

        #region StackPanelElse
        private void txtNetsend_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            txtNetsend.Width = txtNetsend.Width == 150 ? 50 : 150;
            txtNetsend.Height = txtNetsend.Height == 150 ? 28 : 150;
        }

        private void btnNetSend_Click(object sender, RoutedEventArgs e)
        {
            string msg = txtNetsend.Text;
            _ComputerCommandsviewModel.NetSend(msg,txt_Computer.Text);
            txtNetsend.Text = string.Empty;
        }
        #endregion

        #region PrinterFunction
        private void PrinterProperties_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterProperties(txt_PritnerName.Text);
        }

        private void PrinterQueue_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterQueue(txt_PritnerName.Text);
        }

        private void PrinterServerRdp_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterRDP(txt_PritnerName.Text);
        }
        private void PrinterPing_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterPing(txt_PritnerName.Text);
        }
        private void Printerhttp_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterHttp(txt_PritnerName.Text);

        }
        private void PrinterTestPage_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterTestPage(txt_PritnerName.Text);
        }
        private void PrinterInkLevel_Click(object sender, RoutedEventArgs e)
        {
            _PrinterCommandsviewModel.PrinterInkLevel(txt_PritnerName.Text);
        }
        #endregion

        #region Keybord

        List<Key> keys = new List<Key>();
      

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            SetKeyDown(e.Key);

            if (IsKeyDown(Key.D1) && IsKeyDown(Key.LeftCtrl))
            {
                keys.Clear();
                MessageBus.Publish<DefaultProgram>(new DefaultProgram());
            }
            if (IsKeyDown(Key.D2) && IsKeyDown(Key.LeftCtrl))
            {
                keys.Clear();
                MessageBus.Publish<RDPProgram>(new RDPProgram());
            }
            if (IsKeyDown(Key.F5))
            {
                keys.Clear();
                MessageBus.Publish<Refresh>(new Refresh());
            }


            if (IsKeyDown(Key.D0) && IsKeyDown(Key.LeftCtrl))
            {
                keys.Clear();
                foreach (SettingsProperty currentProperty in HelpDesk.Properties.Settings.Default.Properties)
                {
                    if (currentProperty.PropertyType.FullName == "System.Double")
                    {
                        HelpDesk.Properties.Settings.Default[currentProperty.Name] = 0.0;
                    }
                    else if (currentProperty.PropertyType.FullName == "System.String")
                    {
                        HelpDesk.Properties.Settings.Default[currentProperty.Name] = string.Empty;
                    }

                    else if (currentProperty.PropertyType.FullName == "System.Boolean")
                    {
                        HelpDesk.Properties.Settings.Default[currentProperty.Name] = false;
                    }
                }
                HelpDesk.Properties.Settings.Default.Save();


            }
            if (IsKeyDown(Key.Divide) && IsKeyDown(Key.LeftCtrl))
            {
                keys.Clear();
                Process.Start(App.XmlConfigurationFileLocation);
            }

            if (IsKeyDown(Key.OemTilde) && IsKeyDown(Key.LeftCtrl))
            {
                keys.Clear();
                //MessageBus.Publish<PublishHubConnection>(new PublishHubConnection());
                
            }

            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SetKeyUp(e.Key);
        }

        private bool IsKeyDown(Key key)
        {
            return keys.Contains(key);
        }

        private void SetKeyDown(Key key)
        {
            if (!keys.Contains(key))
                keys.Add(key);
        }

        private void SetKeyUp(Key key)
        {
            if (keys.Contains(key))
                keys.Remove(key);
        }
        #endregion
    }
}
