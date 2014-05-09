
namespace TotalConnect.View
{
    using System.Windows;
    using System.Windows.Input;
    using Classes;
    using ViewModel;

    public partial class LoginView
    {
        /// <summary>
        /// Internal copy of the LoginViewModel.
        /// </summary>
        private readonly LoginViewModel m_ViewModel;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LoginView()
        {
            InitializeComponent();

            // Attach navigation handlers..
            this.m_ViewModel = ((ViewModelLocator)Application.Current.Resources["Locator"]).Login;
            this.m_ViewModel.OnNavigateRequest += e => this.NavigationService.Navigate(e);
            this.m_ViewModel.OnNavigateBackRequest += () =>
                {
                    if (this.NavigationService.CanGoBack)
                        this.NavigationService.GoBack();
                };
        }

        /// <summary>
        /// OnNavigatedTo override to ensure we are logged out of a previous session.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Determine if we need to logout..
            if (!string.IsNullOrEmpty(ApplicationState.Instance.Account.SessionId))
            {
                // Attempt to logout of the current session..
                if (this.m_ViewModel.NavigatedFromMainCommand.CanExecute(null))
                    this.m_ViewModel.NavigatedFromMainCommand.Execute(null);
                ApplicationState.Instance.Client.LogoutAsync(ApplicationState.Instance.Account.SessionId);
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPincode_OnTextInputStart(object sender, TextCompositionEventArgs e)
        {
           
        }

        private void txtPincode_OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.txtPincode.Text = string.Empty;
        }
    }
}