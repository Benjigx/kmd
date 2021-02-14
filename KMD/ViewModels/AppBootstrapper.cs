using KMD.Views;
using ReactiveUI;
using Splat;

namespace KMD.ViewModels
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; set; }

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            // Bind 
            RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
            //LogHost.Default.Level = LogLevel.Debug;

            // Navigate to the opening page of the application
            Router.Navigate.Execute(new MangaDownloadVM(this));
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new MangaDownloadView(), typeof(IViewFor<MangaDownloadVM>));
            //dependencyResolver.Register(() => new SecondView(), typeof(IViewFor<SecondViewModel>));
        }
    }
}
