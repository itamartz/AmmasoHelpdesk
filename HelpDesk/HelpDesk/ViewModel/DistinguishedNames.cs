using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HelpDesk.ViewModel
{
    public class DistinguishedNames : INotifyPropertyChanged
    {
        string computers;

        public string Computers
        {
            get { return computers; }
            set { computers = value; OnPropertyChanged(); }
        }

        string users;

        public string Users
        {
            get { return users; }
            set { users = value; OnPropertyChanged(); }
        }

        string printers;

        public string Printers
        {
            get { return printers; }
            set { printers = value; OnPropertyChanged(); }
        }

        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged(); }
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
