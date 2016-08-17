using System;
using Sebastian.Toolkit.MVVM.Contracts;
using Sebastian.Toolkit.Util;

namespace Sebastian.Toolkit.Application
{
    public class ViewModel : PropertyChangedBase, IViewModel
    {
        public event EventHandler<object> MessageSent;

        public virtual void OnActivated(object parameter) { }
        public virtual void OnViewAttached() { }
        public virtual void OnDeactivated() { }
        public virtual void OnCached() { }
        public virtual void ReceiveMessage(object message) { }

        protected void SendMessage(object e)
        {
            MessageSent?.Invoke(this, e);
        }
    }
}