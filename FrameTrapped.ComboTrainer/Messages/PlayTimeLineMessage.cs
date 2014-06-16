namespace FrameTrapped.ComboTrainer.Messages
{
    using System.Collections.Generic;
    using FrameTrapped.ComboTrainer.ViewModels;

    public class PlayTimeLineMessage
    {
        public IEnumerable<TimeLineItemViewModel> PlayerOneTimeLineItemViewModels { get; set; }
        public IEnumerable<TimeLineItemViewModel> PlayerTwoTimeLineItemViewModels { get; set; }

        public PlayTimeLineMessage(IEnumerable<TimeLineItemViewModel> playerOneTimeLineItemViewModels)
        {
            PlayerOneTimeLineItemViewModels = playerOneTimeLineItemViewModels;
            PlayerTwoTimeLineItemViewModels = null;
        }

        public PlayTimeLineMessage(IEnumerable<TimeLineItemViewModel> playerOneTimeLineItemViewModels, IEnumerable<TimeLineItemViewModel> playerTwoTimeLineItemViewModels)
        {
            PlayerOneTimeLineItemViewModels = playerOneTimeLineItemViewModels;
            PlayerTwoTimeLineItemViewModels = playerTwoTimeLineItemViewModels;
        }

    }
}
