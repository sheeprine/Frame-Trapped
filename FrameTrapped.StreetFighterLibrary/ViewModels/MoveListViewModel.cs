namespace FrameTrapped.StreetFighterLibrary.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    using Caliburn.Micro;

    public class MoveListViewModel : Conductor<MoveViewModel>.Collection.OneActive
    {
        private MoveViewModel _selectedMove;

        private bool _movesGrouped;

        public bool MovesGrouped
        {
            get
            {
                return _movesGrouped;
            }

            set
            {
                _movesGrouped = value;
                NotifyOfPropertyChange(() => MovesGrouped);
            }
        }
        
        public MoveViewModel SelectedMove
        {
            get
            {
                return _selectedMove;
            }

            set
            {
                _selectedMove = value;
                NotifyOfPropertyChange(() => SelectedMove);
            }
        }

        public void Add(MoveViewModel move)
        {
            Items.Add(move);
        }

        public void ToggleGroup()
        {
            ICollectionView collection = CollectionViewSource.GetDefaultView(this.Items);
            if (collection != null && collection.CanGroup == true)
            {
                collection.GroupDescriptions.Clear();
                if (MovesGrouped)
                {
                    collection.GroupDescriptions.Add(new PropertyGroupDescription("MoveType"));
                }
            }

            NotifyOfPropertyChange(string.Empty);
        }
    }
}
