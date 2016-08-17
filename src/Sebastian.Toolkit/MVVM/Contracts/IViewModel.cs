using Sebastian.Toolkit.MVVM.Messages;

namespace Sebastian.Toolkit.MVVM.Contracts
{
    public interface IViewModel : IMessageSender, IMessageReceiver
    {
        void OnActivated(object parameter);
        void OnViewAttached();
        void OnDeactivated();
        void OnCached();
    }
}