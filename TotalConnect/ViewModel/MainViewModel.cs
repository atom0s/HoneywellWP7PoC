
namespace TotalConnect.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Classes;
    using Extensions;
    using GalaSoft.MvvmLight.Command;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using TotalConnect.Model;
    using TotalConnectService;

    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Internal timer to handle keep-alive and panel update requests.
        /// </summary>
        private readonly DispatcherTimer m_PulseTimer;

        /// <summary>
        /// Internal counter for keep-alive ticks.
        /// </summary>
        private int m_KeepAliveTicks;

        /// <summary>
        /// Internal counter for pulse update ticks.
        /// </summary>
        private int m_PulseUpdateTicks;

        /// <summary>
        /// Internal copy of the panel meta data and status informaton.
        /// </summary>
        private PanelMetadataAndStatusInfo m_LastPanelStatus;

        /// <summary>
        /// Internal list of current zone status information.
        /// </summary>
        private ZoneStatusList m_ZoneStatusList;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainViewModel()
        {
            // Bind command handlers..
            this.ArmAwayCommand = new RelayCommand(HandleArmAwayRequest);
            this.ArmStayCommand = new RelayCommand(HandleArmStayRequest);
            this.DisarmCommand = new RelayCommand(HandleDisarmRequest);
            this.NavigatedFromCommand = new RelayCommand(() => this.m_PulseTimer.Stop());
            this.NavigatedToCommand = new RelayCommand(() => this.m_PulseTimer.Start());

            // Set view model defaults..
            this.m_KeepAliveTicks = 1000;
            this.m_PulseUpdateTicks = 1000;
            this.m_PulseTimer = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 0, 1)
                };
            this.m_PulseTimer.Tick += PulseTimer_Tick;
            this.DebugStatus = "Unknown status at this time..";
            this.ProcessingPanelVisibility = Visibility.Collapsed;
            this.SecurityStatus = "Loading...";
            this.SecurityStatusBackground = new Uri("../Assets/background_arming_disarming.png", UriKind.Relative);
            this.SecurityStatusIcon = new Uri("../Assets/icon_large_arming_disarming.png", UriKind.Relative);
            this.ZoneListLoadingVisibility = Visibility.Visible;
            this.ZoneList = new ObservableCollection<ZoneEntry>();

            // Bind async service handlers..
            ApplicationState.Instance.Client.KeepAliveCompleted += ClientOnKeepAliveCompleted;
            ApplicationState.Instance.Client.GetPanelMetaDataAndFullStatusByDeviceIDCompleted += ClientOnGetPanelMetaDataAndFullStatusByDeviceIdCompleted;
            ApplicationState.Instance.Client.ArmSecuritySystemCompleted += ClientOnArmSecuritySystemCompleted;
            ApplicationState.Instance.Client.DisarmSecuritySystemCompleted += ClientOnDisarmSecuritySystemCompleted;
            ApplicationState.Instance.Client.CheckSecurityPanelLastCommandStateCompleted += ClientOnCheckSecurityPanelLastCommandStateCompleted;
            ApplicationState.Instance.Client.GetZonesListInStateCompleted += ClientOnGetZonesListInStateCompleted;
        }

        /// <summary>
        /// Async handler called when a keep-alive request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnKeepAliveCompleted(object sender, KeepAliveCompletedEventArgs e)
        {
            // Did the keep-alive request fail..
            if (e.Result.ResultCode != 0)
            {
                System.Diagnostics.Debug.WriteLine("KeepAlive Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
                this.m_PulseTimer.Stop();
                this.NavigateBack();
                return;
            }

            this.DebugStatus = "Last Keep-Alive sync: " + DateTime.Now;
        }

        /// <summary>
        /// Async handler called when a panel status update request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnGetPanelMetaDataAndFullStatusByDeviceIdCompleted(object sender, GetPanelMetaDataAndFullStatusByDeviceIDCompletedEventArgs e)
        {
            // Did the panel request complete..
            if (e.Result.ResultCode == 0)
            {
                this.DebugStatus = "Last panel update sync: " + DateTime.Now;

                // Store the result..
                this.m_LastPanelStatus = e.Result.PanelMetadataAndStatus;

                // Null reference check.. (rare but happens..)
                if (e.Result.PanelMetadataAndStatus == null)
                    return;

                // Display panel status information..
                var state = e.Result.PanelMetadataAndStatus.Partitions[0].ArmingState;
                switch (state)
                {

                    case 10200: // disarmed
                    case 10211: // disarmed (bypass)
                        {
                            this.SecurityStatus = "Disarmed";
                            this.SecurityStatusBackground = new Uri("../Assets/background_disarmed.png", UriKind.Relative);
                            this.SecurityStatusIcon = new Uri("../Assets/icon_large_disarm.png", UriKind.Relative);
                            break;
                        }

                    case 10201: // armed-away
                    case 10202: // armed-away (bypass)
                    case 10205: // armed-away (instant)
                    case 10206: // armed-away (instant-bypass)
                        {
                            this.SecurityStatus = "Armed Away";
                            this.SecurityStatusBackground = new Uri("../Assets/background_armed.png", UriKind.Relative);
                            this.SecurityStatusIcon = new Uri("../Assets/icon_large_arm.png", UriKind.Relative);
                            break;
                        }

                    case 10203: // armed-stay
                    case 10204: // armed-stay (bypass)
                    case 10209: // armed-stay (instant)
                    case 10210: // armed-stay (instant-bypass)
                        {

                            this.SecurityStatus = "Armed Stay";
                            this.SecurityStatusBackground = new Uri("../Assets/background_armed.png", UriKind.Relative);
                            this.SecurityStatusIcon = new Uri("../Assets/icon_large_arm.png", UriKind.Relative);
                            break;
                        }

                    case 10307: // arming
                    case 10308: // disarming
                        {
                            this.SecurityStatus = (state == 10307) ? "Arming..." : "Disarming...";
                            this.SecurityStatusBackground = new Uri("../Assets/background_arming_disarming.png", UriKind.Relative);
                            this.SecurityStatusIcon = new Uri("../Assets/icon_large_arming_disarming.png", UriKind.Relative);
                            break;
                        }

                    default:
                        {
                            this.SecurityStatus = string.Format("Unknown: {0}", state);
                            this.SecurityStatusBackground = new Uri("../Assets/background_arming_disarming.png", UriKind.Relative);
                            this.SecurityStatusIcon = new Uri("../Assets/icon_large_arming_disarming.png", UriKind.Relative);
                            break;
                        }
                }

                // Obtain the zone status list..
                var account = ApplicationState.Instance.Account;
                ApplicationState.Instance.Client.GetZonesListInStateAsync(
                    account.SessionId,
                    account.SessionDetails.Locations[0].LocationID,
                    1, 0);

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("PulseUpdate Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
            }
        }

        /// <summary>
        /// Async handler called when an arm security system request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnArmSecuritySystemCompleted(object sender, ArmSecuritySystemCompletedEventArgs e)
        {
            // Did the arm system request complete..
            if (e.Result.ResultCode == 0)
            {
                this.ProcessingPanelVisibility = Visibility.Collapsed;
                PlaySound("Assets/success.wav");
            }

            // Do we need to query for the result..
            else if (e.Result.ResultCode == 4500)
            {
                this.ProcessingPanelVisibility = Visibility.Visible;

                var account = ApplicationState.Instance.Account;
                ApplicationState.Instance.Client.CheckSecurityPanelLastCommandStateAsync(
                    account.SessionId,
                    account.SessionDetails.Locations[0].LocationID,
                    account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                    -1);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ArmSystem Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
            }
        }

        /// <summary>
        /// Async handler called when a disarm security system request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnDisarmSecuritySystemCompleted(object sender, DisarmSecuritySystemCompletedEventArgs e)
        {
            // Did the disarm system request complete..
            if (e.Result.ResultCode == 0)
            {
                this.ProcessingPanelVisibility = Visibility.Collapsed;
                PlaySound("Assets/success.wav");
            }

            // Do we need to query for the result..
            else if (e.Result.ResultCode == 4500)
            {
                this.ProcessingPanelVisibility = Visibility.Visible;

                var account = ApplicationState.Instance.Account;
                ApplicationState.Instance.Client.CheckSecurityPanelLastCommandStateAsync(
                    account.SessionId,
                    account.SessionDetails.Locations[0].LocationID,
                    account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                    -1);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ArmSystem Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
            }
        }

        /// <summary>
        /// Async handler called when a poll for last command status request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnCheckSecurityPanelLastCommandStateCompleted(object sender, CheckSecurityPanelLastCommandStateCompletedEventArgs e)
        {
            switch (e.Result.ResultCode)
            {
                case 0: // success
                    this.ProcessingPanelVisibility = Visibility.Collapsed;
                    return;

                case -4502: // invalid pin
                    MessageBox.Show("Invalid pincode entered for security command.", "Error", MessageBoxButton.OK);
                    return;

                case 4501: // repoll require (command scheduled)
                    {
                        this.ProcessingPanelVisibility = Visibility.Visible;

                        var account = ApplicationState.Instance.Account;
                        ApplicationState.Instance.Client.CheckSecurityPanelLastCommandStateAsync(
                            account.SessionId,
                            account.SessionDetails.Locations[0].LocationID,
                            account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                            -1);
                        return;
                    }

                default:
                    {
                        System.Diagnostics.Debug.WriteLine("PollLastCommand Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
                        return;
                    }
            }
        }

        /// <summary>
        /// Async handler called when a poll for zones command status request has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnGetZonesListInStateCompleted(object sender, GetZonesListInStateCompletedEventArgs e)
        {
            // Did the zone request complete..
            if (e.Result.ResultCode == 0)
            {
                // Store the zone information..
                this.ZoneListLoadingVisibility = Visibility.Collapsed;
                this.m_ZoneStatusList = e.Result.ZoneStatus;

                // Update the zone list..
                if (this.m_LastPanelStatus != null)
                {
                    foreach (var zone in this.m_ZoneStatusList.Zones.Select(z => new ZoneEntry
                        {
                            ZoneId = z.ZoneID,
                            ZoneName = this.GetZoneName(z.ZoneID),
                            ZoneStatus = this.ConvertZoneStatus(z.ZoneStatus),
                            ZoneStatusId = z.ZoneStatus
                        }))
                    {
                        this.ZoneList.AddUnique(zone);
                    }
                }
            }

            // Do we need to query for the result..
            else if (e.Result.ResultCode == 4500)
            {
                this.ProcessingPanelVisibility = Visibility.Visible;

                var account = ApplicationState.Instance.Account;
                ApplicationState.Instance.Client.CheckSecurityPanelLastCommandStateAsync(
                    account.SessionId,
                    account.SessionDetails.Locations[0].LocationID,
                    account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                    -1);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GetZoneList Result Code Was: {0} - {1}", e.Result.ResultCode, e.Result.ResultData);
            }
        }

        /// <summary>
        /// Pulse timer tick event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PulseTimer_Tick(object sender, EventArgs e)
        {
            // Update tick counters..
            this.m_KeepAliveTicks++;
            this.m_PulseUpdateTicks++;

            //
            // Keep-alive ticks.. [Every 10 seconds.]
            //

            if (this.m_KeepAliveTicks >= 10)
            {
                this.m_KeepAliveTicks = 0;
                ApplicationState.Instance.Client.KeepAliveAsync(ApplicationState.Instance.Account.SessionId);
            }

            //
            // Pulse tick.. [Every 5 seconds.]
            //

            if (this.m_PulseUpdateTicks >= 5)
            {
                this.m_PulseUpdateTicks = 0;

                // Obtain the panel information..
                var account = ApplicationState.Instance.Account;
                ApplicationState.Instance.Client.GetPanelMetaDataAndFullStatusByDeviceIDAsync(
                    account.SessionId,
                    account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                    0, 0, 1);
            }

            System.Diagnostics.Debug.WriteLine("PulseTimer_Tick Tick!");
        }

        /// <summary>
        /// Handles an arm way request.
        /// </summary>
        private void HandleArmAwayRequest()
        {
            // Validation checks..
            var account = ApplicationState.Instance.Account;
            if (string.IsNullOrEmpty(account.SessionId) || account.SessionDetails == null)
            {
                this.NavigateBack();
                return;
            }

            // Display processing panel..
            this.ProcessingPanelVisibility = Visibility.Visible;

            // Attempt to arm (away) the system..
            ApplicationState.Instance.Client.ArmSecuritySystemAsync(
                account.SessionId,
                account.SessionDetails.Locations[0].LocationID,
                account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                0, account.Pincode);
        }

        /// <summary>
        /// Handles an arm (stay) request.
        /// </summary>
        private void HandleArmStayRequest()
        {
            // Validation checks..
            var account = ApplicationState.Instance.Account;
            if (string.IsNullOrEmpty(account.SessionId) || account.SessionDetails == null)
            {
                this.NavigateBack();
                return;
            }

            // Display processing panel..
            this.ProcessingPanelVisibility = Visibility.Visible;

            // Attempt to arm (stay) the system..
            ApplicationState.Instance.Client.ArmSecuritySystemAsync(
                account.SessionId,
                account.SessionDetails.Locations[0].LocationID,
                account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                1, account.Pincode);
        }

        /// <summary>
        /// Handles a disarm request.
        /// </summary>
        private void HandleDisarmRequest()
        {
            // Validation checks..
            var account = ApplicationState.Instance.Account;
            if (string.IsNullOrEmpty(account.SessionId) || account.SessionDetails == null)
            {
                this.NavigateBack();
                return;
            }

            // Display processing panel..
            this.ProcessingPanelVisibility = Visibility.Visible;

            // Attempt to arm (away) the system..
            ApplicationState.Instance.Client.DisarmSecuritySystemAsync(
                account.SessionId,
                account.SessionDetails.Locations[0].LocationID,
                account.SessionDetails.Locations[0].DeviceList[0].DeviceID,
                account.Pincode);
        }

        /// <summary>
        /// Plays a sound file from the application assets.
        /// </summary>
        /// <param name="soundName"></param>
        private static void PlaySound(string soundName)
        {
            using (var stream = TitleContainer.OpenStream(soundName))
            {
                if (stream == null) 
                    return;

                var effect = SoundEffect.FromStream(stream);
                FrameworkDispatcher.Update();
                effect.Play();
            }
        }

        /// <summary>
        /// Obtains the zone name of the given zone id.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        private string GetZoneName(int zoneId)
        {
            // Ensure the session details exist..
            var account = ApplicationState.Instance.Account;
            if (account.SessionDetails == null || this.m_LastPanelStatus == null)
                return string.Format("Unknown Zone: {0}", zoneId);

            // Ensure the panel info exists..
            foreach (var zone in this.m_LastPanelStatus.Zones.Where(zone => zone.ZoneID == zoneId))
                return zone.ZoneDescription;
            return string.Format("Unknown Zone: {0}", zoneId);
        }

        /// <summary>
        /// Converts the zone status id to its string format.
        /// </summary>
        /// <param name="zoneStatus"></param>
        /// <returns></returns>
        private string ConvertZoneStatus(int zoneStatus)
        {
            var flags = new List<string>();

            if (zoneStatus == 0)
                flags.Add("Normal");
            if ((zoneStatus & 1) > 0)
                flags.Add("Bypassed");
            if ((zoneStatus & 2) > 0)
                flags.Add("Faulted");
            if ((zoneStatus & 8) > 0)
                flags.Add("Trouble");
            if ((zoneStatus & 16) > 0)
                flags.Add("Tampered");
            if ((zoneStatus & 32) > 0)
                flags.Add("Supervision Failed");

            return string.Join(", ", flags);
        }
        
        /// <summary>
        /// Command invoked when this view is navigated from.
        /// </summary>
        public ICommand NavigatedFromCommand
        {
            get { return this.Get<ICommand>("NavigatedFromCommand"); }
            set { this.Set("NavigatedFromCommand", value); }
        }

        /// <summary>
        /// Command invoked when this view is navigated to.
        /// </summary>
        public ICommand NavigatedToCommand
        {
            get { return this.Get<ICommand>("NavigatedToCommand"); }
            set { this.Set("NavigatedToCommand", value); }
        }

        /// <summary>
        /// Gets or sets the security status property.
        /// </summary>
        public string SecurityStatus
        {
            get { return this.Get<string>("SecurityStatus"); }
            set { this.Set("SecurityStatus", value); }
        }

        /// <summary>
        /// Gets or sets the security status icon property.
        /// </summary>
        public Uri SecurityStatusBackground
        {
            get { return this.Get<Uri>("SecurityStatusBackground"); }
            set { this.Set("SecurityStatusBackground", value); }
        }

        /// <summary>
        /// Gets or sets the security status icon property.
        /// </summary>
        public Uri SecurityStatusIcon
        {
            get { return this.Get<Uri>("SecurityStatusIcon"); }
            set { this.Set("SecurityStatusIcon", value); }
        }

        /// <summary>
        /// Gets or sets the debug status property.
        /// </summary>
        public string DebugStatus
        {
            get { return this.Get<string>("DebugStatus"); }
            set { this.Set("DebugStatus", value); }
        }

        /// <summary>
        /// Gets or sets the processing panel visibility property.
        /// </summary>
        public Visibility ProcessingPanelVisibility
        {
            get { return this.Get<Visibility>("ProcessingPanelVisibility"); }
            set { this.Set("ProcessingPanelVisibility", value); }
        }

        /// <summary>
        /// Command to invoke the arm away handler.
        /// </summary>
        public ICommand ArmAwayCommand
        {
            get { return this.Get<ICommand>("ArmAwayCommand"); }
            set { this.Set("ArmAwayCommand", value); }
        }

        /// <summary>
        /// Command to invoke the arm stay handler.
        /// </summary>
        public ICommand ArmStayCommand
        {
            get { return this.Get<ICommand>("ArmStayCommand"); }
            set { this.Set("ArmStayCommand", value); }
        }

        /// <summary>
        /// Command to invoke the disarm handler.
        /// </summary>
        public ICommand DisarmCommand
        {
            get { return this.Get<ICommand>("DisarmCommand"); }
            set { this.Set("DisarmCommand", value); }
        }

        /// <summary>
        /// Gets or sets the zone list loading visibility property.
        /// </summary>
        public Visibility ZoneListLoadingVisibility
        {
            get { return this.Get<Visibility>("ZoneListLoadingVisibility"); }
            set { this.Set("ZoneListLoadingVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the zone list property.
        /// </summary>
        public ObservableCollection<ZoneEntry> ZoneList
        {
            get { return this.Get<ObservableCollection<ZoneEntry>>("ZoneList"); }
            set { this.Set("ZoneList", value); }
        }
    }
}
