
namespace TotalConnect.Model
{
    using System;
    using System.Windows.Media;

    public class ZoneEntry : NotifiableModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ZoneEntry()
        {
            this.ZoneId = -1;
            this.ZoneName = "UNKNOWN";
            this.ZoneStatus = "NULL";
            this.ZoneStatusId = -1;
        }
        
        /// <summary>
        /// Gets or sets the zone id property.
        /// </summary>
        public int ZoneId
        {
            get { return this.Get<int>("ZoneId"); }
            set { this.Set("ZoneId", value); }
        }

        /// <summary>
        /// Gets or sets the zone name property.
        /// </summary>
        public string ZoneName
        {
            get { return this.Get<string>("ZoneName"); }
            set { this.Set("ZoneName", value); }
        }

        /// <summary>
        /// Gets or sets the zone status property.
        /// </summary>
        public string ZoneStatus
        {
            get { return this.Get<string>("ZoneStatus"); }
            set { this.Set("ZoneStatus", value); }
        }

        /// <summary>
        /// Gets or sets the zone status id property.
        /// </summary>
        public int ZoneStatusId
        {
            get { return this.Get<int>("ZoneStatusId"); }
            set { this.Set("ZoneStatusId", value); }
        }
        
        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0:d02}] {1}", ZoneId, ZoneName);
        }
    }
}
