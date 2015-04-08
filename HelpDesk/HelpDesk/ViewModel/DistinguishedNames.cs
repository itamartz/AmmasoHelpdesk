using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace HelpDesk.ViewModel
{
    public class DistinguishedNames : INotifyPropertyChanged
    {
        string computers;

        public string Computers
        {
            get { return computers; }
            set { computers = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        string users;

        public string Users
        {
            get { return users; }
            set { users = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        string printers;

        public string Printers
        {
            get { return printers; }
            set { printers = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
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
