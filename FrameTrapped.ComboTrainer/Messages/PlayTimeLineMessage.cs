namespace FrameTrapped.ComboTrainer.Messages
{
    using System.Collections.Generic;
    using FrameTrapped.ComboTrainer.ViewModels;

    public class PlayTimeLineMessage
    {
        public TimeLineViewModel PlayerOneTimeLineItemViewModels { get; set; }
        public TimeLineViewModel PlayerTwoTimeLineItemViewModels { get; set; }
        public int RepeatAmount { get; private set; }

        public PlayTimeLineMessage(TimeLineViewModel playerOneTimeLineViewModels,
            TimeLineViewModel playerTwoTimeItemViewModels,
            int repeatAmount)
        {
            PlayerOneTimeLineItemViewModels = playerOneTimeLineViewModels;
            PlayerTwoTimeLineItemViewModels = playerTwoTimeItemViewModels;
            RepeatAmount = repeatAmount;
        }
    }
}
