
namespace TotalConnect.ViewModel
{
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;

    public class ViewModelLocator
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static ViewModelLocator()
        {
            // Prepare locator service..
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register view models to service..
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        /// <summary>
        /// Gets the LoginViewModel object.
        /// </summary>
        public LoginViewModel Login
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }

        /// <summary>
        /// Gets the MainViewModel object.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
    }
}
