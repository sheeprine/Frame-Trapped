namespace FrameTrapped.StreetFighterLibrary.Utilities
{
    using FrameTrapped.StreetFighterLibrary.ViewModels;
    using FrameTrapped.Input.ViewModels;
    using FrameTrapped.Input.Models;
    using Caliburn.Micro;

    using RainbowLib;
    using RainbowLib.BCM;
    using RainbowLib.BAC;

    public static class BCMreader
    {

        static string sf4path = @"M:\SteamLibrary\SteamApps\common\Super Street Fighter IV - Arcade Edition\";

        private static BCMFile bcm;
        private static BACFile bac;

        private static string loadedCharacter;

        public static void loadBCM(string character)
        {
            string path = sf4path + @"patch\battle\regulation\latest_ae\" + character + @"\" + character + @".bcm"; //This isn't case sensitive.
            bcm = BCMFile.FromFilename(path);
            bac = RainbowLib.BACFile.FromFilename(path.Replace("bcm", "bac"), bcm);
            loadedCharacter = character.ToLower();
        }

        private static InputItemViewModel getCommands(string commands)
        {
            InputItemViewModel input = new InputItemViewModel();

            input.Light_Punch = commands.Contains("LP");
            input.Medium_Punch = commands.Contains("MP");
            input.Hard_Punch = commands.Contains("HP");
            input.Light_Kick = commands.Contains("LK");
            input.Medium_Kick = commands.Contains("MK");
            input.Hard_Kick = commands.Contains("HK");

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

        private static InputItemViewModel getMotion(InputMotionEntry motion) //For some reason 360's and 720's won't work with this. They have no "motion" in the bcm.
        {
            switch (motion.DirectionName.ToString())
            {
                case "DOWN":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Down };
                case "FORWARD":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Forward };
                case "BACK":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Back };
                case "UP":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Up };

                case "DOWN, BACK":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.DownBack };
                case "DOWN, FORWARD":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.DownForward };
                case "UP, FORWARD":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.UpForward };
                case "UP, BACK":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.UpBack };

                case "NEUTRAL":
                    return new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Neutral };

                default:
                    return null;
            }
        }

        public static MoveListViewModel getInputs()
        {
            CommandViewModel tmpCommand = new CommandViewModel(new System.Collections.ObjectModel.ObservableCollection<InputItemViewModel>());
            BindableCollection<HitViewModel> tmpHitList = new BindableCollection<HitViewModel>();
            MoveListViewModel moveList = new MoveListViewModel();

            foreach (InputMotion inputMotion in bcm.InputMotions)
            {
                switch (inputMotion.Entries[0].Type)
                {
                    case InputType.JOY_360:
                        if (inputMotion.Entries[0].Input == RainbowLib.Input.NEUTRAL)
                        {
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Forward });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Down });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Back });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Up });
                        }
                        else
                        {
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Forward });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Down });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Back });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Up });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Forward });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Down });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Back });
                            tmpCommand.Commands.Add(new InputItemViewModel() { Direction = InputCommandModel.DirectionStateEnum.Up });

                        }
                        break;

                    case InputType.NORMAL:
                    case InputType.MASH:

                        tmpCommand = new CommandViewModel(new BindableCollection<InputItemViewModel>());
                        foreach (InputMotionEntry motion in inputMotion.Entries)
                        {
                            tmpCommand.Commands.Add(BCMreader.getMotion(motion));
                        }
                        break;

                    case InputType.CHARGE:
                        //WTF DO WE DO WITH CHARGES?!
                        break;
                }
                moveList.Add(new MoveViewModel(FighterDataAE2012.Events, inputMotion.Name, MoveViewModel.MoveTypeEnum.Normal, tmpHitList, tmpCommand));
            }

            foreach (Move move in bcm.Moves)
            {
                tmpCommand = new CommandViewModel(new BindableCollection<InputItemViewModel>());
                tmpHitList = new BindableCollection<HitViewModel>();

                if (move.InputMotion.Name != "NONE")
                {
                    foreach (InputMotionEntry motion in move.InputMotion.Entries)
                    {
                        tmpCommand.Commands.Add(BCMreader.getMotion(motion));
                    }
                }

                if (move.Input.ToString() != "NONE")
                {
                    tmpCommand.Commands.Add(getCommands(move.Input.ToString()));
                }

                tmpHitList.Add(new HitViewModel(HitViewModel.BlockTypeEnum.Mid, -1, -1, -1, -1, (int)move.EXCost,
                    new[] { HitViewModel.CancelAbilityEnum.Chain, HitViewModel.CancelAbilityEnum.Special, HitViewModel.CancelAbilityEnum.Super },
                    (int)move.Script.FirstHitboxFrame, (int)(move.Script.LastHitboxFrame - move.Script.FirstHitboxFrame), -1, -1, -1, getNotes(move))); //Info loaded here is mostly wrong or missing.

                if (getMoveName(move) != "")
                    moveList.Add(new MoveViewModel(FighterDataAE2012.Events, getMoveName(move), MoveViewModel.MoveTypeEnum.Normal, tmpHitList, tmpCommand));
                else
                    moveList.Add(new MoveViewModel(FighterDataAE2012.Events, move.Name, MoveViewModel.MoveTypeEnum.Normal, tmpHitList, tmpCommand));
            }
            return moveList;
        }


        private static string getNotes(Move move) //These are just test notes.
        {
            string notes = "";

            if (loadedCharacter == "ryu" && move.Name == "BACK_DASH")
            {
                notes += "This is Ryu's backdash ";
            }

            if (move.StateRestriction.ToString().Contains("AIR"))
            {
                notes += "Air OK ";
            }

            if (move.InputMotion.Name.ToString() == "SCREW")
            {
                notes += "360 ";
            }

            if (move.InputMotion.Name.ToString() == "SC_SCREW")
            {
                notes += "720 ";
            }

            return notes;
        }

        private static string getMoveName(Move move)
        {
            string moveName = "";

            if (move.Name.Contains("8"))
            {
                moveName += "Neutral Jump ";
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
                moveName += "Neutral ";
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
                return "";
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
                return "";
            }
            return moveName;
        }
    }
}
