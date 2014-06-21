namespace FrameTrapped.StreetFighterLibrary.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;

    public class ComboableMovesViewModel : Conductor<MoveViewModel>.Collection.OneActive
    {
        private MoveViewModel _baseMove;

        private MoveViewModel _selectedMove;

        public bool BaseMoveExists
        {
            get
            {
                return BaseMove != null;
            }
        }

        public MoveViewModel BaseMove
        {
            get
            {
                return _baseMove;
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
                NotifyOfPropertyChange(() => ComboableMoves);
            }
        }

        public BindableCollection<MoveViewModel> LinkableMoves
        {
            get
            {
                if (BaseMove != null)
                {
                    return new BindableCollection<MoveViewModel>
                        (
                            Items.Where
                            (
                                move => BaseMove.Hits.Last().OnHitAdvantage > move.Hits.First().Startup
                            )
                        );
                }
                return null;
            }
        }

        public BindableCollection<MoveViewModel> CancelableMoves
        {
            get
            {
                if (BaseMove != null)
                {

                    return new BindableCollection<MoveViewModel>
                        (
                            Items.Where
                            (
                            
                                move => BaseMove.Hits.SelectMany<HitViewModel, HitViewModel.CancelAbilityEnum>(
                                    hit =>  hit.CancelAbility.ToList()).Any(( cancel => cancel == move.CancelIntoType )
                                    )
                            )
                        );
                }
                return null;
            }
        }

        public ComboableMovesViewModel ComboableMoves
        {
            get
            {
                if (SelectedMove != null)
                {
                    return new ComboableMovesViewModel(Items, SelectedMove);
                }
                return null;
            }
        }

        public ComboableMovesViewModel(IEnumerable<MoveViewModel> moveList, MoveViewModel baseMove)
        {
            Items.AddRange(moveList);
            _baseMove = baseMove;
            NotifyOfPropertyChange(() => LinkableMoves);
        }

        public ComboableMovesViewModel(IEnumerable<MoveViewModel> moveList)
        {
            Items.AddRange(moveList);
        }
    }
}
