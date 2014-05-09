
namespace TotalConnect.View
{
    using System.Windows;
    using System.Windows.Data;
    using Model;
    using ViewModel;

    public partial class MainView
    {
        /// <summary>
        /// Internal copy of the MainViewModel.
        /// </summary>
        private readonly MainViewModel m_ViewModel;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            // Attach navigation handlers..
            this.m_ViewModel = ((ViewModelLocator)Application.Current.Resources["Locator"]).Main;
            this.m_ViewModel.OnNavigateRequest += e => this.NavigationService.Navigate(e);
            this.m_ViewModel.OnNavigateBackRequest += () =>
                {
                    if (this.NavigationService.CanGoBack)
                        this.NavigationService.GoBack();
                };
        }

        /// <summary>
        /// Filter to use for sorting zone list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoneListFilter(object sender, FilterEventArgs e)
        {
            var zone = e.Item as ZoneEntry;
            e.Accepted = true;
        }

        /// <summary>
        /// OnNavigatedFrom override to ensure our pulse timer is disabled.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Inform the ViewModel to disable the pulse timer..
            if (this.m_ViewModel.NavigatedFromCommand != null && this.m_ViewModel.NavigatedFromCommand.CanExecute(null))
                this.m_ViewModel.NavigatedFromCommand.Execute(null);

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// OnNavigatedTo override to ensure our pulse timer is enabled.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Inform the ViewModel to disable the pulse timer..
            if (this.m_ViewModel.NavigatedToCommand != null && this.m_ViewModel.NavigatedToCommand.CanExecute(null))
                this.m_ViewModel.NavigatedToCommand.Execute(null);

            base.OnNavigatedTo(e);
        }
    }
}