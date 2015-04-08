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

            ActiveDirectoryCheckBox = new RelayCommand<object>(DoActiveDirectoryCheckBox, canActiveDirectoryCheckBox);
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

        public ICommand ActiveDirectoryCheckBox { get; set; }
        private void DoActiveDirectoryCheckBox(object obj)
        {

            CheckBox computerCheckBox = obj as CheckBox;

            ActiveDirectoryObjectPublish adop = new ActiveDirectoryObjectPublish();
            adop.ActiveDirectoryObject = (ActiveDirectoryObject)Enum.Parse(typeof(ActiveDirectoryObject), computerCheckBox.Tag.ToString());
            adop.Ischeck = (bool)computerCheckBox.IsChecked;

            if (adop != null)
                PublishMessage<ActiveDirectoryObjectPublish>(adop);
        }

        private bool canActiveDirectoryCheckBox(object obj)
        {
            return true;
        }

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
