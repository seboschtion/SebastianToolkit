using Sebastian.Toolkit.MVVM.Contracts;

namespace Sebastian.Toolkit.MVVM.Navigation
{
    public interface INavigator
    {
        void Navigate<TViewModel>() where TViewModel : IViewModel;
        void Navigate<TViewModel>(object parameter) where TViewModel : IViewModel;
        void GoBack();
        bool GoBackIf();
        bool CanGoBack { get; }
    }
}