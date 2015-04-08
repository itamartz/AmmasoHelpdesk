using HelpDesk.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HelpDesk.ViewModel
{
    public class OptionsViewModel : BaseViewModel
    {
        public OptionsViewModel()
        {
            UnloadedCommand = new RelayCommand<object>(DoUnloadedCommand, canUnloadedCommand);
            MessageBus = DependencyInjection.SimpleContainer.Get<ImessageBus>();
        }


        #region UnloadedCommand
        public ICommand UnloadedCommand { get; set; }
        private bool canUnloadedCommand(object obj)
        {
            return true;
        }
        private void DoUnloadedCommand(object obj)
        {
            Unloaded();
        }

        #endregion
        public void Unloaded()
        {
            PublishMessage<ActiveDirectorySave>(new ActiveDirectorySave());
        }

    }
}
