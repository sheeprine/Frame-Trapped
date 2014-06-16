namespace FrameTrapped.ComboTrainer.ViewModels
{
    using System;
    using System.Drawing;
    using Caliburn.Micro;
    using FrameTrapped.ComboTrainer.Messages;
    using FrameTrapped.ComboTrainer.Utilities;

    public class ComboTrainerViewModel : Screen
    {
        private IEventAggregator _events;

        private TimeLineViewModel _playerOneTimeLineViewModel;

        private TimeLineViewModel _playerTwoTimeLineViewModel;

        private GameViewModel _gameViewModel;

        private Size _selectedResolution;

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
            _events.Publish(new PlayTimeLineMessage(PlayerOneTimeLineViewModel.TimeLineItems, PlayerTwoTimeLineViewModel.TimeLineItems));
        }

        public Size SelectedResolution
        {
            get { return _selectedResolution; }
            set
            {
                _selectedResolution = value;
                GameViewModel.SetResolution(value.Width, value.Height);
                NotifyOfPropertyChange(() => SelectedResolution); 
            }
        }
        
        public void StartSF4()
        {
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
        }

        ~ComboTrainerViewModel()
        {
            try
            {
                SF4ProcessHandler.Instance.StopSF4();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public ComboTrainerViewModel(IEventAggregator events)
        {
            _events = events;

            PlayerOneTimeLineViewModel = new TimeLineViewModel(events);
            PlayerTwoTimeLineViewModel = new TimeLineViewModel(events);
        }
    }
}
