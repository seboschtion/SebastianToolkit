using System.Collections.Generic;

namespace Sebastian.Toolkit.MVVM.Messages
{
    internal class PostalService
    {
        private readonly IDictionary<IMessageSender, IMessageReceiver> _pairs = new Dictionary<IMessageSender, IMessageReceiver>();

        internal void Remove(IMessageSender sender)
        {
            sender.MessageSent -= SenderOnMessageSent;
            _pairs.Remove(sender);
        }

        internal void Add(IMessageSender sender, IMessageReceiver receiver)
        {
            sender.MessageSent += SenderOnMessageSent;
            _pairs.Add(sender, receiver);
        }

        private void SenderOnMessageSent(object sender, object o)
        {
            IMessageReceiver receiver;
            var success = _pairs.TryGetValue((IMessageSender)sender, out receiver);
            if (success)
            {
                receiver.ReceiveMessage(o);
            }
        }
    }
}