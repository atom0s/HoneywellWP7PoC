
namespace TotalConnect.Model
{
    using System.Xml.Serialization;
    using ProtoBuf;
    using TotalConnectService;

    [ProtoContract]
    public class Account
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Account()
        {
            this.Clear();
        }

        /// <summary>
        /// Clears all current properties.
        /// </summary>
        public void Clear()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.RememberMe = false;
            this.SessionId = string.Empty;
            this.SessionDetails = null;
        }

        /// <summary>
        /// Gets or sets the account username.
        /// </summary>
        [XmlElement, ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the account password.
        /// </summary>
        [XmlElement, ProtoMember(2)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the account pincode.
        /// </summary>
        [XmlElement, ProtoMember(3)]
        public int Pincode { get; set; }

        /// <summary>
        /// Gets or sets the account remember me status.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the account session id.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the account session details.
        /// </summary>
        public SessionDetailResults SessionDetails;
    }
}
