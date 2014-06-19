namespace FrameTrapped.Input.Utilities
{
    using System;
    using System.Diagnostics;


    using FrameTrapped.Input.Utilities.MemoryEditor;
    using FrameTrapped.Input.Utilities.SignatureScanner;

     
    /// <summary>
    ///  This class reads data from the memory of a running sf4 instance.
    /// </summary>
    public class SF4Memory
    {
        /// <summary>
        /// The static singletone instance of <see cref="SF4Memory"/>.
        /// </summary>
        private static SF4Memory instance;

        private MemoryReader memory = new MemoryReader();

        private SigScan SigScan = new SigScan();

        private int playerBase = 0;
        private int frameCounterBase = 0;
        private int comboCounterBase = 0;

        public void runScan()
        {
            byte[] pattern = new byte[] { 
                0x84, 0x05, 0xE0, 0xA2, 0xA8, 0x00, 0x75, 0x24, 0x09, 
                0x05, 0x00, 0x00, 0x00, 0x00, 0xB9, 0x00, 0x00, 0x00, 
                0x00, 0xC7, 0x45, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE8, 
                0x00, 0x00, 0x00, 0x00, 0x68, 0x00, 0x00, 0x00, 0x00, 
                0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x04, 0xB8, 
                0x00, 0x00, 0x00, 0x00, 0x8B, 0x4D, 0xF4, 0x64, 0x89, 
                0x0D, 0x00, 0x00, 0x00, 0x00, 0x59, 0x8B, 0xE5, 0x5D };   //This is the pattern we use to find the base of what we want in memory. Add 0x5c to it to get to the players.

            string mask = "xxx?xxxxxx????x????xx?????x????x????x????xxxx????xxxxxx????xxxx";

            this.SigScan.Address = new IntPtr(0x400000);
            this.SigScan.Size = 0x900000; //This should be more than enough.
            IntPtr address = IntPtr.Zero;

            try
            {
                SigScan.Process = Process.GetProcessesByName("SSFIV")[0];
                address = SigScan.FindPattern(pattern, mask, 0x2d);
            }
            catch
            {
                //Error message here. (Couldn't find the process)
            }
            SigScan.ResetRegion();

            if (address != IntPtr.Zero)
            {
                openSF4Process();
                playerBase = readIntFromGameMemory(address.ToInt32()) + 0x5c;
                frameCounterBase = playerBase + 0x24;
                comboCounterBase = playerBase + 0x4;
            }
            else
            {
                //Error message here. (Probably wrong pattern/mask)
            }
        
        }

        public bool openSF4Process()
        {
            if (this.memory.OpenProcess("SSFIV"))
            {
                return true;
            }
            else
            {
                return false;
            }     
        }

        private int readIntFromGameMemory(int address)
        {
            return Convert.ToInt32(this.memory.ReadInt(address ));
        }

        private int readIntFromGameMemory(int address, int[] offsets)
        {
            return Convert.ToInt32(this.memory.ReadInt(address, offsets));
        }

        private float readFloatFromGameMemory(int address, int[] offsets)
        {
            return this.memory.ReadFloat(address, offsets);
        }

        private byte[] readMemoryAOB(int address, int[] offsets, uint bytestoread)
        {
            return this.memory.ReadAOB(address , offsets, bytestoread);
        }

        public int GetFrameCount()
        {
            return readIntFromGameMemory(frameCounterBase, new int[] { 0x28 });
        }

        public float GetP1PosX()
        {
            return readFloatFromGameMemory(playerBase, new int[] { 0x8, 0x70 });
        }

        public float GetP2PosX()
        {
            return readFloatFromGameMemory(playerBase, new int[] { 0xC, 0x70 });
        }

        /// <summary>
        /// Gets the combo counter.
        /// </summary>
        /// <remarks>There is only one combo counter. It's the same for both players. Will increase with 1 for each hit in a combo.</remarks>
        public int GetComboCounter()
        {
            return readIntFromGameMemory(comboCounterBase, new int[] { 0x130 });
        }

        public int GetPlayerScript(int player) //Returns animation number (script number).
        {
            int address = playerBase;
            int action = -1;
            if (player == 1)
                action = readIntFromGameMemory(address, new int[] { 0x8, 0xB0, 0x18 });
            else
                action = readIntFromGameMemory(address, new int[] { 0xC, 0xB0, 0x18 });
            return action;
        }

        public int GetAnimationFrame(int player) //Returns which frame the animation is on
        {
            int address = playerBase;
            if (player == 1)
            {
                return System.BitConverter.ToInt16(readMemoryAOB(address, new int[] { 0x8, 0xB0, 0x1E }, 2), 0);
            }
            else
            {
                return System.BitConverter.ToInt16(readMemoryAOB(address, new int[] { 0xC, 0xB0, 0x1E }, 2), 0);
            }
        }

        public string GetCharacter(int player)                      //Find out who plays who.
        {
            string character = "Not available";
            int characterI = -1;

            if (player == 1)
            {
                characterI = readIntFromGameMemory(0x688398);
            }
            else if (player == 2)
            {
                characterI = readIntFromGameMemory(0x688650);
            }


            if (characterI == 0)
                character = "Ryu";
            else if (characterI == 1)
                character = "Ken";
            else if (characterI == 2)
                character = "Chun-Li";
            else if (characterI == 3)
                character = "E.Honda";
            else if (characterI == 4)
                character = "Blanka";
            else if (characterI == 5)
                character = "Zangief";
            else if (characterI == 6)
                character = "Guile";
            else if (characterI == 7)
                character = "Dhalsim";
            else if (characterI == 8)
                character = "Balrog";
            else if (characterI == 9)
                character = "Vega";
            else if (characterI == 10)
                character = "Sagat";
            else if (characterI == 11)
                character = "M.Bison";
            else if (characterI == 12)
                character = "C.Viper";
            else if (characterI == 13)
                character = "Rufus";
            else if (characterI == 14)
                character = "El Fuerte";
            else if (characterI == 15)
                character = "Abel";
            else if (characterI == 16)
                character = "Seth";
            else if (characterI == 17)
                character = "Akuma";
            else if (characterI == 18)
                character = "Gouken";
            else if (characterI == 19)
                character = "T.Hawk";
            else if (characterI == 20)
                character = "Cammy";
            else if (characterI == 21)
                character = "Fei Long";
            else if (characterI == 22)
                character = "Deejay";
            else if (characterI == 23)
                character = "Sakura";
            else if (characterI == 24)
                character = "Rose";
            else if (characterI == 25)
                character = "Gen";
            else if (characterI == 26)
                character = "Dan";
            else if (characterI == 27)
                character = "Guy";
            else if (characterI == 28)
                character = "Cody";
            else if (characterI == 29)
                character = "Ibuki";
            else if (characterI == 30)
                character = "Makoto";
            else if (characterI == 31)
                character = "Dudley";
            else if (characterI == 32)
                character = "Adon";
            else if (characterI == 33)
                character = "Hakan";
            else if (characterI == 34)
                character = "Juri";
            else if (characterI == 35)
                character = "Yun";
            else if (characterI == 36)
                character = "Yang";
            else if (characterI == 37)
                character = "Evil Ryu";
            else if (characterI == 38)
                character = "Oni";
            else if (characterI == -1)
                character = "Couldn't get character";

            return character;
        }

        /// <summary>
        /// Gets the static accessor for making a <see cref="SF4Memory" /> instance.
        /// </summary>
        public static SF4Memory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SF4Memory();
                }
                return instance;
            }
        }

    }
}
