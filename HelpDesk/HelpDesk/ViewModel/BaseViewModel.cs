using HelpDesk.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HelpDesk.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(
        [CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                new PropertyChangedEventArgs(caller));
            }
        }
        #endregion

        public BaseViewModel()
        {
            
        }


        #region MessageBus
        private ImessageBus _messageBus;
        public ImessageBus MessageBus
        {
            get { return _messageBus; }
            set
            {
                if (_messageBus != null)
                    Unsubscribe();
                _messageBus = value;
                Subscribe();
            }
        }
        protected virtual void Unsubscribe() { }
        protected virtual void Subscribe() { }
        protected void PublishMessage<TMessage>(TMessage message)
        {
            if (this.MessageBus != null)
                this.MessageBus.Publish<TMessage>(message);
        }
        #endregion
    }
}
