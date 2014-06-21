namespace FrameTrapped.ComboTrainer.Messages
{
    using System;

    public class OpenTimeLineMessage
    {
        public bool Append { get; private set; }

        public string FilePath { get; private set; }

        public int Player { get; private set; }

        public OpenTimeLineMessage(string filePath, int player, bool append)
        {
            FilePath = filePath;
            Player = player;
            Append = append;
        }

        public OpenTimeLineMessage(string filePath, int player)
        {
            FilePath = filePath;
            Player = player;
            Append = false;
        }
    }
}
