using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace HelpDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string XmlConfigurationFileLocation { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            MessageBus msg = (MessageBus)this.Resources["APPMessageBus"];
            DependencyInjection.SimpleContainer.MapInstance<ImessageBus>(msg);



            string[] args = e.Args;
            if (args.Count() > 0)
                DoArgs(args);
            base.OnStartup(e);

           

            MainWindow main = new MainWindow();
            


            try
            {
                main.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message,"ApplicationMSG");
            }
            
        }

        private void DoArgs(string[] args)
        {
            foreach (string item in args)
            {
                if (item == "-resetcred")
                {
                    HelpDesk.Properties.Settings.Default.txt_AdminUser = "";
                    HelpDesk.Properties.Settings.Default.txt_AdminPassword = "";
                    HelpDesk.Properties.Settings.Default.txt_Domain = "";
                    HelpDesk.Properties.Settings.Default.XmlConfigurationFileLocation = "";
                    HelpDesk.Properties.Settings.Default.Save();
                }
                if (item == "-resetall")
                {
                    foreach (SettingsProperty currentProperty in HelpDesk.Properties.Settings.Default.Properties)
                    {
                        HelpDesk.Properties.Settings.Default[currentProperty.Name] = string.Empty;
                        HelpDesk.Properties.Settings.Default.Save();
                    }
                }

            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            HelpDesk.Properties.Settings.Default.Save();
        }


    }
}
