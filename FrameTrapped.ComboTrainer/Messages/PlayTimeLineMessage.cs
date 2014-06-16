namespace FrameTrapped.ComboTrainer.Messages
{
    using System.Collections.Generic;
    using FrameTrapped.ComboTrainer.ViewModels;

    public class PlayTimeLineMessage
    {
        public IEnumerable<TimeLineItemViewModel> PlayerOneTimeLineItemViewModels { get; private set; }
        public IEnumerable<TimeLineItemViewModel> PlayerTwoTimeLineItemViewModels { get; private set; }
        public int RepeatAmount { get; private set; }

        public PlayTimeLineMessage(IEnumerable<TimeLineItemViewModel> playerOneTimeLineItemViewModels)
        {
            PlayerOneTimeLineItemViewModels = playerOneTimeLineItemViewModels;
            PlayerTwoTimeLineItemViewModels = null;
        }

        public PlayTimeLineMessage(IEnumerable<TimeLineItemViewModel> playerOneTimeLineItemViewModels,
            IEnumerable<TimeLineItemViewModel> playerTwoTimeLineItemViewModels,
            int repeatAmount)
        {
            PlayerOneTimeLineItemViewModels = playerOneTimeLineItemViewModels;
            PlayerTwoTimeLineItemViewModels = playerTwoTimeLineItemViewModels;
            RepeatAmount = repeatAmount;
        }

    }
}
