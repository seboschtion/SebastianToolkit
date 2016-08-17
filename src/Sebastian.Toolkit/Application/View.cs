using System;
using Windows.UI.Xaml.Controls;
using Sebastian.Toolkit.MVVM.Contracts;
using Sebastian.Toolkit.MVVM.Messages;

namespace Sebastian.Toolkit.Application
{
    public class View : Page, IView, IMessageSender, IMessageReceiver
    {
        public event EventHandler<object> MessageSent;

        public virtual void ReceiveMessage(object message)
        {

        }

        public void SendMessage(object message)
        {
            MessageSent?.Invoke(this, message);
        }

        public virtual void OnInitialized()
        {
            
        }
    }
}
