using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HelpDesk.Commands;

namespace HelpDesk.ViewModel
{
    public class UsersViewViewModel : BaseViewModel
    {
        XMLApi xml = new XMLApi();
        private ObservableCollection<RemoteSoftware> obRemoteSoftware;
        public UsersViewViewModel()
        {
            Brows = new RelayCommand<object>(DoBrows, CanBrows);
            Delete = new RelayCommand<object>(DoDelete, CanDelete);

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
            //List<RemoteSoftware> listRemoteSoftware = new List<RemoteSoftware>();
            //listRemoteSoftware.Add(new RemoteSoftware() { Name = "Default Name" });
            List<RemoteSoftware> listRemoteSoftware = await xml.GetUsersRemoteSoftwares();
            UsersRemoteSoftware = new ObservableCollection<RemoteSoftware>(listRemoteSoftware);
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
                RemoteSoftware re = (obj as RemoteSoftware);
                dlg.Title = string.Format("Brows for {0} exe", re.Name);
                Nullable<bool> result = dlg.ShowDialog();
                // Get the selected file name and display in a TextBox
                if (result == true)
                {
                    //TextBox te = FindVisualChildren<TextBox>(this).Where(t => t.Text == re.ProgramPath).FirstOrDefault();
                    //te.Text = dlg.FileName;
                    re.ProgramPath = dlg.FileName;
                }
            }

            

           

            //BtnRemove
            //if (obj != null)
            //{

            //    System.Windows.Controls.Button b = (obj as System.Windows.Controls.Button);
            //    if (obRemoteSoftware.Count - 1 != 0)
            //    {
            //        //int index = Convert.ToInt32(b.Tag);
            //        RemoteSoftware sof = (b.Tag as RemoteSoftware);
            //        //RemoteSoftware sof = obRemoteSoftware[index];
            //        sof.Isremove = true;
            //        obRemoteSoftware.Remove(sof);

            //    }
            //}
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
            obRemoteSoftware.Add(new RemoteSoftware()
            {
                Name = "Default Name" + obRemoteSoftware.Count,
                Index = obRemoteSoftware.Count
            });
        }
        #endregion

        #region Delete
        public ICommand Delete { get; set; }
        private bool CanDelete(object obj)
        {
            return obRemoteSoftware.Count - 1 != 0;
        }

        private void DoDelete(object obj)
        {
            if (obj != null)
            {

                System.Windows.Controls.Button b = (obj as System.Windows.Controls.Button);
                if (obRemoteSoftware.Count - 1 != 0)
                {
                    //int index = Convert.ToInt32(b.Tag);
                    RemoteSoftware sof = (b.Tag as RemoteSoftware);
                    //RemoteSoftware sof = obRemoteSoftware[index];
                    sof.Isremove = true;
                    obRemoteSoftware.Remove(sof);

                }
            }
        }
        #endregion

        #endregion
        public ObservableCollection<RemoteSoftware> UsersRemoteSoftware
        {
            get { return obRemoteSoftware; }
            set
            {
                obRemoteSoftware = value;
                OnPropertyChanged();
            }
        }

    }
}
