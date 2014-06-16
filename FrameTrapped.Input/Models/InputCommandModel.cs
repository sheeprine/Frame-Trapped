
namespace FrameTrapped.Input.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using FrameTrapped.Input.Utilities;

    public enum Input
    {
        Up,
        Down,
        Left,
        Right,
        Back,
        Forward,

        LightPunch,
        MediumPunch,
        HardPunch,

        LightKick,
        MediumKick,
        HardKick
    }

    public class InputCommandModel
    {
        public enum DirectionStateEnum
        {
            DownBack = 1,
            Down = 2,
            DownForward = 3,

            Back = 4,
            Neutral = 5,
            Forward = 6,

            UpBack = 7,
            Up = 8,
            UpForward = 9
        }

        public enum ButtonStateEnum
        {
            Pressed = 1,
            Released = 0
        }


        public DirectionStateEnum DirectionState;

        public ButtonStateEnum LightPunch;
        public ButtonStateEnum MediumPunch;
        public ButtonStateEnum HardPunch;

        public ButtonStateEnum LightKick;
        public ButtonStateEnum MediumKick;
        public ButtonStateEnum HardKick;

        public bool NonePressed
        {
            get
            {
                return
                    DirectionState == DirectionStateEnum.Neutral &&
                    LightPunch == ButtonStateEnum.Released &&
                    MediumPunch == ButtonStateEnum.Released &&
                    HardPunch == ButtonStateEnum.Released &&
                    LightKick == ButtonStateEnum.Released &&
                    MediumKick == ButtonStateEnum.Released &&
                    HardKick == ButtonStateEnum.Released;
            }
        }

        /// <summary>
        /// Converts the current SF4InputState into an InputsArray of pressed buttons.
        /// </summary>
        /// <returns><see cref="Input"/> array.</returns>
        public Input[] ToInputsPressedArray()
        {
            List<Input> inputsArray = new List<Input>();
            switch (DirectionState)
            {
                case DirectionStateEnum.UpBack:
                    inputsArray.Add(Input.Up);
                    inputsArray.Add(Input.Back);
                    break;
                case DirectionStateEnum.Up:
                    inputsArray.Add(Input.Up);
                    break;
                case DirectionStateEnum.UpForward:
                    inputsArray.Add(Input.Up);
                    inputsArray.Add(Input.Forward);
                    break;
                case DirectionStateEnum.Back:
                    inputsArray.Add(Input.Back);
                    break;
                case DirectionStateEnum.Neutral:
                    // Do nothing - Placeholder for later.
                    break;
                case DirectionStateEnum.Forward:
                    inputsArray.Add(Input.Forward);
                    break;
                case DirectionStateEnum.DownBack:
                    inputsArray.Add(Input.Down);
                    inputsArray.Add(Input.Back);
                    break;
                case DirectionStateEnum.Down:
                    inputsArray.Add(Input.Down);
                    break;
                case DirectionStateEnum.DownForward:
                    inputsArray.Add(Input.Forward);
                    inputsArray.Add(Input.Down);
                    break;
            }

            if (LightPunch == ButtonStateEnum.Pressed) inputsArray.Add(Input.LightPunch);
            if (MediumPunch == ButtonStateEnum.Pressed) inputsArray.Add(Input.MediumPunch);
            if (HardPunch == ButtonStateEnum.Pressed) inputsArray.Add(Input.HardPunch);
            if (LightKick == ButtonStateEnum.Pressed) inputsArray.Add(Input.LightKick);
            if (MediumKick == ButtonStateEnum.Pressed) inputsArray.Add(Input.MediumKick);
            if (HardKick == ButtonStateEnum.Pressed) inputsArray.Add(Input.HardKick);

            return inputsArray.ToArray();
        }


        /// <summary>
        /// Converts the current SF4InputState into an InputsArray of released buttons.
        /// </summary>
        /// <returns><see cref="Input"/> array.</returns>
        public Input[] ToInputsReleasedArray()
        {
            List<Input> inputsArray = new List<Input>();

            inputsArray.Add(Input.Up);
            inputsArray.Add(Input.Down );
            inputsArray.Add(Input.Back);
            inputsArray.Add(Input.Forward);

            switch (DirectionState)
            {
                case DirectionStateEnum.UpBack:
                    inputsArray.Remove(Input.Up);
                    inputsArray.Remove(Input.Back);
                    break;
                case DirectionStateEnum.Up:
                    inputsArray.Remove(Input.Up);
                    break;
                case DirectionStateEnum.UpForward:
                    inputsArray.Remove(Input.Up);
                    inputsArray.Remove(Input.Forward);
                    break;
                case DirectionStateEnum.Back:
                    inputsArray.Remove(Input.Back);
                    break;
                case DirectionStateEnum.Neutral:
                    // Do nothing - Placeholder for later.
                    break;
                case DirectionStateEnum.Forward:
                    inputsArray.Remove(Input.Forward);
                    break;
                case DirectionStateEnum.DownBack:
                    inputsArray.Remove(Input.Down);
                    inputsArray.Remove(Input.Back);
                    break;
                case DirectionStateEnum.Down:
                    inputsArray.Remove(Input.Down);
                    break;
                case DirectionStateEnum.DownForward:
                    inputsArray.Remove(Input.Forward);
                    inputsArray.Remove(Input.Down);
                    break;
            }

            if (LightPunch == ButtonStateEnum.Released) inputsArray.Add(Input.LightPunch);
            if (MediumPunch == ButtonStateEnum.Released) inputsArray.Add(Input.MediumPunch);
            if (HardPunch == ButtonStateEnum.Released) inputsArray.Add(Input.HardPunch);
            if (LightKick == ButtonStateEnum.Released) inputsArray.Add(Input.LightKick);
            if (MediumKick == ButtonStateEnum.Released) inputsArray.Add(Input.MediumKick);
            if (HardKick == ButtonStateEnum.Released) inputsArray.Add(Input.HardKick);

            return inputsArray.ToArray();
        }

        public InputCommandModel()
        {
            LightPunch = ButtonStateEnum.Released;
            MediumPunch = ButtonStateEnum.Released;
            HardPunch = ButtonStateEnum.Released;

            LightKick = ButtonStateEnum.Released;
            MediumKick = ButtonStateEnum.Released;
            HardKick = ButtonStateEnum.Released;

            DirectionState = DirectionStateEnum.Neutral;
        }
    }
}
