﻿namespace FrameTrapped.ComboTrainer.ViewModels
{
    using System;
    using System.Drawing;
    using Caliburn.Micro;
    using FrameTrapped.ComboTrainer.Messages;

    public class ComboTrainerViewModel : Screen,
        IHandle<AddTimeLineItemMessage>,
        IHandle<OpenTimeLineMessage>,
        IHandle<SaveTimeLineMessage>
    {
        private IEventAggregator _events;

        private bool _isBusy;

        private bool _repeat;

        private int _repeatAmount;

        private bool _playerOneTimeLineEnabled;

        private bool _playerTwoTimeLineEnabled;

        private TimeLineViewModel _playerOneTimeLineViewModel;

        private TimeLineViewModel _playerTwoTimeLineViewModel;

        private GameViewModel _gameViewModel;

        private Size _selectedResolution;

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

        public BindableCollection<Size> Resolutions
        {
            get
            {
                // silly example of the collection to bind to
                return new BindableCollection<Size>(
                    new Size[] 
                    { 
                        new Size(640, 480),
                        new Size(800, 600),
                        new Size(1024, 768),
                        new Size(1280, 720),
                        new Size(1600, 900),
                        new Size(1680, 1050),
                        new Size(1920, 1080)
                    });
            }
        }

        /// <summary>
        /// Starts playback for the time line.
        /// </summary>
        public void PlaybackStart()
        {
            _events.Publish(new FocusStreetFighterMessage());
            try
            {
               _events.Publish(
                    new PlayTimeLineMessage(
                        PlayerOneTimeLineViewModel,
                        PlayerTwoTimeLineViewModel,
                        RepeatAmount));
            }
            catch(Exception ex)
            {
                Execute.OnUIThread(() =>
                               System.Windows.MessageBox.Show(
                               System.Windows.Application.Current.MainWindow,
                               string.Format("The game crashed: {0} !", ex.Message),
                               "Error",
                               System.Windows.MessageBoxButton.OK,
                               System.Windows.MessageBoxImage.Error));
                }
        }

        public Size SelectedResolution
        {
            get { return _selectedResolution; }
            set
            {
                _selectedResolution = value;
                if (GameViewModel != null){
                    GameViewModel.SetResolution(value.Width, value.Height);
                }
                NotifyOfPropertyChange(() => SelectedResolution);
            }
        }

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
        /// Handles the <see cref="AddTimeLineItemMessage"/>
        /// </summary>
        /// <param name="message">The add time line message.</param>
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

        public ComboTrainerViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);

            _isBusy = false;

            PlayerOneTimeLineEnabled = false;
            PlayerTwoTimeLineEnabled = false;

            PlayerOneTimeLineViewModel = new TimeLineViewModel(events);
            PlayerTwoTimeLineViewModel = new TimeLineViewModel(events);
        }

        public ComboTrainerViewModel() :
            this(new EventAggregator()) { }

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
    }
}
