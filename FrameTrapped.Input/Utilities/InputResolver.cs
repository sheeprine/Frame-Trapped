
namespace FrameTrapped.Input.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using WindowsInput;

    using FrameTrapped.Input.Models;
    using System.Reflection;
    using System.IO;
    using FrameTrapped.Input.Properties;
    using System.Configuration;
    using System.Diagnostics;

    /**
     * this class translates our custom Input enum to actual keyboard codes that are used by the input simulator.
     * the main reason for this class is the resolving of back and forward since the Directions change when a player crosses up.
     * it also reads the keyboard config file sf4keyboard.cfg
     */
    public class InputResolver
    {
        private SF4Memory _sf4m;

        private static InputResolver _instance;

        public static InputResolver Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InputResolver();
                }
                return _instance;
            }
        }

        public List<WindowsInput.VirtualKeyCode> InputMap = new List<VirtualKeyCode>();

        public Dictionary<Input, WindowsInput.VirtualKeyCode> PlayerOneInputMap = new Dictionary<Input, VirtualKeyCode>();
        public Dictionary<Input, WindowsInput.VirtualKeyCode> PlayerTwoInputMap = new Dictionary<Input, VirtualKeyCode>();

        private void readInputConfig()
        {
            string inputFolder = Path.Combine(Environment.SpecialFolder.MyDocuments), @"CAPCOM\SUPERSTREETFIGHTERIV\input");
            byte[] bytes;

            bytes = File.ReadAllBytes(Path.Combine(inputFolder, @"KEYBOARD_DEVICE#0.ctrlsav"));

            PlayerOneInputMap.Add(Input.Up, (VirtualKeyCode)bytes[192]);
            PlayerOneInputMap.Add(Input.Down, (VirtualKeyCode)bytes[200]);
            PlayerOneInputMap.Add(Input.Left, (VirtualKeyCode)bytes[208]);
            PlayerOneInputMap.Add(Input.Right, (VirtualKeyCode)bytes[216]);

            PlayerOneInputMap.Add(Input.LightPunch, (VirtualKeyCode)bytes[224]);
            PlayerOneInputMap.Add(Input.MediumPunch, (VirtualKeyCode)bytes[232]);
            PlayerOneInputMap.Add(Input.HardPunch, (VirtualKeyCode)bytes[240]);
            PlayerOneInputMap.Add(Input.LightKick, (VirtualKeyCode)bytes[248]);
            PlayerOneInputMap.Add(Input.MediumKick, (VirtualKeyCode)bytes[256]);
            PlayerOneInputMap.Add(Input.HardKick, (VirtualKeyCode)bytes[264]);

            InputMap.Add((VirtualKeyCode)bytes[192]);
            InputMap.Add((VirtualKeyCode)bytes[200]);
            InputMap.Add((VirtualKeyCode)bytes[208]);
            InputMap.Add((VirtualKeyCode)bytes[216]);

            InputMap.Add((VirtualKeyCode)bytes[224]);
            InputMap.Add((VirtualKeyCode)bytes[232]);
            InputMap.Add((VirtualKeyCode)bytes[240]);
            InputMap.Add((VirtualKeyCode)bytes[248]);
            InputMap.Add((VirtualKeyCode)bytes[256]);
            InputMap.Add((VirtualKeyCode)bytes[264]);

            bytes = File.ReadAllBytes(Path.Combine(inputFolder, @"KEYBOARD_DEVICE#1.ctrlsav"));

            PlayerTwoInputMap.Add(Input.Up, (VirtualKeyCode)bytes[192]);
            PlayerTwoInputMap.Add(Input.Down, (VirtualKeyCode)bytes[200]);
            PlayerTwoInputMap.Add(Input.Left, (VirtualKeyCode)bytes[208]);
            PlayerTwoInputMap.Add(Input.Right, (VirtualKeyCode)bytes[216]);

            PlayerTwoInputMap.Add(Input.LightPunch, (VirtualKeyCode)bytes[224]);
            PlayerTwoInputMap.Add(Input.MediumPunch, (VirtualKeyCode)bytes[232]);
            PlayerTwoInputMap.Add(Input.HardPunch, (VirtualKeyCode)bytes[240]);
            PlayerTwoInputMap.Add(Input.LightKick, (VirtualKeyCode)bytes[248]);
            PlayerTwoInputMap.Add(Input.MediumKick, (VirtualKeyCode)bytes[256]);
            PlayerTwoInputMap.Add(Input.HardKick, (VirtualKeyCode)bytes[264]);

            InputMap.Add((VirtualKeyCode)bytes[192]);
            InputMap.Add((VirtualKeyCode)bytes[200]);
            InputMap.Add((VirtualKeyCode)bytes[208]);
            InputMap.Add((VirtualKeyCode)bytes[216]);

            InputMap.Add((VirtualKeyCode)bytes[224]);
            InputMap.Add((VirtualKeyCode)bytes[232]);
            InputMap.Add((VirtualKeyCode)bytes[240]);
            InputMap.Add((VirtualKeyCode)bytes[248]);
            InputMap.Add((VirtualKeyCode)bytes[256]);
            InputMap.Add((VirtualKeyCode)bytes[264]);
        }

        private VirtualKeyCode ResolvePlayerBackwardInput(int player)
        {
            if (player == 1)
            {
                return (_sf4m.GetP1PosX() < _sf4m.GetP2PosX()) ? Get(Input.Left, player) : Get(Input.Right, player);
            }
            else
            {
                return (_sf4m.GetP1PosX() > _sf4m.GetP2PosX()) ? Get(Input.Left, player) : Get(Input.Right, player);
            }
        }

        private VirtualKeyCode ResolvePlayerForwardInput(int player)
        {
            if (player == 1)
            {
                return (_sf4m.GetP1PosX() > _sf4m.GetP2PosX()) ? Get(Input.Left, player) : Get(Input.Right, player);
            }
            else
            {
                return (_sf4m.GetP1PosX() < _sf4m.GetP2PosX()) ? Get(Input.Left, player) : Get(Input.Right, player);
            }
        }

        public bool IsPlayerOneOnLeft()
        {
            return (_sf4m.GetP1PosX() < _sf4m.GetP2PosX());
        }

        public VirtualKeyCode Get(Input input, int player)
        {
            if (input == Input.Back)
            {
                return ResolvePlayerBackwardInput(player);
            }
            else if (input == Input.Forward)
            {
                return ResolvePlayerForwardInput(player);
            }
            else if (PlayerOneInputMap.ContainsKey(input))
            {
                if (player == 1)
                {
                    return PlayerOneInputMap[input];
                }
                else
                {
                    return PlayerTwoInputMap[input];
                }
            }
            else
            {
                throw new KeyNotFoundException("Cannot find input key for " + input);
            }
        }

        private InputResolver()
        {
            this._sf4m = SF4Memory.Instance;

            readInputConfig();
        }
    }
}
