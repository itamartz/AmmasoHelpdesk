using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Commands
{
    public interface ImessageBus
    {
        void Subscribe<TMessage>(Action<TMessage> handler);
        void Unsubscribe<TMessage>(Action<TMessage> handler);
        void Publish<TMessage>(TMessage message);
        void Publish(Object message);
    }
    public sealed class MessageBus : ImessageBus
    {
        public string Name { get; set; }
        private Dictionary<Type, List<Object>> _Subscribers = new Dictionary<Type, List<Object>>();

        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (_Subscribers.ContainsKey(typeof(TMessage)))
            {
                 var handlers = _Subscribers[typeof(TMessage)];

                 try
                 {
                     foreach (Action<TMessage> item in handlers)
                     {
                         if (item.Method.Name == handler.Method.Name)
                         {
                             Debug.WriteLine("Alredy exist " + handler.Method.Name);
                         }
                         else
                         {
                             handlers.Add(handler);
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     Debug.WriteLine(ex);
                 }

               

            }
            else
            {
                var handlers = new List<Object>();
                handlers.Add(handler);
                _Subscribers[typeof(TMessage)] = handlers;
            }
        }

        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            if (_Subscribers.ContainsKey(typeof(TMessage)))
            {
                var handlers = _Subscribers[typeof(TMessage)];
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    _Subscribers.Remove(typeof(TMessage));
                }
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            if (_Subscribers.ContainsKey(typeof(TMessage)))
            {
                var handlers = _Subscribers[typeof(TMessage)];
                foreach (Action<TMessage> handler in handlers)
                {
                    handler.Invoke(message);
                }
            }
        }

        public void Publish(Object message)
        {
            var messageType = message.GetType();
            if (_Subscribers.ContainsKey(messageType))
            {
                var handlers = _Subscribers[messageType];
                foreach (var handler in handlers)
                {
                    var actionType = handler.GetType();
                    var invoke = actionType.GetMethod("Invoke", new Type[] { messageType });
                    invoke.Invoke(handler, new Object[] { message });
                }
            }
        }
    }
}
