namespace FrameTrapped.StreetFighterLibrary.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    using Caliburn.Micro;

    public class MoveListViewModel : BindableCollection<MoveViewModel>
    {
        private MoveViewModel _selectedMove;

        public MoveViewModel SelectedMove
        {
            get
            {
                return _selectedMove;
            }

            set
            {
                _selectedMove = value;
                NotifyOfPropertyChange(string.Empty);
            }
        }

        public void Group()
        {
            ICollectionView collection = CollectionViewSource.GetDefaultView(this);
            if (collection != null && collection.CanGroup == true)
            {
                collection.GroupDescriptions.Clear();
                collection.GroupDescriptions.Add(new PropertyGroupDescription("MoveType"));
                //collection.GroupDescriptions.Add(new PropertyGroupDescription("BlockType"));
            }

            NotifyOfPropertyChange(string.Empty);
        }

        public void Ungroup()
        {
            ICollectionView collection = CollectionViewSource.GetDefaultView(this);
            if (collection != null)
            {
                collection.GroupDescriptions.Clear();
            }
        }
    }
}
