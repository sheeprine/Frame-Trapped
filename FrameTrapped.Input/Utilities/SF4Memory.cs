namespace FrameTrapped.Input.Utilities
{
    using System;

    using FrameTrapped.Input.Utilities.MemoryEditor;
     
    /**
     * this class reads data from the memory of a running sf4 instance.
     * atm it only reads the current framecount and the x positions of both players.
     * other adresses are
     * 
     * INFO         TYPE        ADDRESS     OFFSETS
     * --------------------------------------------------
     * p1posY       float       0x80f0cc    8, 100
     * p1exmeter    int         0x80f0cc    8, 0x6c64
     * p1health     int         0x80f0cc    8, 0x6c5c
     * p1recovery   int         0x80f0cc    8, 160, 220
     * p1ultra      int         0x80f0cc    8, 0x6c78
     * p2posY       float       0x80f0cc    12, 100
     * p2exmeter    int         0x80f0cc    12, 0x6c64
     * p2health     int         0x80f0cc    12, 0x6c5c
     * p2ultra      int         0x80f0cc    12, 0x6c78
     * 
     * i haven't tested these other adresses they may or may not work
     * if they don't try removing the added steamVersionMemoryOffset
     */
    public class SF4Memory
    {
        private MemoryReader memory = new MemoryReader();
        
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
            return Convert.ToInt32(this.memory.ReadInt(this.memory.BaseAddress() + address ));
        }

        private int readIntFromGameMemory(int address, int[] offsets)
        {
            return Convert.ToInt32(this.memory.ReadInt(this.memory.BaseAddress() + address, offsets));
        }

        private float readFloatFromGameMemory(int address, int[] offsets)
        {
            return this.memory.ReadFloat(this.memory.BaseAddress() + address, offsets);
        }

        private byte[] readMemoryAOB(int address, int[] offsets, uint bytestoread)
        {
            return this.memory.ReadAOB(this.memory.BaseAddress() + address , offsets, bytestoread);
        }
        
        
        /* //Old one. It only works when stage quality = high
        public int GetFrameCount()
        {
            return readIntFromGameMemory(0x80f0d0, new int[] { 0x1c4 });
        }
        */

        public int GetFrameCount() //I have not tested this much. Don't know how stable it is. Address found by toolassistedabel AKA abeltech. Should work on both high and low stage quality settings.
        {
            return readIntFromGameMemory(0x687E90, new int[] { 0x28 });
        }

        public float GetP1PosX()
        {
            return readFloatFromGameMemory(0x687E6C, new int[] { 0x8, 0x70 });
        }

        public float GetP2PosX()
        {
            return readFloatFromGameMemory(0x687E6C, new int[] { 0xC, 0x70 });
        }

        public int GetComboCounter()                  //There is only one combo counter. It's the same for both players. Will increase with 1 for each hit in a combo.
        {
            return readIntFromGameMemory(0x687E70, new int[] { 0x130 });
        }

        public int GetPlayerScript(int player) //Returns animation number (script number).
        {
            int address = 0x687E6C;
            int action = -1;
            if (player == 1)
                action = readIntFromGameMemory(address, new int[] { 0x8, 0xB0, 0x18 });
            else
                action = readIntFromGameMemory(address, new int[] { 0xC, 0xB0, 0x18 });
            return action;
        }

        public int GetAnimationFrame(int player) //Returns which frame the animation is on
        {
            int address = 0x687E6C;
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
                characterI = readIntFromGameMemory(0x689C6C);
            }
            else if (player == 2)
            {
                characterI = readIntFromGameMemory(0x689C8C);
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

    }
}
