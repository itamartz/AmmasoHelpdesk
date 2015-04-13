using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HelpDesk.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        MTObservableCollection<string> computers;
        ObservableCollection<string> Printers;
        ObservableCollection<Principal> txtUsersNames;
        ObservableCollection<Button> AllComputersButtons;

        private ComputerCommands _ComputerCommandsviewModel;
        private UserCommands _UserCommandsviewModel;
        private PrinterCommands _PrinterCommandsviewModel;
        private ObservableCollection<RemoteSoftware> listRemoteSoftware;



        XMLApi xml = new XMLApi();

        List<Principal> AllComputers;
        public MainWindowViewModel()
        {
            computers = new MTObservableCollection<string>();
            AllComputersButtons = new ObservableCollection<Button>();
            
            //MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();

            _ComputerCommandsviewModel = new ComputerCommands();
            _UserCommandsviewModel = new UserCommands();
            _PrinterCommandsviewModel = new PrinterCommands();

            Restart = new RelayCommand<object>(DoRestart, CanRestart);
            Shutdown = new RelayCommand<object>(DoShutdown, CanShutdown);
            ActiveDirectoryDSA = new RelayCommand<object>(DoActiveDirectory, CanActiveDirectory);
            GroupPolicyManagement = new RelayCommand<object>(DoGroupPolicyManagement, CanGroupPolicyManagement);
            OpenPort = new RelayCommand<object>(DoOpenPort, CanOpenPort);

            Load();
        }

        #region Subscribe
        protected override void Subscribe()
        {
            MessageBus.Subscribe<ActiveDirectoryObjectPublish>(ActiveDirectoryObjectSelected);
            MessageBus.Subscribe<ActiveDirectorySave>(ActiveDirectorySaveEvent);
            MessageBus.Subscribe<DefaultProgram>(StartDefaultProgram);
            
        }
        protected override void Unsubscribe()
        {
            MessageBus.Unsubscribe<ActiveDirectoryObjectPublish>(ActiveDirectoryObjectSelected);
        }

        private void ActiveDirectoryObjectSelected(ActiveDirectoryObjectPublish obj)
        {
            ActiveDirectory_OnCheckActiveDirectoryObject(obj.Ischeck, obj.ActiveDirectoryObject);
        }
        private void ActiveDirectory_OnCheckActiveDirectoryObject(bool IsCheck, ActiveDirectoryObject ado)
        {
            switch (ado)
            {
                case ActiveDirectoryObject.Computers:
                    SetActiveDirectoryObjectComputers(IsCheck);
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

            Task TComputers = Task.Run(() =>
            {
                SetActiveDirectoryObjectComputers(Properties.Settings.Default.CheckComputers);
            });
            //Task TUsers = Task.Run(() =>
            //{
            //    SetActiveDirectoryObjectUsers(Properties.Settings.Default.CheckUsers);
            //});
            Task TPrinters = Task.Run(() =>
            {
                SetActiveDirectoryObjectPrinters(Properties.Settings.Default.CheckPrinters);
            });
            //Task TRemoteSoftware = Task.Run(() =>
            //{
            //    GetAllRemoteSoftware();
            //});
            GetAllRemoteSoftware();

            Task x = await Task.WhenAny(TComputers, TPrinters);
            Debug.WriteLine(x.Status);
            LoadComputerSyntax();

        }

        /// <summary>
        /// Create Button from XML
        /// </summary>
        /// <returns></returns>
        private async void GetAllRemoteSoftware()
        {
            AllComputersButtons.Clear();
            listRemoteSoftware = new ObservableCollection<RemoteSoftware>(await xml.GetXMLButtons());
            //stack1.Children.Clear();

            foreach (RemoteSoftware item in listRemoteSoftware)
            {
                Button b = new Button();
                b.Content = item.Name;
                b.Height = 28;
                b.Width = 75;

                if (item.Default)
                {
                    b.FontWeight = FontWeights.Bold;
                }
                //b.Width = 75;
                b.Margin = new Thickness(0, 5, 5, 5);
                b.Click += b_Click;
                b.Tag = item;
                //stack1.Children.Add(b);
                AllComputersButtons.Add(b);
            }

        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            RemoteSoftware sof = b.Tag as RemoteSoftware;
            _ComputerCommandsviewModel.RunRemoteSoftware(sof, _selectedComputer);
        }

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
        private async void SetActiveDirectoryObjectComputers(bool IsCheck)
        {
            Debug.WriteLine("Start SetActiveDirectoryObjectComputers");
            //Debug.WriteLine(Environment.StackTrace);
            ComputerCircularProgressBar = IsCheck;
            Computers.Clear();
            Debug.WriteLine("Computers.Clear()");

            if (IsCheck)
            {
                TxtComputerCount = null;
                AllComputers = await _ComputerCommandsviewModel.GetAllComputers();
                Debug.WriteLine(AllComputers.Count);
                AllComputers = AllComputers.OrderBy(a => a.SamAccountName).ToList();

                foreach (Principal item in AllComputers)
                {
                    computers.Add(item.Name);
                }
            }
            else
            {
                TxtComputerCount = null;
                Computers.Clear();
            }

            TxtComputerCount = Computers.Count.ToString();
            Debug.WriteLine("End SetActiveDirectoryObjectComputers");

        }
        
        private async void SetActiveDirectoryObjectUsers(bool IsCheck)
        {
            Debug.WriteLine("Start SetActiveDirectoryObjectUsers");
            if (IsCheck)
            {
                //TxtUsersNamesCount = null;
                List<Principal> AllUsers = await _UserCommandsviewModel.GetAllUsersFromAD();
                AllUsers = AllUsers.OrderBy(a => a.SamAccountName).ToList();
                TxtUsersNames = new ObservableCollection<Principal>(AllUsers);
            }
            else
            {
                if (TxtUsersNames != null)
                {
                    TxtUsersNames.Clear();
                }
            }
            if (TxtUsersNames != null) { }
            //TxtUsersNamesCount = TxtUsersNames.Count.ToString();
            else
            {
            }
            //TxtUsersNamesCount = "0";
            Debug.WriteLine("End SetActiveDirectoryObjectUsers");

        }
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
        private void SetComputersPrincipal(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {


                foreach (Principal item in AllComputers)
                {
                    Computers.Add(item.Name);
                }
            }
            else
            {
                foreach (Principal item in AllComputers)
                {
                    try
                    {
                        string removeprefix = item.Name.ToLower().Replace(prefix.ToLower(), "");
                        Computers.Add(removeprefix);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
            }


        }

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

        #region ICommands

        #region Restart
        private bool CanRestart(object obj)
        {
            return (obj != null);
        }
        public ICommand Restart { get; set; }

        private void DoRestart(object obj)
        {
            MessageBus.Publish<ComputerRestart>(new ComputerRestart() { ComputerName = SelectedComputer });
        }
        #endregion

        #region Shutdown
        public ICommand Shutdown { get; set; }
        private bool CanShutdown(object obj)
        {
            return (obj != null);
        }
        private void DoShutdown(object obj)
        {
            MessageBus.Publish<ComputerShutdown>(new ComputerShutdown() { ComputerName = SelectedComputer });
        }
        #endregion

        #region DSA
        public ICommand ActiveDirectoryDSA { get; set; }
        private bool CanActiveDirectory(object obj)
        {
            return File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\dsa.msc");
        }
        private void DoActiveDirectory(object obj)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\dsa.msc");
        }
        #endregion

        #region GPO
        public ICommand GroupPolicyManagement { get; set; }
        private bool CanGroupPolicyManagement(object obj)
        {
            return File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\gpmc.msc");
        }
        private void DoGroupPolicyManagement(object obj)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\gpmc.msc");
        }
        #endregion

        #region OpenPort
        public ICommand OpenPort { get; set; }
        private bool CanOpenPort(object obj)
        {
            bool b = (obj != null);
            if (b)
            {
                string str = (string)obj;
                return !string.IsNullOrEmpty(str);
            }
            else
                return b;
        }
        private async void DoOpenPort(object obj)
        {
            string txtPortNumber = (string)obj;
            int port = 0;
            Task T = Task.Run(() =>
            {
                if (int.TryParse(txtPortNumber, out port))
                {
                    if (CheckPort(SelectedComputer, port, 500))
                    {
                        MessageBox.Show("Port " + port + " Open");
                    }
                    else
                    {
                        MessageBox.Show("Port " + port + " Closed");
                    }
                }
            });
            await Task.WhenAny(T);
        }
        bool CheckPort(string strIpAddress, int intPort, int nTimeoutMsec)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);


                IAsyncResult result = socket.BeginConnect(strIpAddress, intPort, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(nTimeoutMsec, true);

                return socket.Connected;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (null != socket)
                    socket.Close();
            }
        }

        #endregion

        #endregion

        #region ObservableCollectionBinding
        public MTObservableCollection<string> Computers
        {
            get { return computers; }
            set
            {
                computers = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Principal> TxtUsersNames
        {
            get { return txtUsersNames; }
            set
            {
                txtUsersNames = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> AllPrinters
        {
            get { return Printers; }
            set
            {
                Printers = value;
                OnPropertyChanged();
            }

        }
        public ObservableCollection<Button> AllComputerSoftware
        {
            get { return AllComputersButtons; }
            set
            {
                AllComputersButtons = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public bool ComputerCircularProgressBar { get; set; }
        public void StartDefaultProgram(DefaultProgram obj)
        {
            if (listRemoteSoftware != null && listRemoteSoftware.Count != 0)
            {
                RemoteSoftware rem = listRemoteSoftware.Where(r => r.Default == true).FirstOrDefault();
                _ComputerCommandsviewModel.RunRemoteSoftware(rem, SelectedComputer);
            }
        }


    }
}
