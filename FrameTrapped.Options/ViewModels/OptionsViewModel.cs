namespace FrameTrapped.Options.ViewModels
{
    using Microsoft.Win32;
    using System;

    using Caliburn.Micro;
    using FrameTrapped.Common.Properties;

    public class OptionsViewModel : Screen
    {
        /// <summary>
        /// The events aggregator.
        /// </summary>
        IEventAggregator _events;

        /// <summary>
        /// The location of SSFIV.exe
        /// </summary>
        private string _steamLocation;

        /// <summary>
        /// Whether Street Fighter is on Steam.
        /// </summary>
        private bool _ssfivSteamVersion;

        /// <summary>
        /// Gets or sets a value for the location of SSFIV.
        /// </summary>
        public string SteamLocation
        {
            get
            {
                return _steamLocation;
            }

            set
            {
                _steamLocation = value;
                Settings.Default.SteamLocation = _steamLocation;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => SteamLocation);
            }
        }

        /// <summary>
        /// Gets or sets a value for the location of SSFIV.
        /// </summary>
        public bool SSFIVSteamVersion
        {
            get
            {
                return _ssfivSteamVersion;
            }

            set
            {
                _ssfivSteamVersion = value;
                Settings.Default.SSFIVSteamVersion = _ssfivSteamVersion;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => SSFIVSteamVersion);
            }
        }

        public void SteamLocationDialog()
        {
            // Create OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = "Steam.exe";
            dlg.Filter = "Steam|Steam.exe";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                SteamLocation = filename;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsViewModel"/> class.
        /// </summary>
        /// <param name="events"></param>
        public OptionsViewModel(IEventAggregator events)
        {
            SteamLocation = Settings.Default.SteamLocation;
            SSFIVSteamVersion = Settings.Default.SSFIVSteamVersion;
            _events = events;
        }
    }
}
