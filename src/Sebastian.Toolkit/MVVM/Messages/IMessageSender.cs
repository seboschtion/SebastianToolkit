using System;

namespace Sebastian.Toolkit.MVVM.Messages
{
    public interface IMessageSender
    {
        event EventHandler<object> MessageSent;
    }
}