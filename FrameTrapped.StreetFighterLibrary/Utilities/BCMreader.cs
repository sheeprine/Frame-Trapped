namespace FrameTrapped.StreetFighterLibrary.Utilities
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Caliburn.Micro;

    using FrameTrapped.Input.Models;
    using FrameTrapped.Input.ViewModels;
    using FrameTrapped.StreetFighterLibrary.ViewModels;

    using RainbowLib;
    using RainbowLib.BAC;
    using RainbowLib.BCM;

    using Input = RainbowLib.Input;

    /// <summary>The bcm reader.</summary>
    public static class BCMreader
    {
        /// <summary>The Street Fighter 4 path.</summary>
        private const string SF4Path = @"M:\SteamLibrary\SteamApps\common\Super Street Fighter IV - Arcade Edition\";

        /// <summary>The bac file.</summary>
        private static BACFile bac;

        /// <summary>The bcm file.</summary>
        private static BCMFile bcm;

        /// <summary>The loaded character.</summary>
        private static string loadedCharacter;

        /// <summary>
        /// Gets the get calculated frames.
        /// </summary>
        /// <param name="target">
        /// The target script. 
        /// </param>
        /// <returns>
        /// A float.
        /// </returns>
        public static float GetCalculatedFrames(Script target)
        {
            ushort realFrames = target.TotalFrames;
            ObservableCollection<SpeedCommand> speed = target.CommandLists[4];

            if (speed == null || speed.Count <= 0)
            {
                return realFrames;
            }

            float totalframesskipped = 0f;

            for (int i = 0; i < speed.Count; i++)
            {
                int appliedframes = 0;

                if (i < speed.Count - 1)
                {
                    appliedframes = speed[i + 1].StartFrame - speed[i].StartFrame;
                }
                else
                {
                    appliedframes = realFrames - speed[i].StartFrame;
                }

                if (speed[i].Multiplier == 0)
                {
                    totalframesskipped += appliedframes;
                }
                else
                {
                    totalframesskipped += appliedframes / speed[i].Multiplier;
                }
            }

            return totalframesskipped;
        }

        /// <summary>
        /// Gets the get hit-box frame calculations.
        /// </summary>
        /// <param name="target"> The target script. </param>
        /// <returns> An array of floats. </returns>
        public static float[] GetHitboxFrameCalculations(Script target)
        {
            ObservableCollection<HitboxCommand> hitboxes = target.CommandLists[7];

            float startup = 0f, active = 0f, recovery = 0f;

            foreach (HitboxCommand hitbox in hitboxes)
            {
                if (hitbox.Type == HitboxCommand.HitboxType.PROXIMITY)
                {
                    continue;
                }

                active += (hitbox.EndFrame - hitbox.StartFrame) / GetFrameSpeedMultiplier(target, hitbox.StartFrame);

                if (startup == 0)
                {
                    startup = GetSpeedModifiedFramesUpTo(target, hitbox.StartFrame);
                }
            }

            recovery = GetCalculatedFrames(target) - active - startup;

            return new[] { startup, active, recovery };
        }

        /// <summary>
        /// Gets the get human frames.
        /// </summary>
        /// <param name="target">
        /// The target script
        /// </param>
        /// <returns>
        /// String formatted with the startup, active and recovery frames.
        /// </returns>
        public static string GetHumanFrames(Script target)
        {
            float[] hitboxcalc = GetHitboxFrameCalculations(target);
            object[] param =
                {
                    (int)hitboxcalc[0], (int)hitboxcalc[1], (int)hitboxcalc[2],
                    (int)GetCalculatedFrames(target)
                }; // round everything

            return string.Format("Frames* -> Approx [ Start Up {0}, Active {1}, Recovery {2}, Total {3} ]", param);
        }

        /// <summary>The get moves.</summary>
        /// <returns>The <see cref="MoveListViewModel" />.</returns>
        public static MoveListViewModel GetMoves()
        {
            CommandViewModel tmpCommand = new CommandViewModel(new ObservableCollection<InputItemViewModel>());
            BindableCollection<HitViewModel> tmpHitList = new BindableCollection<HitViewModel>();
            MoveListViewModel moveList = new MoveListViewModel();

            foreach (Move move in bcm.Moves)
            {
                tmpCommand = new CommandViewModel(new BindableCollection<InputItemViewModel>());
                
                if (move.InputMotion.Name != "NONE")
                {
                    foreach (InputMotionEntry motion in move.InputMotion.Entries)
                    {
                        tmpCommand.Commands.Add(GetMotion(motion));
                    }
                }

                if (move.Input.ToString() != "NONE")
                {
                    tmpCommand.Commands.Add(GetCommands(move.Input.ToString()));
                }

                foreach (CancelList cancel in bcm.CancelLists)
                {
                    if (cancel.Name == move.Name)
                    {
                        // Add move to the cancel list.
                        // move.
                    }
                }

                tmpHitList = GetHits(move);

                // tmpHitList.Add(
                //    new HitViewModel(
                //        HitViewModel.BlockTypeEnum.Mid, 
                //        -1, 
                //        -1, 
                //        -1, 
                //        -1, 
                //        move.EXCost, 
                //        new[] { HitViewModel.CancelAbilityEnum.Chain, HitViewModel.CancelAbilityEnum.Special, HitViewModel.CancelAbilityEnum.Super }, 
                //        move.Script.FirstHitboxFrame, 
                //        move.Script.LastHitboxFrame - move.Script.FirstHitboxFrame, 
                //        -1, 
                //        -1, 
                //        -1, 
                //        GetNotes(move))); // Info loaded here is mostly wrong or missing.

                moveList.Add(
                    GetMoveName(move) != string.Empty
                        ? new MoveViewModel(
                              FighterDataAE2012.Events,
                              GetMoveName(move),
                              MoveViewModel.MoveTypeEnum.Normal,
                              tmpHitList,
                              tmpCommand)
                        : new MoveViewModel(
                              FighterDataAE2012.Events,
                              move.Name,
                              MoveViewModel.MoveTypeEnum.Normal,
                              tmpHitList,
                              tmpCommand));
            }

            foreach (InputMotion inputMotion in bcm.InputMotions)
            {
                if (inputMotion.Entries.Count != 0)
                {
                    switch (inputMotion.Entries[0].Type)
                    {
                        case InputType.JOY_360:
                            if (inputMotion.Entries[0].Input == Input.NEUTRAL)
                            {
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                            }
                            else
                            {
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                                tmpCommand.Commands.Add(
                                    new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                            }

                            break;

                        case InputType.NORMAL:
                        case InputType.MASH:

                            tmpCommand = new CommandViewModel(new BindableCollection<InputItemViewModel>());
                            foreach (InputMotionEntry motion in inputMotion.Entries)
                            {
                                tmpCommand.Commands.Add(GetMotion(motion));
                            }

                            break;

                        case InputType.CHARGE:

                            // WTF DO WE DO WITH CHARGES?!
                            break;
                    }
                }

                moveList.Add(
                    new MoveViewModel(
                        FighterDataAE2012.Events,
                        inputMotion.Name,
                        MoveViewModel.MoveTypeEnum.Normal,
                        tmpHitList,
                        tmpCommand));
            }

            return moveList;
        }

        /// <summary>
        /// The load BCM.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        public static void LoadBCM(string character)
        {
            string path = string.Format("{0}patch\\battle\\regulation\\latest_ae\\{1}\\{1}.bcm", SF4Path, character);

            // This isn't case sensitive.
            bcm = BCMFile.FromFilename(path);
            bac = BACFile.FromFilename(path.Replace("bcm", "bac"), bcm);

            loadedCharacter = character.ToLower();
        }

        /// <summary>
        /// Get the speed multiplier for the given frame.
        /// </summary>
        /// <param name="target">The target script</param>
        /// <param name="frame">The frame in the move to calculate.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float GetFrameSpeedMultiplier(Script target, int frame)
        {
            ObservableCollection<SpeedCommand> speed = target.CommandLists[4];

            if (speed == null || speed.Count <= 0)
            {
                return 1f;
            }

            for (int i = 0; i < speed.Count; i++)
            {
                if (i < speed.Count - 1)
                {
                    if (frame >= speed[i].EndFrame && frame <= speed[i + 1].StartFrame)
                    {
                        return speed[i].Multiplier;
                    }
                }
                else
                {
                    float lastmult = speed[speed.Count - 1].Multiplier;
                    return lastmult == 0 ? 1f : lastmult;
                }
            }

            return 1f;
        }

        /// <summary>
        /// The get speed modified frames up to.
        /// </summary>
        /// <param name="target">
        /// The target script.
        /// </param>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float GetSpeedModifiedFramesUpTo(Script target, int frame)
        {
            ObservableCollection<SpeedCommand> speed = target.CommandLists[4];

            if (speed == null || speed.Count <= 0)
            {
                return frame;
            }

            float totalFramesSkipped = 0f;

            for (int i = 0; i < speed.Count; i++)
            {
                int appliedframes = 0;

                if (i < speed.Count - 1)
                {
                    if (speed[i].EndFrame >= frame)
                    {
                        continue;
                    }

                    appliedframes = speed[i + 1].StartFrame - speed[i].StartFrame;
                }
                else
                {
                    if (speed[speed.Count - 1].EndFrame >= frame)
                    {
                        continue;
                    }

                    appliedframes = frame - speed[i].StartFrame;
                }

                if (speed[i].Multiplier == 0)
                {
                    totalFramesSkipped += appliedframes;
                }
                else
                {
                    totalFramesSkipped += appliedframes / speed[i].Multiplier;
                }
            }

            return totalFramesSkipped;
        }

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <param name="commands">
        /// The commands.
        /// </param>
        /// <returns>
        /// The <see cref="InputItemViewModel"/>.
        /// </returns>
        private static InputItemViewModel GetCommands(string commands)
        {
            InputItemViewModel input = new InputItemViewModel
                                           {
                                               Light_Punch = commands.Contains("LP"),
                                               Medium_Punch = commands.Contains("MP"),
                                               Hard_Punch = commands.Contains("HP"),
                                               Light_Kick = commands.Contains("LK"),
                                               Medium_Kick = commands.Contains("MK"),
                                               Hard_Kick = commands.Contains("HK")
                                           };

            if (commands.Contains("UP, FORWARD"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.UpForward;
            }
            else if (commands.Contains("UP, BACK"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.UpBack;
            }
            else if (commands.Contains("DOWN, FORWARD"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.DownForward;
            }
            else if (commands.Contains("DOWN, BACK"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.DownBack;
            }
            else if (commands.Contains("FORWARD"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.Forward;
            }
            else if (commands.Contains("BACK"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.Back;
            }
            else if (commands.Contains("UP"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.Up;
            }
            else if (commands.Contains("DOWN"))
            {
                input.Direction = InputCommandModel.DirectionStateEnum.Down;
            }
            else
            {
                input.Direction = InputCommandModel.DirectionStateEnum.Neutral;
            }

            return input;
        }

        /// <summary>
        /// The get hits.
        /// </summary>
        /// <param name="move">
        /// The move.
        /// </param>
        /// <returns>
        /// The <see cref="BindableCollection"/>.
        /// </returns>
        private static BindableCollection<HitViewModel> GetHits(Move move)
        {
            BindableCollection<HitViewModel> tmpHitList = new BindableCollection<HitViewModel>();
            HitViewModel.BlockTypeEnum tmpBlock = new HitViewModel.BlockTypeEnum();
            int damage = -1;
            int lateDamage = -1;
            int stun = -1;
            int latestun = -1;
            int meterGain = -1;
            HitViewModel.CancelAbilityEnum[] tmpCancel = new HitViewModel.CancelAbilityEnum[1];
            float startUp = -1;
            float active = -1;
            float recovery = -1;
            int onBlockAdvantage = -1;
            int onHitAdvantage = -1;
            string notes = string.Empty;

            List<HitViewModel.CancelAbilityEnum> cancelList = new List<HitViewModel.CancelAbilityEnum>();

            // These aren't finished. More cancels exists. I'll add them as I go.
            if (move.Script.Name != "NONE")
            {

            //    foreach (CancelCommand cancel in move.Script.CommandLists[(int)CommandListType.CANCELS])
            //    {
            //        if (cancel.CancelList != null)
            //        {
            //            // This is just for debugging...
            //            notes += cancel.CancelList + " ";
            //            if (cancel.CancelList.ToString().Contains("SUPER MOVE"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Special);
            //            }

            //            if (cancel.CancelList.ToString().Contains("SUPER COMBO"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Super);
            //            }

            //            if (cancel.CancelList.ToString().Contains("SC"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Super);
            //            }

            //            if (cancel.CancelList.ToString().Contains("UC"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Ultra);
            //            }

            //            if (cancel.CancelList.ToString().Contains("SAVING"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Dash);
            //            }

            //            if (cancel.CancelList.ToString().Contains("CANCEL_"))
            //            {
            //                cancelList.Add(HitViewModel.CancelAbilityEnum.Chain);
            //            }
            //        }
            //    }

            //    tmpCancel = cancelList.ToArray();

                Script target = move.Script;
                ObservableCollection<HitboxCommand> hitboxes = target.CommandLists[7];

                foreach (HitboxCommand hitbox in move.Script.CommandLists[(int)CommandListType.HITBOX])
                {
                    // Exclude proximity boxes
                    if (hitbox.Type != HitboxCommand.HitboxType.PROXIMITY)
                    {
                        switch (hitbox.HitLevel)
                        {
                            case HitboxCommand.HitLevelType.UNBLOCKABLE:
                                tmpBlock = HitViewModel.BlockTypeEnum.Unblockable;
                                break;
                            case HitboxCommand.HitLevelType.OVERHEAD:
                                tmpBlock = HitViewModel.BlockTypeEnum.High;
                                break;
                            case HitboxCommand.HitLevelType.MID:
                                tmpBlock = HitViewModel.BlockTypeEnum.Mid;
                                break;
                            case HitboxCommand.HitLevelType.LOW:
                                tmpBlock = HitViewModel.BlockTypeEnum.Low;
                                break;

                            default:
                                tmpBlock = HitViewModel.BlockTypeEnum.Mid;
                                break;
                        }

                        active = (hitbox.EndFrame - hitbox.StartFrame)
                                   / GetFrameSpeedMultiplier(target, hitbox.StartFrame);

                        if (startUp == -1.0)
                        {
                            startUp = GetSpeedModifiedFramesUpTo(target, hitbox.StartFrame);
                        }


                        recovery = GetCalculatedFrames(target) - active - startUp;
                        float[] hitboxcalc = new[] { startUp, active, recovery };
                        int[] param =
                        {
                            (int)hitboxcalc[0], (int)hitboxcalc[1], (int)hitboxcalc[2],
                            (int)GetCalculatedFrames(move.Script)
                        }; // round everything

                        // Frames
                        startUp = param[0];
                        active = param[1];
                        recovery = param[2];

                        // Hit data
                        HitBoxDataset hitBoxDataset = bac.HitboxTable.First(o => o.UsageString.Contains(move.Script.Name));
                        if (hitBoxDataset != null)
                        {
                            HitBoxData standingHit = hitBoxDataset.Data[0];
                            damage = standingHit.Damage;
                            stun = standingHit.Stun;
                            meterGain = standingHit.SelfMeter;
                        }
                    }
                }
                tmpHitList.Add(
                    new HitViewModel(
                        tmpBlock,
                        damage,
                        lateDamage,
                        stun,
                        latestun,
                        meterGain,
                        tmpCancel,
                        (int)startUp,
                        (int)active,
                        (int)recovery,
                        onBlockAdvantage,
                        onHitAdvantage,
                        notes));

                // Info loaded here is mostly wrong or missing. Notes are currently used for debugging/getting info.
            }

            return tmpHitList;
        }

        /// <summary>
        /// The get motion.
        /// </summary>
        /// <param name="motion">
        /// The motion.
        /// </param>
        /// <returns>
        /// The <see cref="InputItemViewModel"/>.
        /// </returns>
        /// <remarks>For some reason 360's and 720's won't work with this. They have no "motion" in the bcm.</remarks>
        private static InputItemViewModel GetMotion(InputMotionEntry motion)
        {
            switch (motion.DirectionName)
            {
                case "DOWN":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down };
                case "FORWARD":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward };
                case "BACK":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back };
                case "UP":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up };

                case "DOWN, BACK":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack };
                case "DOWN, FORWARD":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward };
                case "UP, FORWARD":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward };
                case "UP, BACK":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpBack };

                case "NEUTRAL":
                    return new InputItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral };

                default:
                    return null;
            }
        }

        /// <summary>
        /// The get move name.
        /// </summary>
        /// <param name="move">
        /// The move.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetMoveName(Move move)
        {
            string moveName = string.Empty;

            if (move.Name.Contains("8"))
            {
                moveName += "Jump ";
            }
            else if (move.Name.Contains("7"))
            {
                moveName += "Back Jump ";
            }
            else if (move.Name.Contains("9"))
            {
                moveName += "Forward Jump ";
            }
            else if (move.Name.Contains("5"))
            {
                moveName += "Standing ";
            }
            else if (move.Name.Contains("4"))
            {
                moveName += "Back ";
            }
            else if (move.Name.Contains("6"))
            {
                moveName += "Forward ";
            }
            else if (move.Name.Contains("2"))
            {
                moveName += "Down ";
            }
            else if (move.Name.Contains("1"))
            {
                moveName += "Down Back ";
            }
            else if (move.Name.Contains("3"))
            {
                moveName += "Down Forward ";
            }
            else
            {
                return string.Empty;
            }

            if (move.Name.Contains("LP"))
            {
                moveName += "Light Punch";
            }
            else if (move.Name.Contains("MP"))
            {
                moveName += "Medium Punch";
            }
            else if (move.Name.Contains("HP"))
            {
                moveName += "Hard Punch";
            }
            else if (move.Name.Contains("LK"))
            {
                moveName += "Light Kick";
            }
            else if (move.Name.Contains("MK"))
            {
                moveName += "Medium Kick";
            }
            else if (move.Name.Contains("HK"))
            {
                moveName += "Hard Kick";
            }
            else
            {
                return string.Empty;
            }

            return moveName;
        }

        /// <summary>
        /// The get notes.
        /// </summary>
        /// <param name="move">
        /// The move.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetNotes(Move move)
        {
            // These are just test notes.
            string notes = string.Empty;

            if (loadedCharacter == "ryu" && move.Name == "BACK_DASH")
            {
                notes += "This is Ryu's backdash ";
            }

            if (move.StateRestriction.ToString().Contains("AIR"))
            {
                notes += "Air OK ";
            }

            if (move.InputMotion.Name == "SCREW")
            {
                notes += "360 ";
            }

            if (move.InputMotion.Name == "SC_SCREW")
            {
                notes += "720 ";
            }

            return notes;
        }
    }
}