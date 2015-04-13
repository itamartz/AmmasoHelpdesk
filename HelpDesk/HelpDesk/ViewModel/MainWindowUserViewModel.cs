using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HelpDesk.ViewModel
{
    public class MainWindowUserViewModel : BaseViewModel
    {
        XMLApi xml = new XMLApi();
        UserCommands _userCommand;
        private ObservableCollection<Principal> txtUsersNames;
        private MTObservableCollection<Button> AllComputersButtons;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private string UsersRemoteSoftwares;

        public MainWindowUserViewModel()
        {
            //MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();

            txtUsersNames = new ObservableCollection<Principal>();
            AllComputersButtons = new MTObservableCollection<Button>();

            _userCommand = new UserCommands();
            Load();
        }

        #region Load
        private async void Load()
        {
            Task TUsers = Task.Run(() =>
            {
                SetActiveDirectoryObjectUsers(Properties.Settings.Default.CheckUsers);
            });
            SetRemoteSoftware();

            await Task.WhenAny(TUsers);
        }

        //private void WatcherUserXML()
        //{
        //    if (!string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
        //    {
        //        UsersRemoteSoftwares = Path.Combine(App.XmlConfigurationFileLocation, "UsersRemoteSoftwares.xml");
        //        fileSystemWatcher = new System.IO.FileSystemWatcher();
        //        fileSystemWatcher.Path = App.XmlConfigurationFileLocation;
        //        fileSystemWatcher.Filter = "UsersRemoteSoftwares.xml";
        //        fileSystemWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size | NotifyFilters.LastAccess;
        //        fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(UsersRemoteSoftwaresChange);
        //        fileSystemWatcher.EnableRaisingEvents = true;
        //    }
        //}

        private async void SetActiveDirectoryObjectUsers(bool Ischeck)
        {
            if (Ischeck)
            {
                TxtUsersNamesCount = null;
                List<Principal> listUsers = await _userCommand.GetAllUsersFromAD();
                listUsers = listUsers.OrderBy(p => p.SamAccountName).ToList();
                TxtUsersNames = new ObservableCollection<Principal>(listUsers);
                TxtUsersNamesCount = TxtUsersNames.Count.ToString();
            }
            else
            {
                TxtUsersNamesCount = "0";
            }
        }

        private async void SetRemoteSoftware()
        {
            List<RemoteSoftware> listRemoteSoftware = await xml.GetUsersRemoteSoftwares();
            AllComputersButtons.Clear();

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

        private void b_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button b = sender as Button;
            RemoteSoftware sof = b.Tag as RemoteSoftware;
            _userCommand.RunRemoteSoftware(sof, SelectedComputer);
        }
        #endregion

        #region Subscribe / Unsubscribe
        protected override void Subscribe()
        {
            MessageBus.Subscribe<ActiveDirectoryUserPublish>((obj) =>
                {
                    SetActiveDirectoryObjectUsers(obj.Ischeck);
                });

            MessageBus.Subscribe<ComputerSelected>((obj) =>
                {
                    SelectedComputer = obj.ComputerName;
                });
            MessageBus.Subscribe<UsersViewViewModelSave>(
                (obj) =>
                {
                    SetRemoteSoftware();
                });
        }
        protected override void Unsubscribe()
        {
        }
        #endregion

        #region ObservableCollections
        public ObservableCollection<Principal> TxtUsersNames
        {
            get { return txtUsersNames; }
            set
            {
                txtUsersNames = value;
                OnPropertyChanged();
            }
        }
        public MTObservableCollection<Button> RemoteSoftwares
        {
            get
            {
                return AllComputersButtons;
            }
            set
            {
                AllComputersButtons = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Properties full

        private string _selectedComputer;
        public string SelectedComputer
        {
            get { return _selectedComputer; }
            set
            {
                _selectedComputer = value;
                OnPropertyChanged();
            }
        }

        UserPrincipal selectedItemUser;
        public UserPrincipal SelectedItemUser
        {
            get { return selectedItemUser; }
            set
            {
                selectedItemUser = value;
                try
                {
                    IsAccountLockedOut = selectedItemUser.IsAccountLockedOut();
                }
                finally
                {
                    OnPropertyChanged("IsAccountLockedOut");
                }
            }
        }

        bool isAccountLockedOut;
        public bool IsAccountLockedOut
        {
            get { return isAccountLockedOut; }
            set
            {
                isAccountLockedOut = value;
                Debug.WriteLine(isAccountLockedOut);
                OnPropertyChanged();
            }
        }

        string txtUsersNamesCount;
        public string TxtUsersNamesCount
        {
            get { return txtUsersNamesCount; }
            set { txtUsersNamesCount = value; OnPropertyChanged(); }
        }
        #endregion

    }
}
