
namespace TotalConnect.Classes
{
    using Model;
    using TotalConnect.TotalConnectService;

    public class ApplicationState
    {
        /// <summary>
        /// Internal application state instance.
        /// </summary>
        private static ApplicationState m_Instance;

        /// <summary>
        /// Internal TotalConnect soap client.
        /// </summary>
        private readonly svcTC2APISoapClient m_Client;

        /// <summary>
        /// Internal account instance.
        /// </summary>
        private Account m_Account;

        /// <summary>
        /// Default Constructor
        /// </summary>
        private ApplicationState()
        {
            this.m_Account = new Account();
            this.m_Client = new svcTC2APISoapClient("svcTC2APISoap");
        }

        /// <summary>
        /// Gets the ApplicationState instance.
        /// </summary>
        public static ApplicationState Instance
        {
            get { return m_Instance ?? (m_Instance = new ApplicationState()); }
        }

        /// <summary>
        /// Gets the account object.
        /// </summary>
        public Account Account
        {
            get { return this.m_Account; }
        }

        /// <summary>
        /// Gets the soap client object.
        /// </summary>
        public svcTC2APISoapClient Client
        {
            get { return this.m_Client; }
        }

        /// <summary>
        /// Loads the application settings.
        /// </summary>
        /// <returns></returns>
        public bool LoadSettings()
        {
            // Attempt to load the app settings from the phone storage..
            var settings = StorageManager.ReadObject<byte[]>("appSettings");
            if (settings == null) return false;

            try
            {
                // Deserialize the account object..
                this.m_Account = ProtobufHandler.Deserialize<Account>(settings);
                this.m_Account.RememberMe = (!string.IsNullOrEmpty(this.m_Account.Username) && !string.IsNullOrEmpty(this.m_Account.Password));
                return true;
            }
            catch
            {
                this.m_Account = new Account();
                return false;
            }
        }

        /// <summary>
        /// Saves the application settings.
        /// </summary>
        /// <returns></returns>
        public bool SaveSettings()
        {
            var settings = ProtobufHandler.Serialize(this.m_Account);
            StorageManager.WriteObject(settings, "appSettings");
            return true;
        }
    }
}
