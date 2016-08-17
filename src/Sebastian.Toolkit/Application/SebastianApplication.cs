using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Sebastian.Toolkit.Logging;
using Sebastian.Toolkit.MVVM.Container;
using Sebastian.Toolkit.MVVM.Navigation;

namespace Sebastian.Toolkit.Application
{
    public abstract class SebastianApplication : Windows.UI.Xaml.Application
    {
        private IoC _container;
        private AppFrame _rootFrame;
        private Navigator _navigator;

        public abstract void SetupContainer(IoC container);
        public abstract void NavigateToFirstView(INavigator navigator);
        public virtual void Initialize() { }
        public virtual void Launched(LaunchActivatedEventArgs e) { }
        public virtual void WindowCreated() { }
        public virtual void ConfigureFrame(AppFrame frame) { }

        protected SebastianApplication()
        {
            InitializeInternal();
            UnhandledException += OnUnhandledException;
        }

        private void BuildFrame()
        {
            _navigator = _navigator ?? _container.Singleton<INavigator, Navigator>(_container);
            _rootFrame = _navigator.Frame;
        }

        private void ActivateWindow()
        {
            Window.Current.Content = _rootFrame;
            if (_rootFrame.Content == null)
            {
                NavigateToFirstView(_navigator);
            }
            Window.Current.Activate();
        }

        public void InitializeInternal()
        {
            Initialize();
        }

        protected sealed override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            this.DebugSettings.EnableFrameRateCounter = System.Diagnostics.Debugger.IsAttached;
#endif
            if (_container == null)
            {
                _container = new IoC();
                SetupContainer(_container);
            }

            Launched(e);

            BuildFrame();

            ConfigureFrame(_rootFrame);

            ActivateWindow();

            WindowCreated();
        }
        
        protected void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            this.Logger().Fatal(unhandledExceptionEventArgs.Message, unhandledExceptionEventArgs.Exception);
        }
    }
}