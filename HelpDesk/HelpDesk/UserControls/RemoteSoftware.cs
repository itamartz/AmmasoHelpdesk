using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.UserControls
{
    public class RemoteSoftware : INotifyPropertyChanged
    {
        string programPath;
        public string ProgramPath
        {
            get { return programPath; }
            set
            {
                programPath = value;
                OnPropertyChanged();
            }
        }
        public string Options { get; set; }

        bool _default;
        public bool Default
        {
            get { return _default; }
            set
            {
                _default = value;
                OnPropertyChanged();
            }
        }
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        int index;
        public int Index 
        {

            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged();
            }
        }

        private int column;

        public int Column
        {
            get { return column; }
            set { column = value; }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(
        [CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                new PropertyChangedEventArgs(caller));
            }
        }


        public bool Isremove { get; set; }
    }
}
