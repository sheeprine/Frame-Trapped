namespace FrameTrapped.ComboTrainer.Messages
{
    using System;

    public class SaveTimeLineMessage
    {
        public int Player { get; private set; }

        public SaveTimeLineMessage(int player)
        {
            Player = player;
        }
    }
}
