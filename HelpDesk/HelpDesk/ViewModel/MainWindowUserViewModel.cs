using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.ViewModel
{
    public class MainWindowUserViewModel : BaseViewModel
    {
        XMLApi xml = new XMLApi();
        UserCommands _userCommand;
        private ObservableCollection<Principal> txtUsersNames;
        public MainWindowUserViewModel()
        {
            txtUsersNames = new ObservableCollection<Principal>();

            _userCommand = new UserCommands();
            MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();


            Load();
        }

        private async void Load()
        {
            List<RemoteSoftware> listRemoteSoftware = await xml.GetUsersRemoteSoftwares();
            Task TUsers = Task.Run(() =>
            {
                SetActiveDirectoryObjectUsers(Properties.Settings.Default.CheckUsers);
            });

        }

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

        #region Subscribe / Unsubscribe
        protected override void Subscribe()
        {
            MessageBus.Subscribe<ActiveDirectoryUserPublish>((obj) =>
                {
                    SetActiveDirectoryObjectUsers(obj.Ischeck);
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
        #endregion

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
    }
}
