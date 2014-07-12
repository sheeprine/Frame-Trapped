namespace FrameTrapped.ComboTrainer.ViewModels
{
    using System;
    using System.Windows;

    using Caliburn.Micro;

    using FrameTrapped.ComboTrainer.Messages;

    using Size = System.Drawing.Size;

    /// <summary>
    /// </summary>
    public class ComboTrainerViewModel : Screen, 
                                         IHandle<AddTimeLineItemMessage>, 
                                         IHandle<OpenTimeLineMessage>, 
                                         IHandle<SaveTimeLineMessage>
    {
        /// <summary>
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// </summary>
        private GameViewModel _gameViewModel;

        /// <summary>
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// </summary>
        private bool _playerOneTimeLineEnabled;

        /// <summary>
        /// </summary>
        private TimeLineViewModel _playerOneTimeLineViewModel;

        /// <summary>
        /// </summary>
        private bool _playerTwoTimeLineEnabled;

        /// <summary>
        /// </summary>
        private TimeLineViewModel _playerTwoTimeLineViewModel;

        /// <summary>
        /// </summary>
        private bool _repeat;

        /// <summary>
        /// </summary>
        private int _repeatAmount;

        /// <summary>
        /// </summary>
        private Size _selectedResolution;

        /// <summary>
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        /// <summary>
        /// </summary>
        public bool Repeat
        {
            get
            {
                return _repeat;
            }

            set
            {
                _repeat = value;
                NotifyOfPropertyChange(() => Repeat);
            }
        }

        /// <summary>
        /// </summary>
        public int RepeatAmount
        {
            get
            {
                if (Repeat)
                {
                    return _repeatAmount;
                }
                else
                {
                    return 1;
                }
            }

            set
            {
                _repeatAmount = value;
                NotifyOfPropertyChange(() => RepeatAmount);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Player One timeline is enabled.
        /// </summary>
        public bool PlayerOneTimeLineEnabled
        {
            get
            {
                return _playerOneTimeLineEnabled;
            }

            set
            {
                _playerOneTimeLineEnabled = value;
                NotifyOfPropertyChange(() => PlayerOneTimeLineEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Player Two timeline is enabled.
        /// </summary>
        public bool PlayerTwoTimeLineEnabled
        {
            get
            {
                return _playerTwoTimeLineEnabled;
            }

            set
            {
                _playerTwoTimeLineEnabled = value;
                NotifyOfPropertyChange(() => PlayerTwoTimeLineEnabled);
            }
        }

        /// <summary>
        /// </summary>
        public TimeLineViewModel PlayerOneTimeLineViewModel
        {
            get
            {
                return _playerOneTimeLineViewModel;
            }

            set
            {
                _playerOneTimeLineViewModel = value;
                NotifyOfPropertyChange(() => PlayerOneTimeLineViewModel);
            }
        }

        /// <summary>
        /// </summary>
        public TimeLineViewModel PlayerTwoTimeLineViewModel
        {
            get
            {
                return _playerTwoTimeLineViewModel;
            }

            set
            {
                _playerTwoTimeLineViewModel = value;
                NotifyOfPropertyChange(() => PlayerTwoTimeLineViewModel);
            }
        }

        /// <summary>
        /// </summary>
        public GameViewModel GameViewModel
        {
            get
            {
                return _gameViewModel;
            }

            set
            {
                _gameViewModel = value;
                NotifyOfPropertyChange(() => GameViewModel);
            }
        }

        /// <summary>
        /// </summary>
        public BindableCollection<Size> Resolutions
        {
            get
            {
                return
                    new BindableCollection<Size>(
                        new[]
                            {
                                new Size(640, 480), new Size(800, 600), new Size(1024, 768), new Size(1280, 720), 
                                new Size(1600, 900), new Size(1680, 1050), new Size(1920, 1080)
                            });
            }
        }

        /// <summary>
        /// </summary>
        public Size SelectedResolution
        {
            get
            {
                return _selectedResolution;
            }

            set
            {
                _selectedResolution = value;
                if (GameViewModel != null)
                {
                    GameViewModel.SetResolution(value.Width, value.Height);
                }

                NotifyOfPropertyChange(() => SelectedResolution);
            }
        }

        /// <summary>
        /// Handles the <see cref="AddTimeLineItemMessage"/>
        /// </summary>
        /// <param name="message">
        /// The add time line message.
        /// </param>
        public void Handle(AddTimeLineItemMessage message)
        {
            TimeLineItemViewModel timeLineItem = message.TimeLineItemViewModel;
            if (message.Player == 1)
            {
                PlayerOneTimeLineViewModel.AddTimeLineItem(timeLineItem);
            }
            else
            {
                PlayerTwoTimeLineViewModel.AddTimeLineItem(timeLineItem);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public void Handle(OpenTimeLineMessage message)
        {
            if (message.Player == 1)
            {
                PlayerOneTimeLineViewModel.OpenTimeLine(message.FilePath, message.Append);
            }
            else
            {
                PlayerTwoTimeLineViewModel.OpenTimeLine(message.FilePath, message.Append);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public void Handle(SaveTimeLineMessage message)
        {
            if (message.Player == 1)
            {
                PlayerOneTimeLineViewModel.SaveTimeLine();
            }
            else
            {
                PlayerTwoTimeLineViewModel.SaveTimeLine();
            }
        }

        /// <summary>Starts playback for the time line.</summary>
        public void PlaybackStart()
        {
            _events.Publish(new FocusStreetFighterMessage());
            try
            {
                _events.Publish(
                    new PlayTimeLineMessage(PlayerOneTimeLineViewModel, PlayerTwoTimeLineViewModel, RepeatAmount));
            }
            catch (Exception ex)
            {
                Execute.OnUIThread(
                    () =>
                    MessageBox.Show(
                        Application.Current.MainWindow, 
                        string.Format("The game crashed: {0} !", ex.Message), 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error));
            }
        }

        /// <summary>
        /// </summary>
        public void StartSF4()
        {
            IsBusy = true;
            if (GameViewModel == null)
            {
                GameViewModel = new GameViewModel(_events);
            }
            else
            {
                GameViewModel.TryClose();
                GameViewModel = null;
                GameViewModel = new GameViewModel(_events);
            }

            IsBusy = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="events">
        /// </param>
        public ComboTrainerViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);

            _isBusy = false;

            PlayerOneTimeLineEnabled = true;
            PlayerTwoTimeLineEnabled = false;

            PlayerOneTimeLineViewModel = new TimeLineViewModel(events);
            PlayerTwoTimeLineViewModel = new TimeLineViewModel(events);
        }

        /// <summary>
        /// </summary>
        public ComboTrainerViewModel()
            : this(new EventAggregator())
        {
        }
    }
}