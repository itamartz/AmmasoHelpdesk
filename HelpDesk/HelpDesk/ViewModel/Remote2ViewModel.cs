using HelpDesk.Commands;
using HelpDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HelpDesk.ViewModel
{
    public class Remote2ViewModel : BaseViewModel
    {
        ObservableCollection<RemoteSoftware> obRemoteSoftware = new ObservableCollection<RemoteSoftware>();
        XMLApi xml = new XMLApi();

        public Remote2ViewModel()
        {
            Load();
            AllICommands();
        }


        #region Subscribe / Unsubscribe
        protected override void Subscribe()
        {
            base.Subscribe();
        }
        protected override void Unsubscribe()
        {
            base.Unsubscribe();
        }
        #endregion

        private async void Load()
        {
            List<RemoteSoftware> listRemoteSoftware = await xml.GetXMLButtons();
            OBRemoteSoftwares.Clear();
            foreach (RemoteSoftware software in listRemoteSoftware)
            {
                OBRemoteSoftwares.Add(software);
            }

            XmlConfigurationFileLocation = Properties.Settings.Default.XmlConfigurationFileLocation;
            if (string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
                App.XmlConfigurationFileLocation = XmlConfigurationFileLocation;
        }
        private void AllICommands()
        {
            UnloadedCommand = new RelayCommand<object>(DoUnloadedCommand, CanUnloadedCommand);
            AddButtonCommand = new RelayCommand<object>(DoAddButtonCommand, CanAddButtonCommand);
            RmoveButtonCommand = new RelayCommand<object>(DoRmoveButtonCommand, CanRmoveButtonCommand);
        }

        #region Binding
        public ObservableCollection<RemoteSoftware> OBRemoteSoftwares
        {
            get
            {
                return obRemoteSoftware;
            }
            set
            {
                obRemoteSoftware = value;
                OnPropertyChanged();
            }
        }

        private string xmlConfigurationFileLocation;

        public string XmlConfigurationFileLocation
        {
            get { return xmlConfigurationFileLocation; }
            set { xmlConfigurationFileLocation = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommands

        #region Unloaded
        public ICommand UnloadedCommand { get; set; }

        private bool CanUnloadedCommand(object obj)
        {
            return true;
        }

        private void DoUnloadedCommand(object obj)
        {
            xml.Save(obRemoteSoftware);
        }
        #endregion

        #region AddButton
        public ICommand AddButtonCommand { get; set; }

        private bool CanAddButtonCommand(object obj)
        {
            return true;
        }

        private void DoAddButtonCommand(object obj)
        {
            RemoteSoftware software = obj as RemoteSoftware;


            if (software != null)
            {
                RemoteSoftware Newsoftware = new RemoteSoftware();
                Newsoftware.Name = software.Name + "1";
                Newsoftware.Index = obRemoteSoftware.Count;
                Newsoftware.Isremove = software.Isremove;
                Newsoftware.Options = software.Options;
                Newsoftware.ProgramPath = software.ProgramPath;


                obRemoteSoftware.Add(Newsoftware);

                //{
                //    Name = "Default Name" + obRemoteSoftware.Count,
                //    Index = 
                //});
            }

        }
        #endregion

        #region RmoveButton
        public ICommand RmoveButtonCommand { get; set; }

        private bool CanRmoveButtonCommand(object obj)
        {
            return true;
        }

        private void DoRmoveButtonCommand(object obj)
        {
            RemoteSoftware software = obj as RemoteSoftware;

            if (software != null)
            {
                if (obRemoteSoftware.Count - 1 != 0)
                {
                    software.Isremove = true;
                    obRemoteSoftware.Remove(software);
                }
            }
        }
        #endregion

        #endregion



    }
}
