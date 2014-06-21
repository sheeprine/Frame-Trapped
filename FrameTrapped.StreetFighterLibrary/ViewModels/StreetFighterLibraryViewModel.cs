namespace FrameTrapped.StreetFighterLibrary.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Caliburn.Micro;
    using FrameTrapped.ComboTrainer.Messages;
    using FrameTrapped.ComboTrainer.ViewModels;
    using FrameTrapped.Input.ViewModels;
    using FrameTrapped.StreetFighterLibrary.Utilities;

    public class StreetFighterLibraryViewModel : Screen
    {
        /// <summary>
        /// The events aggregator.
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// The title.
        /// </summary>
        private string _title;

        /// <summary>
        /// The currently selected fighter.
        /// </summary>
        private FighterViewModel _selectedFighter;

        /// <summary>
        /// Is the move list tab item selected?
        /// </summary>
        private bool _moveListTabItemSelected;

        /// <summary>
        /// Is the linkable moves tab item selected?
        /// </summary>
        private bool _linkableMovesTabItemSelected;
        
        /// <summary>
        /// Gets or sets window title.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        /// <summary>
        /// Gets the home view model.
        /// </summary>
        public ObservableCollection<FighterViewModel> FightersList { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected fighter.
        /// </summary>
        public FighterViewModel SelectedFighter
        {
            get
            {
                return _selectedFighter;
            }
            set
            {
                if (value != _selectedFighter)
                {
                    _selectedFighter = value;
                    NotifyOfPropertyChange(() => SelectedFighter);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the home tab item is selected.
        /// </summary>
        public bool MoveListTabItemSelected
        {
            get
            {
                return _moveListTabItemSelected;
            }

            set
            {
                _moveListTabItemSelected = value;
                NotifyOfPropertyChange(() => MoveListTabItemSelected);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the home tab item is selected.
        /// </summary>
        public bool LinkableMovesTabItemSelected
        {
            get
            {
                return _linkableMovesTabItemSelected;
            }

            set
            {
                _linkableMovesTabItemSelected = value;
                NotifyOfPropertyChange(() => LinkableMovesTabItemSelected);
            }
        } 
        /// <summary>
        /// Add command to time line.
        /// </summary>
        /// <param name="moveViewModel">The move view model containing the moves.</param>
        public void AddCommandToTimeline(MoveViewModel moveViewModel, int player)
        {
            foreach (InputItemViewModel inputItemViewModel in moveViewModel.Command.Commands)
            {
                TimeLineItemViewModel timeLineItemViewModel = new TimeLineItemViewModel();
                timeLineItemViewModel.InputItemViewModel = inputItemViewModel;
                _events.Publish(new AddTimeLineItemMessage(timeLineItemViewModel, player));
            }
        }

        public StreetFighterLibraryViewModel(IEventAggregator events)
        {
            _events = events;
            FighterDataAE2012.Events = _events;

            FightersList = new ObservableCollection<FighterViewModel>();

            // First Row
            FightersList.Add(FighterDataAE2012.Ryu());
            FightersList.Add(FighterDataAE2012.Ken());
            FightersList.Add(FighterDataAE2012.EHonda());
            FightersList.Add(FighterDataAE2012.Ibuki());
            FightersList.Add(FighterDataAE2012.Makoto());
            FightersList.Add(FighterDataAE2012.Dudley());
            FightersList.Add(FighterDataAE2012.Seth());
            FightersList.Add(FighterDataAE2012.Gouken());
            FightersList.Add(FighterDataAE2012.Akuma());
            FightersList.Add(FighterDataAE2012.Gen());
            FightersList.Add(FighterDataAE2012.Dan());
            FightersList.Add(FighterDataAE2012.Sakura());
            FightersList.Add(FighterDataAE2012.Oni());

            // Second Row
            FightersList.Add(FighterDataAE2012.Yun());
            FightersList.Add(FighterDataAE2012.Juri());
            FightersList.Add(FighterDataAE2012.ChunLi());
            FightersList.Add(FighterDataAE2012.Dhalsim());
            FightersList.Add(FighterDataAE2012.Abel());
            FightersList.Add(FighterDataAE2012.CViper());
            FightersList.Add(FighterDataAE2012.MBison());
            FightersList.Add(FighterDataAE2012.Sagat());
            FightersList.Add(FighterDataAE2012.Cammy());
            FightersList.Add(FighterDataAE2012.DeeJay());
            FightersList.Add(FighterDataAE2012.Cody());
            FightersList.Add(FighterDataAE2012.Guy());
            FightersList.Add(FighterDataAE2012.Hakan());
            FightersList.Add(FighterDataAE2012.Yang());

            //Third Row
            FightersList.Add(FighterDataAE2012.EvilRyu());
            FightersList.Add(FighterDataAE2012.Guile());
            FightersList.Add(FighterDataAE2012.Blanka());
            FightersList.Add(FighterDataAE2012.Zangief());
            FightersList.Add(FighterDataAE2012.Rufus());
            FightersList.Add(FighterDataAE2012.ElFuerte());
            FightersList.Add(FighterDataAE2012.Vega());
            FightersList.Add(FighterDataAE2012.Balrog());
            FightersList.Add(FighterDataAE2012.FeiLong());
            FightersList.Add(FighterDataAE2012.THawk());
            FightersList.Add(FighterDataAE2012.Adon());
            FightersList.Add(FighterDataAE2012.Rose());

            MoveListTabItemSelected = true;
        }
    }
}
