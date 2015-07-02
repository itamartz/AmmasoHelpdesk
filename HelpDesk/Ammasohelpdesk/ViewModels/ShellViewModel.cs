using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ammasohelpdesk.ViewModels
{
    public class ShellViewModel
    {
        public ShellViewModel()
        {
            Title = DateTime.Now.ToString();

            ICommands();
        }

        private void ICommands()
        {
            Exit = new DelegateCommand<object>(ApplicationExit);
        }


        #region Props
        
        public string Title { get; set; }
        
        #endregion       

        #region ICommand

        public ICommand Exit { get; private set; }
        private void ApplicationExit(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        #endregion
    }
}
