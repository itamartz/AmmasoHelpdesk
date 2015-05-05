using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HelpDesk.Commands;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;

namespace HelpDesk.ViewModel
{
    public class UsersViewViewModel : BaseViewModel
    {
        XMLApi xml = new XMLApi();

        private MTObservableCollection<RemoteSoftware> obRemoteSoftware;

        List<RemoteSoftware> listRemoteSoftware;

        public UsersViewViewModel()
        {
            obRemoteSoftware = new MTObservableCollection<RemoteSoftware>();
            UsersRemoteSoftware = new MTObservableCollection<RemoteSoftware>();
            Brows = new RelayCommand<object>(DoBrows, CanBrows);
            Delete = new RelayCommand<object>(DoDelete, CanDelete);
            Add = new RelayCommand<object>(DoAdd, CanAdd);

            MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();

            Load();
        }
       

        #region BaseViewModel Subscribe / Unsubscribe
        protected override void Subscribe()
        {
        }
        protected override void Unsubscribe()
        {
        }
        #endregion

        private async void Load()
        {
            listRemoteSoftware = await xml.GetUsersRemoteSoftwares();
            foreach (RemoteSoftware item in listRemoteSoftware)
            {
                UsersRemoteSoftware.Add(item);
            }

        }


        #region ICommands

        #region Brows
        public ICommand Brows { get; set; }
        private bool CanBrows(object obj)
        {
            return true;
        }

        private void DoBrows(object obj)
        {
            if (obj != null)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                Button b = obj as Button;
                RemoteSoftware re = (b.Tag as RemoteSoftware);
                dlg.Title = string.Format("Brows for {0} exe", re.Name);
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    re.ProgramPath = dlg.FileName;
                }
            }
        }
        #endregion

        #region Add
        public ICommand Add { get; set; }
        private bool CanAdd(object obj)
        {
            return true;
        }
        private void DoAdd(object obj)
        {
            UsersRemoteSoftware.Add(new RemoteSoftware() { Name = "Default name " + (UsersRemoteSoftware.Count + 1) });
        }
        #endregion

        #region Delete
        public ICommand Delete { get; set; }
        private bool CanDelete(object obj)
        {
            return (obRemoteSoftware.Count - 1 != 0);
        }

        private void DoDelete(object obj)
        {
            if (obj != null)
            {

                Button b = (obj as System.Windows.Controls.Button);
                if (obRemoteSoftware.Count - 1 != 0)
                {
                    RemoteSoftware sof = (b.Tag as RemoteSoftware);
                    sof.Isremove = true;
                    obRemoteSoftware.Remove(sof);

                }
            }
        }
        #endregion

        #endregion

        public MTObservableCollection<RemoteSoftware> UsersRemoteSoftware
        {
            get { return obRemoteSoftware; }
            set
            {
                obRemoteSoftware = value;
                OnPropertyChanged();
            }
        }
        public void Save()
        {
            xml.SaveUsersRemoteSoftware(obRemoteSoftware);
            MessageBus.Publish<UsersViewViewModelSave>(new UsersViewViewModelSave());
        }
    }
}
