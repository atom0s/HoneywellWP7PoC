
namespace TotalConnect.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Classes;
    using GalaSoft.MvvmLight.Command;
    using Microsoft.Phone.Tasks;
    using TotalConnect.Model;
    using TotalConnectService;

    public class LoginViewModel : ViewModelBase
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LoginViewModel()
        {
            // Bind command handlers..
            this.ForgotUsernamePasswordCommand = new RelayCommand(HandleForgotUsernamePasswordRequest);
            this.LoginCommand = new RelayCommand(HandleLoginRequest);
            this.NavigatedFromMainCommand = new RelayCommand(() =>
                {
                    // Adjust ui while waiting for logout response..
                    this.LoginEnabled = false;
                    this.LoginErrorVisibility = Visibility.Collapsed;
                    this.LoginStatus = "Logging in.. please wait...";
                    this.PleaseWaitLoginVisibility = Visibility.Collapsed;
                    this.PleaseWaitLogoutVisibility = Visibility.Visible;
                });

            // Set view model defaults..
            this.LoginEnabled = true;
            this.LoginErrorVisibility = Visibility.Collapsed;
            this.LoginStatus = "Logging in.. please wait...";
            this.Password = ApplicationState.Instance.Account.Password;
            this.Pincode = ApplicationState.Instance.Account.Pincode;
            this.PleaseWaitLoginVisibility = Visibility.Collapsed;
            this.PleaseWaitLogoutVisibility = Visibility.Collapsed;
            this.RememberMe = ApplicationState.Instance.Account.RememberMe;
            this.Username = ApplicationState.Instance.Account.Username;
            
            // Bind async service handlers..
            ApplicationState.Instance.Client.AuthenticateUserLoginCompleted += ClientOnAuthenticateUserLoginCompleted;
            ApplicationState.Instance.Client.GetSessionDetailsCompleted += ClientOnGetSessionDetailsCompleted;
            ApplicationState.Instance.Client.LogoutCompleted += ClientOnLogoutCompleted;
        }

        /// <summary>
        /// Async handler called when a login request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnAuthenticateUserLoginCompleted(object sender, AuthenticateUserLoginCompletedEventArgs e)
        {
            // Did the login fail..
            if (e.Result.ResultCode != 0)
            {
                // Restore the ui controls..
                this.LoginEnabled = true;
                this.LoginErrorVisibility = Visibility.Visible;
                this.PleaseWaitLoginVisibility = Visibility.Collapsed;
                return;
            }

            // Store the account information..
            ApplicationState.Instance.Account.Username = this.Username;
            ApplicationState.Instance.Account.Password = this.Password;
            ApplicationState.Instance.Account.Pincode = this.Pincode;
            ApplicationState.Instance.Account.RememberMe = this.RememberMe;
            ApplicationState.Instance.Account.SessionId = e.Result.SessionID;

            // Obtain session information..
            this.LoginStatus = "Obtaining session info..";
            ApplicationState.Instance.Client.GetSessionDetailsAsync(e.Result.SessionID, 34857971, "2.2.0");
        }

        /// <summary>
        /// Async handler called when a get session details request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnGetSessionDetailsCompleted(object sender, GetSessionDetailsCompletedEventArgs e)
        {
            // Did the request fail..
            if (e.Result.ResultCode != 0)
            {
                // Clear the account data..
                ApplicationState.Instance.Account.Clear();

                // Restore the ui controls..
                this.LoginEnabled = true;
                this.LoginErrorVisibility = Visibility.Visible;
                this.PleaseWaitLoginVisibility = Visibility.Collapsed;
                return;
            }

            // Store the session details..
            ApplicationState.Instance.Account.SessionDetails = e.Result;

            // Navigate to the main page..
            this.Navigate(new Uri("/View/MainView.xaml", UriKind.Relative));

            // Restore the ui controls..
            this.LoginEnabled = true;
            this.LoginErrorVisibility = Visibility.Collapsed;
            this.PleaseWaitLoginVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Async handler called when a logout request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnLogoutCompleted(object sender, LogoutCompletedEventArgs e)
        {
            // Clear the session id..
            ApplicationState.Instance.Account.SessionId = string.Empty;

            // Restore the ui..
            this.LoginEnabled = true;
            this.LoginErrorVisibility = Visibility.Collapsed;
            this.PleaseWaitLoginVisibility = Visibility.Collapsed;
            this.PleaseWaitLogoutVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles a login request.
        /// </summary>
        private void HandleLoginRequest()
        {
            // Disable previous login error if present..
            this.LoginErrorVisibility = Visibility.Collapsed;
            
            // Ensure the username is set..
            if (string.IsNullOrEmpty(this.Username))
            {
                MessageBox.Show("You MUST fill in a valid username!", "Invalid Username!", MessageBoxButton.OK);
                return;
            }

            // Ensure the password is set..
            if (string.IsNullOrEmpty(this.Password))
            {
                MessageBox.Show("You MUST fill in a valid password!", "Invalid Password!", MessageBoxButton.OK);
                return;
            }

            // Ensure the pincode is set..
            if (this.Pincode.ToString().Length != 4)
            {
                MessageBox.Show("You MUST fill in a valid pincode!", "Invalid pincode!", MessageBoxButton.OK);
                return;
            }

            try
            {
                // Attempt to login..
                this.LoginEnabled = false;
                this.LoginStatus = "Logging in.. please wait...";
                this.PleaseWaitLoginVisibility = Visibility.Visible;
                ApplicationState.Instance.Client.AuthenticateUserLoginAsync(this.Username, this.Password, 34857971, "2.2.0");
            }
            catch
            {
                // Restore ui on error..
                this.LoginEnabled = true;
                this.LoginErrorVisibility = Visibility.Visible;
                this.PleaseWaitLoginVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles a forgot username or password request.
        /// </summary>
        private void HandleForgotUsernamePasswordRequest()
        {
            // Launch browser to attempt to restore username or password..
            var webTask = new WebBrowserTask
                {
                    Uri = new System.Uri("https://rs.alarmnet.com/TotalConnect2/LoginHelp/Index", System.UriKind.Absolute)
                };
            webTask.Show();
        }

        /// <summary>
        /// Command to invoke the navigated from main handler.
        /// </summary>
        public ICommand NavigatedFromMainCommand
        {
            get { return this.Get<ICommand>("NavigatedFromMainCommand"); }
            set { this.Set("NavigatedFromMainCommand", value); }
        }

        /// <summary>
        /// Command to invoke the login handler.
        /// </summary>
        public ICommand LoginCommand
        {
            get { return this.Get<ICommand>("LoginCommand"); }
            set { this.Set("LoginCommand", value); }
        }

        /// <summary>
        /// Command to invoke the forgot username or password handler.
        /// </summary>
        public ICommand ForgotUsernamePasswordCommand
        {
            get { return this.Get<ICommand>("ForgotUsernamePasswordCommand"); }
            set { this.Set("ForgotUsernamePasswordCommand", value); }
        }

        /// <summary>
        /// Gets or sets the login enabled property.
        /// </summary>
        public bool LoginEnabled
        {
            get { return this.Get<bool>("LoginEnabled"); }
            set { this.Set("LoginEnabled", value); }
        }

        /// <summary>
        /// Gets or sets the please wait login visibility property.
        /// </summary>
        public Visibility PleaseWaitLoginVisibility
        {
            get { return this.Get<Visibility>("PleaseWaitLoginVisibility"); }
            set { this.Set("PleaseWaitLoginVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the please wait logout visibility property.
        /// </summary>
        public Visibility PleaseWaitLogoutVisibility
        {
            get { return this.Get<Visibility>("PleaseWaitLogoutVisibility"); }
            set { this.Set("PleaseWaitLogoutVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the login error visibility property.
        /// </summary>
        public Visibility LoginErrorVisibility
        {
            get { return this.Get<Visibility>("LoginErrorVisibility"); }
            set { this.Set("LoginErrorVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the login status property.
        /// </summary>
        public string LoginStatus
        {
            get { return this.Get<string>("LoginStatus"); }
            set { this.Set("LoginStatus", value); }
        }

        /// <summary>
        /// Gets or sets the username property.
        /// </summary>
        public string Username
        {
            get { return this.Get<string>("Username"); }
            set { this.Set("Username", value); }
        }

        /// <summary>
        /// Gets or sets the password property.
        /// </summary>
        public string Password
        {
            get { return this.Get<string>("Password"); }
            set { this.Set("Password", value); }
        }

        /// <summary>
        /// Gets or sets the pincode property.
        /// </summary>
        public int Pincode
        {
            get { return this.Get<int>("Pincode"); }
            set { this.Set("Pincode", value); }
        }

        /// <summary>
        /// Gets or sets the remember me property.
        /// </summary>
        public bool RememberMe
        {
            get { return this.Get<bool>("RememberMe"); }
            set { this.Set("RememberMe", value); }
        }
    }
}
