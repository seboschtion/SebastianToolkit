using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Sebastian.Toolkit.Logging;
using Sebastian.Toolkit.MVVM.Container;
using Sebastian.Toolkit.MVVM.Contracts;
using Sebastian.Toolkit.MVVM.Messages;

namespace Sebastian.Toolkit.MVVM.Navigation
{
    internal class Navigator : INavigator
    {
        private readonly IoC _ioC;
        private readonly PostalService _postalService;
        private readonly NavigationCache _navigationCache;
        private readonly Regex _viewRegex = new Regex(@"(?<Namespace>.*)\.(?<Prefix>.*)(?<Model>Model)(?<Rest>.*)");
        private readonly Type[] _badPageTransitions = {typeof (NavigationThemeTransition)};

        public Navigator(IoC ioC)
        {
            _ioC = ioC;
            _postalService = new PostalService();
            _navigationCache = new NavigationCache(11);
            Frame = new AppFrame
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemBackRequested;
        }

        internal AppFrame Frame { get; }

        public bool CanGoBack => _navigationCache.HasItems;

        public void Navigate<TViewModel>() where TViewModel : IViewModel
        {
            Navigate<TViewModel>(null);
        }

        public void Navigate<TViewModel>(object navigationParameter) where TViewModel : IViewModel
        {
            var viewType = GetViewForViewModel(typeof(TViewModel));
            if (viewType == null)
            {
                throw new NavigatorException("Did not find any view for the given viewmodel.");
            }

            DeactivateCurrent(true);

            var view = CreateView<TViewModel>(viewType);
            var viewModel = view.DataContext as IViewModel;
            view.DataContext = viewModel;
            (view as IView)?.OnInitialized();
            viewModel?.OnActivated(navigationParameter);
            SetView(view);
        }

        private void SetView(FrameworkElement view)
        {
            var viewModel = (IViewModel) view.DataContext;

            RegisterToPostalService(view, viewModel);
            RegisterToPostalService(viewModel, view);

            viewModel.OnViewAttached();

            Frame.Content = view;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private void DeactivateCurrent(bool cache)
        {
            var view = Frame.Content as Page;
            if (view != null)
            {
                var viewModel = (IViewModel)view.DataContext;
                if (cache)
                {
                    _navigationCache.Cache(view);
                    viewModel.OnCached();
                    this.Logger().Debug($"Cached view {view.GetType().FullName}");
                }
                else
                {
                    viewModel.OnDeactivated();
                    this.Logger().Debug($"Deactivated view {view.GetType().FullName}");
                }

                RemoveFromPostalService(viewModel);
                RemoveFromPostalService(view);
            }
        }

        public void GoBack()
        {
            DeactivateCurrent(false);
            var view = _navigationCache.GetAndRemoveLast();
            SetView(view);
        }

        public bool GoBackIf()
        {
            bool couldGoBack = CanGoBack;
            if (CanGoBack)
            {
                GoBack();
            }
            return couldGoBack;
        }

        private void OnSystemBackRequested(object sender, BackRequestedEventArgs e)
        {
            GoBackIf();
            e.Handled = true;
        }

        private Type GetViewForViewModel(Type viewModel)
        {
            var groups = _viewRegex.Match(viewModel.AssemblyQualifiedName).Groups;
            var viewName = groups[2].Value;
            var viewAssemblyQualifiedName = $"{groups[1].Value.Replace("Model", "")}.{viewName}{groups[4]}";
            var type = Type.GetType(viewAssemblyQualifiedName);
            return type;
        }

        private Page CreateView<TViewModel>(Type viewType)
            where TViewModel : IViewModel
        {
            var view = BuildViewInstance(viewType);
            RemoveBadTransitions(view);
            var viewModel = BuildViewModelInstance<TViewModel>();
            view.DataContext = viewModel;
            return view;
        }

        private void RemoveBadTransitions(UIElement page)
        {
            if (page.Transitions == null)
            {
                return;
            }
            var transitions = page.Transitions.Where(t => !_badPageTransitions.Contains(t.GetType())).ToList();
            if (transitions.Count < page.Transitions.Count)
            {
                this.Logger().Warn("There were transitions which had to be removed.");
            }

            page.Transitions.Clear();
            foreach (var transition in transitions)
            {
                page.Transitions.Add(transition);
            }
        }

        private Page BuildViewInstance(Type viewType)
        {
            return (Page)Activator.CreateInstance(viewType);
        }

        private IViewModel BuildViewModelInstance<TViewModel>() where TViewModel : IViewModel
        {
            var viewModelType = typeof (TViewModel);
            var parameterInfos = viewModelType.GetConstructors().First().GetParameters();
            if (parameterInfos.Length > 0)
            {
                var parameters = _ioC.GetMethodsParameters(parameterInfos);
                return (TViewModel)Activator.CreateInstance(typeof(TViewModel), parameters);
            }
            return (TViewModel)Activator.CreateInstance(typeof(TViewModel));
        }

        private void RegisterToPostalService(object sender, object receiver)
        {
            var messageSender = sender as IMessageSender;
            var messageReceiver = receiver as IMessageReceiver;

            if (messageSender != null && messageReceiver != null)
            {
                _postalService.Add(messageSender, messageReceiver);
                this.Logger().Debug($"Added sender: {messageSender.GetType().Name} and receiver: {messageReceiver.GetType().Name} to postalservice.");
            }
        }

        private void RemoveFromPostalService(object participant)
        {
            var senderParticipant = participant as IMessageSender;

            if (senderParticipant != null)
            {
                _postalService.Remove(senderParticipant);
                this.Logger().Debug($"Removed sender from postalservice: {senderParticipant.GetType().Name}.");
            }
        }
    }
}