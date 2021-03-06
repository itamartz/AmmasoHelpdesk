﻿using HelpDesk.Commands;
using HelpDesk.UserControls;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

using Microsoft.AspNet.SignalR.Client;
using System.Security;
using System.Net;

namespace HelpDesk.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        //MTObservableCollection<string> computers;
        ObservableCollection<string> Printers;
        
        //ObservableCollection<Principal> txtUsersNames;
        
        //ObservableCollection<Button> AllComputersButtons;

        //private ComputerCommands _ComputerCommandsviewModel;
        // private UserCommands _UserCommandsviewModel;
        private PrinterCommands _PrinterCommandsviewModel;
        //private ObservableCollection<RemoteSoftware> listRemoteSoftware;

        const string ServerURI = "http://*:8080";

        public IDisposable SignalR { get; set; }
        public HubConnection ConnectionHub { get; set; }
        public IHubProxy HubProxy { get; set; }

        XMLApi xml = new XMLApi();

        List<Principal> AllComputers;
        public MainWindowViewModel()
        {

            //computers = new MTObservableCollection<string>();
            //AllComputersButtons = new ObservableCollection<Button>();

            //MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();

            //_ComputerCommandsviewModel = new ComputerCommands();
            //_UserCommandsviewModel = new UserCommands();

            _PrinterCommandsviewModel = new PrinterCommands();

            
            Load();
            // StartSignalR();

        }

        #region Subscribe
        protected override void Subscribe()
        {
            //MessageBus.Subscribe<ActiveDirectoryObjectPublish>(ActiveDirectoryObjectSelected);
            MessageBus.Subscribe<ActiveDirectorySave>(ActiveDirectorySaveEvent);
          
            MessageBus.Subscribe<Refresh>((obj) =>
            {
                Load();
            });
           
            MessageBus.Subscribe<PublishHubConnection>(HubConnectionSet);

        }


        protected override void Unsubscribe()
        {
        }

        
        private void ActiveDirectory_OnCheckActiveDirectoryObject(bool IsCheck, ActiveDirectoryObject ado)
        {
            switch (ado)
            {
                case ActiveDirectoryObject.Computers:
                    //SetActiveDirectoryObjectComputers(IsCheck);
                    break;
                case ActiveDirectoryObject.Printers:
                    SetActiveDirectoryObjectPrinters(IsCheck);
                    break;
                default:
                    break;
            }
        }
        private void ActiveDirectorySaveEvent(ActiveDirectorySave obj)
        {
            Load();
        }
        #endregion

        #region Load
        private async void Load()
        {
            #region Old

            //await Task.WhenAll(StartInitialzeAutoComplets(), GetAllRemoteSoftware());

            #endregion

            if (string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
            {
                App.XmlConfigurationFileLocation = Properties.Settings.Default.XmlConfigurationFileLocation;
            }

            
            Task TPrinters = Task.Run(() =>
            {
                SetActiveDirectoryObjectPrinters(Properties.Settings.Default.CheckPrinters);
            });

            //GetAllRemoteSoftware();

            Task x = await Task.WhenAny(TPrinters);
            Debug.WriteLine(x.Status);
            LoadComputerSyntax();



        }

        private void StartSignalR()
        {
            try
            {
                SignalR = WebApp.Start<Startup>(ServerURI);
                Debug.WriteLine("SignalR Is running");
                SignalRRunning = "SignalR Is running: " + ServerURI;
            }
            catch (TargetInvocationException)
            {
                Debug.WriteLine("A server is already running at " + ServerURI);
                //this.Dispatcher.Invoke(() => ButtonStart.IsEnabled = true);
            }
        }

        private string signalR;

        public string SignalRRunning
        {
            get { return signalR; }
            set { signalR = value; OnPropertyChanged(); }
        }

        private async void HubConnectionSet(PublishHubConnection obj)
        {
            var querystringData = new Dictionary<string, string>();
            querystringData.Add("version", "version 1.0");

            ConnectionHub = new HubConnection("http://ILQHFAATC1DT703:8080", querystringData);
            //Connection = new HubConnection(ServerURI);
            //Connection.Closed += Connection_Closed;
            HubProxy = ConnectionHub.CreateHubProxy("MyHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string>("AddMessage",
                (name, message) =>
                {
                    Debug.WriteLine(message);
                });
            try
            {
                await ConnectionHub.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to connect to server: Start server before connecting clients. " + ex.Message);
                //No connection: Don't enable Send button or show chat UI
            }

            //HubProxy.Invoke("Send", SelectedComputer, "itamartz");

            if (App.RunProcess != null)
            {

                //Run On remote Server
                //if (!App.AdminCredentialsAndRemoteSoftware.IsRunning)
                //App.AdminCredentialsAndRemoteSoftware.IsRunning = true;

                await HubProxy.Invoke("RunRemoteSoftware", App.AdminCredentialsAndRemoteSoftware);
                Debug.WriteLine("Invoke RunRemoteSoftware");
                try
                {
                    // close local RunProcess
                    App.RunProcess.Kill();
                    App.RunProcess = null;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException("close local RunProcess", ex);
                }
            }

            //Close SignalR

            try
            {
                ConnectionHub.Stop();
                ConnectionHub.Dispose();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("HubConnectionSet", ex);
            }
        }

        private void Connection_Closed()
        {
            Debug.WriteLine("Connection Close");
        }

        /// <summary>
        /// Create Button from XML
        /// </summary>
        /// <returns></returns>
        //private async void GetAllRemoteSoftware()
        //{
        //    AllComputersButtons.Clear();
        //    listRemoteSoftware = new ObservableCollection<RemoteSoftware>(await xml.GetXMLButtons());
        //    //stack1.Children.Clear();

        //    foreach (RemoteSoftware item in listRemoteSoftware)
        //    {
        //        Button b = new Button();
        //        b.Content = item.Name;
        //        b.Height = 28;
        //        b.Width = 75;

        //        if (item.Default)
        //        {
        //            b.FontWeight = FontWeights.Bold;
        //        }
        //        //b.Width = 75;
        //        b.Margin = new Thickness(0, 5, 5, 5);
        //        b.Click += b_Click;
        //        b.Tag = item;
        //        //stack1.Children.Add(b);
        //        AllComputersButtons.Add(b);
        //    }

        //}

        //void b_Click(object sender, RoutedEventArgs e)
        //{
        //    Button b = sender as Button;
        //    RemoteSoftware sof = b.Tag as RemoteSoftware;
        //    _ComputerCommandsviewModel.RunRemoteSoftware(sof, _selectedComputer);
        //}

        private void LoadComputerSyntax()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.txt_ComputerSyntax))
            {
                Properties.Settings.Default.txt_ComputerSyntax = "{computerName}";
            }

        }

        private string _selectedComputer;
        public string SelectedComputer
        {
            get { return _selectedComputer; }
            set
            {
                _selectedComputer = value;
                OnPropertyChanged();
                MessageBus.Publish<ComputerSelected>(new ComputerSelected() { ComputerName = value });
            }
        }

        string txtComputerCount;
        public string TxtComputerCount
        {
            get { return txtComputerCount; }
            set { txtComputerCount = value; OnPropertyChanged(); }
        }
      
        //private async void SetActiveDirectoryObjectComputers(bool IsCheck)
        //{
        //    Debug.WriteLine("Start SetActiveDirectoryObjectComputers");
        //    //Debug.WriteLine(Environment.StackTrace);
        //    ComputerCircularProgressBar = IsCheck;
        //    Computers.Clear();
        //    Debug.WriteLine("Computers.Clear()");

        //    if (IsCheck)
        //    {
        //        TxtComputerCount = null;
        //        AllComputers = await _ComputerCommandsviewModel.GetAllComputers();
        //        Debug.WriteLine(AllComputers.Count);
        //        AllComputers = AllComputers.OrderBy(a => a.SamAccountName).ToList();

        //        foreach (Principal item in AllComputers)
        //        {
        //            computers.Add(item.Name);
        //        }
        //    }
        //    else
        //    {
        //        TxtComputerCount = null;
        //        Computers.Clear();
        //    }

        //    TxtComputerCount = Computers.Count.ToString();
        //    Debug.WriteLine("End SetActiveDirectoryObjectComputers");

        //}
        
        private async void SetActiveDirectoryObjectPrinters(bool IsCheck)
        {
            if (IsCheck)
            {
                List<string> _AllPrinters = await _PrinterCommandsviewModel.GetAllPrinters();
                _AllPrinters.Sort();
                AllPrinters = new ObservableCollection<string>(_AllPrinters);
                PrinterCount = AllPrinters.Count;
            }
            else
            {
                AllPrinters = null;
                PrinterCount = 0;
            }
        }
      
        //private void SetComputersPrincipal(string prefix)
        //{
        //    if (string.IsNullOrEmpty(prefix))
        //    {


        //        foreach (Principal item in AllComputers)
        //        {
        //            Computers.Add(item.Name);
        //        }
        //    }
        //    else
        //    {
        //        foreach (Principal item in AllComputers)
        //        {
        //            try
        //            {
        //                string removeprefix = item.Name.ToLower().Replace(prefix.ToLower(), "");
        //                Computers.Add(removeprefix);
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Windows.Forms.MessageBox.Show(ex.Message);
        //            }
        //        }
        //    }


        //}

        private int printerCount;

        public int PrinterCount
        {
            get { return printerCount; }
            set
            {
                printerCount = value;
                OnPropertyChanged();
            }
        }

        #endregion

      

        #region WindowsCommand

        private string version;

        public string PublishVersion
        {
            get
            {
                Version obj = GetRunningVersion();
                version = string.Format("HelpDesk | Version {0}.{1}.{2}.{3}", obj.Major, obj.Minor, obj.Build, obj.Revision);
                return version;
            }
            set { version = value; }
        }

        public Version GetPublishedVersion()
        {


            XmlDocument xmlDoc = new XmlDocument();
            Assembly asmCurrent = System.Reflection.Assembly.GetExecutingAssembly();
            string executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

            xmlDoc.Load(executePath + ".manifest");
            string retval = string.Empty;
            if (xmlDoc.HasChildNodes)
            {
                retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value.ToString();
            }
            return new Version(retval);
        }
        private Version GetRunningVersion()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        #endregion

     

        #region ObservableCollectionBinding
        //public MTObservableCollection<string> Computers
        //{
        //    get { return computers; }
        //    set
        //    {
        //        computers = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public ObservableCollection<Principal> TxtUsersNames
        //{
        //    get { return txtUsersNames; }
        //    set
        //    {
        //        txtUsersNames = value;
        //        OnPropertyChanged();
        //    }
        //}
        public ObservableCollection<string> AllPrinters
        {
            get { return Printers; }
            set
            {
                Printers = value;
                OnPropertyChanged();
            }

        }
       
        #endregion

        public bool ComputerCircularProgressBar { get; set; }
       

    }
}
