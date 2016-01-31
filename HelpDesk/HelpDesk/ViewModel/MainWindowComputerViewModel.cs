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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;


namespace HelpDesk.ViewModel
{
    public class MainWindowComputerViewModel : BaseViewModel
    {
        private MTObservableCollection<string> computers;
        private ObservableCollection<Button> AllComputersButtons;
        private ObservableCollection<RemoteSoftware> listRemoteSoftware;

        private ComputerCommands _ComputerCommandsviewModel;

        List<Principal> AllComputers;
        XMLApi xml = new XMLApi();

        /// <summary>
        /// Constracture
        /// </summary>
        public MainWindowComputerViewModel()
        {
            computers = new MTObservableCollection<string>();
            AllComputersButtons = new ObservableCollection<Button>();

            _ComputerCommandsviewModel = new ComputerCommands();

            ICommandSets();
            load();
        }

        private void ICommandSets()
        {
            RDPCommand = new RelayCommand<object>(DoRDPCommand, CanRDPCommand);
            CDriveCommand = new RelayCommand<object>(DoCDriveCommand, CanCDriveCommand);
            DDriveCommand = new RelayCommand<object>(DoDDriveCommand, CanDDriveCommand);
            PingCommand = new RelayCommand<object>(DoPingCommand, CanPingCommand);
            ManageCommand = new RelayCommand<object>(DoManageCommand, CanManageCommand);

            Restart = new RelayCommand<object>(DoRestart, CanRestart);
            Shutdown = new RelayCommand<object>(DoShutdown, CanShutdown);
            ActiveDirectoryDSA = new RelayCommand<object>(DoActiveDirectory, CanActiveDirectory);
            GroupPolicyManagement = new RelayCommand<object>(DoGroupPolicyManagement, CanGroupPolicyManagement);
            OpenPort = new RelayCommand<object>(DoOpenPort, CanOpenPort);

        }

        #region Load
        private void load()
        {
            Task.Run(() =>
            {
                SetActiveDirectoryObjectComputers(Properties.Settings.Default.CheckComputers);
            });

            GetAllRemoteSoftware();
        }
        private async void GetAllRemoteSoftware()
        {
            AllComputersButtons.Clear();
            listRemoteSoftware = new ObservableCollection<RemoteSoftware>(await xml.GetXMLButtons());
            //stack1.Children.Clear();

            foreach (RemoteSoftware item in listRemoteSoftware)
            {
                Button b = new Button();

                try
                {
                    GetIcon(item.ProgramPath);

                }
                catch (Exception)
                {
                    
                }
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

        #endregion

        #region Subscribe / Unsubscribe
        protected override void Subscribe()
        {
            MessageBus.Subscribe<ActiveDirectoryObjectPublish>((obj) =>
            {
                SetActiveDirectoryObjectComputers(obj.Ischeck);
            });
            MessageBus.Subscribe<WindowActivated>((arg) =>
            {
                UserShouldEditValueNow = arg.Activated;
            });
            MessageBus.Subscribe<RDPProgram>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerRDP(_selectedComputer);
            });
            MessageBus.Subscribe<CDrive>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerCDrive(_selectedComputer);
            });
            MessageBus.Subscribe<DDrive>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerDDrive(_selectedComputer);
            });
            MessageBus.Subscribe<PingProgram>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerPing(_selectedComputer);
            });
            MessageBus.Subscribe<ManageProgram>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerManage(_selectedComputer);
            });
            MessageBus.Subscribe<DefaultProgram>((obj) =>
            {
                if (listRemoteSoftware != null && listRemoteSoftware.Count != 0)
                {
                    RemoteSoftware rem = listRemoteSoftware.Where(r => r.Default == true).FirstOrDefault();
                    _ComputerCommandsviewModel.RunRemoteSoftware(rem, SelectedComputer);
                }
            });

            MessageBus.Subscribe<ComputerShutdown>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerShutdown(_selectedComputer);
            });
            MessageBus.Subscribe<ComputerRestart>((obj) =>
            {
                if (!string.IsNullOrEmpty(_selectedComputer))
                    _ComputerCommandsviewModel.ComputerRestart(_selectedComputer);
            });
        }
        protected override void Unsubscribe()
        {
            MessageBus.Unsubscribe<ActiveDirectoryObjectPublish>((obj) =>
                {
                    SetActiveDirectoryObjectComputers(obj.Ischeck);
                });
        }

        #endregion

        #region Binding

        #region UserShouldEditValueNow
        private bool _UserShouldEditValueNow;

        public bool UserShouldEditValueNow
        {
            get
            {
                return _UserShouldEditValueNow;
            }
            set
            {
                _UserShouldEditValueNow = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedComputer
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

        #endregion

        #region Computers
        public MTObservableCollection<string> Computers
        {
            get { return computers; }
            set
            {
                computers = value;
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

        public bool ComputerCircularProgressBar { get; set; }

        string txtComputerCount;
        public string TxtComputerCount
        {
            get { return txtComputerCount; }
            set { txtComputerCount = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #region ICommands

        #region RDP
        public ICommand RDPCommand { get; set; }

        private bool CanRDPCommand(object obj)
        {
            return true;
        }

        private void DoRDPCommand(object obj)
        {
            MessageBus.Publish<RDPProgram>(new RDPProgram());
        }
        #endregion

        #region C$
        public ICommand CDriveCommand { get; set; }

        private bool CanCDriveCommand(object obj)
        {
            return true;
        }

        private void DoCDriveCommand(object obj)
        {
            MessageBus.Publish<CDrive>(new CDrive());
        }
        #endregion

        #region D$
        public ICommand DDriveCommand { get; set; }

        private bool CanDDriveCommand(object obj)
        {
            return true;
        }

        private void DoDDriveCommand(object obj)
        {
            MessageBus.Publish<DDrive>(new DDrive());
        }
        #endregion

        #region Ping
        public ICommand PingCommand { get; set; }

        private bool CanPingCommand(object obj)
        {
            return true;
        }

        private void DoPingCommand(object obj)
        {
            MessageBus.Publish<PingProgram>(new PingProgram());
        }
        #endregion

        #region Manage
        public ICommand ManageCommand { get; set; }

        private bool CanManageCommand(object obj)
        {
            return true;
        }

        private void DoManageCommand(object obj)
        {
            MessageBus.Publish<ManageProgram>(new ManageProgram());
        }
        #endregion

        #region Restart
        public ICommand Restart { get; set; }

        private bool CanRestart(object obj)
        {
            return (obj != null);
        }

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

        public static ImageSource GetIcon(string fileName)
        {
            Icon icon = Icon.ExtractAssociatedIcon(fileName);
            //return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new Int32Rect(icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
            BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions());
            return source;
        }

    }
}
