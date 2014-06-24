namespace FrameTrapped.Home.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Caliburn.Micro;

    using FrameTrapped.ComboTrainer.Messages;

    class RecentFileViewModel
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        /// <summary>
        /// The full path name of the file.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The command count.
        /// </summary>
        private long _commandCount;

        /// <summary>
        /// The file last modified date
        /// </summary>
        private DateTime _lastModified;

        /// <summary>
        /// The character this file was designed for.
        /// </summary>
        private string _characterName;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

        /// <summary>
        /// Gets the file last modified time.
        /// </summary> 
        public string LastModifiedDisplay
        {
            get
            {
                return string.Format("{0} {1}", _lastModified.ToLongDateString(), _lastModified.ToShortTimeString());
            }
        }

        /// <summary>
        /// Gets the command count.
        /// </summary> 
        public string CommandCount
        {
            get
            {
                return _commandCount.ToString();
            }
        }

        /// <summary>
        /// Load file associated with this item.
        /// </summary>
        public void Load()
        {
            _events.Publish(new OpenTimeLineMessage(_fileName,1));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentFileViewModel" /> class.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="fileName">Full path to the file.</param>
        /// <param name="loadingTime">The loading time.</param>
        /// <param name="thumbnailPath">Full path to the thumbnail image file, if any.</param>
        public RecentFileViewModel(IEventAggregator events, string fileName, string championName = "Generic")
        {
            _events = events;

            _fileName = fileName;
            _characterName = championName;

            if (File.Exists(fileName))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    _lastModified = fileInfo.LastWriteTime;
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Exception while getting recent file info for {0}. {1}", fileName, ex.Message));
                }
            }
        }
    }
}
