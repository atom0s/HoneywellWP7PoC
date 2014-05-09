
namespace TotalConnect.Model
{
    using System;
    using System.IO.IsolatedStorage;
    using TotalConnect.Classes;

    /// <summary>
    /// View Model Base Implementation
    /// 
    /// Base view model object that is inheritable by all view models.
    /// Implements the NotifiableModel class to allow for clean properties.
    /// </summary>
    public abstract class ViewModelBase : NotifiableModel
    {
        /// <summary>
        /// Internal static design mode flag.
        /// </summary>
        private static bool? _isInDesignMode;

        /// <summary>
        /// Internal navigation state object.
        /// </summary>
        private static NavigationArgs _navigationState;

        /// <summary>
        /// Gets if this ViewModelBase is in design mode.
        /// </summary>
        public bool IsInDesignMode
        {
            get { return ViewModelBase.IsInDesignModeStatic; }
        }

        /// <summary>
        /// Gets the static ViewModelBase design mode flag.
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (_isInDesignMode.HasValue)
                    return ViewModelBase._isInDesignMode.Value;

                try
                {
                    IsolatedStorageSettings.ApplicationSettings.Contains("_fakeStorageSettingsName");
                    ViewModelBase._isInDesignMode = false;
                    return ViewModelBase._isInDesignMode.Value;
                }
                catch
                {
                    ViewModelBase._isInDesignMode = true;
                    return ViewModelBase._isInDesignMode.Value;
                }
            }
        }

        /// <summary>
        /// Navigate event delegate.
        /// </summary>
        /// <param name="uri"></param>
        public delegate void NavigateRequest(Uri uri);

        /// <summary>
        /// Navigate back event delegate.
        /// </summary>
        public delegate void NavigateBackRequest();

        /// <summary>
        /// Event to handle a navigate request.
        /// </summary>
        public event NavigateRequest OnNavigateRequest;

        /// <summary>
        /// Event to handle a navigate back request.
        /// </summary>
        public event NavigateBackRequest OnNavigateBackRequest;

        /// <summary>
        /// Gets or sets the current navigation state.
        /// </summary>
        public NavigationArgs NavigationState
        {
            get { return _navigationState; }
            set { _navigationState = value; }
        }

        /// <summary>
        /// Navigates back one page, if possible.
        /// </summary>
        public void NavigateBack()
        {
            if (this.OnNavigateBackRequest != null)
                this.OnNavigateBackRequest();
        }

        /// <summary>
        /// Navigates to the given page.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="state"></param>
        public void Navigate(Uri uri, NavigationArgs state = null)
        {
            this.NavigationState = state;

            if (this.OnNavigateRequest != null)
                this.OnNavigateRequest(uri);
        }
    }
}
