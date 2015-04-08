using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelpDesk.ViewModel
{
    public class ActiveDirectoryViewModel : BaseViewModel
    {
        XMLApi xml = new XMLApi();
        ObservableCollection<DistinguishedNames> obDistinguishedNames;


        public ActiveDirectoryViewModel()
        {
            MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();

            Load();

            AddDistinguishedNames = new RelayCommand<object>(AddDistinguishedNamesToObs, canAddDistinguishedNamesToObs);
            RemoveDistinguishedNames = new RelayCommand<object>(RemoveDistinguishedNamesToObs, canRemoveDistinguishedNamesToObs);

            ActiveDirectoryComputers = new RelayCommand<object>(DoActiveDirectoryComputers, canActiveDirectoryComputers);
            ActiveDirectoryUsers = new RelayCommand<object>(DoActiveDirectoryUsers, canActiveDirectoryUsers);
            ActiveDirectoryPrinters = new RelayCommand<object>(DoActiveDirectoryPrinters, canActiveDirectoryPrinters);
        }

        #region AddDistinguishedNames
        public ICommand AddDistinguishedNames { get; set; }
        private bool canAddDistinguishedNamesToObs(object obj)
        {
            return true;
        }

        private void AddDistinguishedNamesToObs(object obj)
        {
            if (ObDistinguishedNames == null)
            {
                ObDistinguishedNames = new ObservableCollection<DistinguishedNames>();
            }
            ObDistinguishedNames.Add(new DistinguishedNames() { Index = ObDistinguishedNames.Count });
        }
        #endregion

        #region RemoveDistinguishedNames
        public ICommand RemoveDistinguishedNames { get; set; }
        private void RemoveDistinguishedNamesToObs(object obj)
        {
            DistinguishedNames dn = obj as DistinguishedNames;

            if (dn != null)
            {
                ObDistinguishedNames.RemoveAt(dn.Index);
            }

            for (int i = 0; i < ObDistinguishedNames.Count; i++)
            {
                ObDistinguishedNames[i].Index = i;
            }
        }

        private bool canRemoveDistinguishedNamesToObs(object obj)
        {
            return ObDistinguishedNames.Count != 1;
        }
        #endregion

        #region ICommands

        #region ActiveDirectoryComputers
        public ICommand ActiveDirectoryComputers { get; set; }
        private void DoActiveDirectoryComputers(object obj)
        {

            CheckBox computerCheckBox = obj as CheckBox;
            ActiveDirectoryObjectPublish adop = new ActiveDirectoryObjectPublish();
            adop.ActiveDirectoryObject = ActiveDirectoryObject.Computers;
            adop.Ischeck = (bool)computerCheckBox.IsChecked;

            if (adop != null)
                PublishMessage<ActiveDirectoryObjectPublish>(adop);
        }
        private bool canActiveDirectoryComputers(object obj)
        {
            if (ObDistinguishedNames != null)
            {
                if (ObDistinguishedNames.Any(d => !string.IsNullOrEmpty(d.Computers)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        #endregion

        #region ActiveDirectoryUsers
        public ICommand ActiveDirectoryUsers { get; set; }
        private void DoActiveDirectoryUsers(object obj)
        {

            CheckBox UserCheckBox = obj as CheckBox;
            ActiveDirectoryUserPublish adop = new ActiveDirectoryUserPublish();
            adop.Ischeck = (bool)UserCheckBox.IsChecked;

            if (adop != null)
                PublishMessage<ActiveDirectoryUserPublish>(adop);
        }
        private bool canActiveDirectoryUsers(object obj)
        {
            
            if (ObDistinguishedNames != null)
            {
                if (ObDistinguishedNames.Any(d => !string.IsNullOrEmpty(d.Users)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        #endregion

        #region ActiveDirectoryPrinters
        public ICommand ActiveDirectoryPrinters { get; set; }
        private void DoActiveDirectoryPrinters(object obj)
        {

            CheckBox PrintersCheckBox = obj as CheckBox;
            ActiveDirectoryObjectPublish adop = new ActiveDirectoryObjectPublish();
            adop.ActiveDirectoryObject = ActiveDirectoryObject.Printers;
            adop.Ischeck = (bool)PrintersCheckBox.IsChecked;

            if (adop != null)
                PublishMessage<ActiveDirectoryObjectPublish>(adop);
        }
        private bool canActiveDirectoryPrinters(object obj)
        {
            if (ObDistinguishedNames != null)
            {
                if (ObDistinguishedNames.Any(d => !string.IsNullOrEmpty(d.Printers)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        #endregion

        #endregion
        private async void Load()
        {
            List<DistinguishedNames> listDistinguishedNames = await xml.GetDistinguishedNames();
            ObDistinguishedNames = new ObservableCollection<DistinguishedNames>(listDistinguishedNames);

            Debug.WriteLine(listDistinguishedNames.Count);
        }
        public ObservableCollection<DistinguishedNames> ObDistinguishedNames
        {
            get { return obDistinguishedNames; }
            set { obDistinguishedNames = value; OnPropertyChanged(); }
        }

        string userPassword;

        public string UserPassword
        {
            get
            {
                userPassword = Utility.AdminPassword;   //Properties.Settings.Default.txt_AdminPassword;
                return userPassword;
            }
            set
            {
                userPassword = value;
                Utility.AdminPassword = value;
                OnPropertyChanged();
            }
        }

        internal void Save(string pas)
        {
            UserPassword = pas;
            xml.Save(ObDistinguishedNames);
            //PublishMessage<ActiveDirectorySave>(new ActiveDirectorySave());
        }
    }

}
