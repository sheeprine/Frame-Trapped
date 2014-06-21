namespace FrameTrapped.ComboTrainer.Messages
{
    using FrameTrapped.ComboTrainer.ViewModels;

    public class AddTimeLineItemMessage
    {
        public int Player { get; private set; }

        public TimeLineItemViewModel TimeLineItemViewModel { get; private set; }

        public AddTimeLineItemMessage(TimeLineItemViewModel timeLineItemViewModel, int player)
        {
            TimeLineItemViewModel = timeLineItemViewModel;
            Player = player;
        }
    }
}
