namespace FrameTrapped.ComboTrainer.ViewModels
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using FrameTrapped.Input.Models;
    using FrameTrapped.Input.Utilities;
    using FrameTrapped.Input.ViewModels;
    using FrameTrapped.Utilities;
    using FrameTrapped.ComboTrainer.Messages;
    using System.IO;

    /// <summary>
    /// The time line item view model class.
    /// </summary>
    public class TimeLineViewModel : Conductor<TimeLineItemViewModel>.Collection.OneActive
    {
        /// <summary>
        /// The premade commands enumerator.
        /// </summary>
        public enum PremadeCommandsEnum
        {
            /// <summary>
            /// Empty command.
            /// </summary>
            Empty,

            /// <summary>
            /// Dash forward.
            /// </summary>
            Dash,

            /// <summary>
            /// Backdash.
            /// </summary>
            Backdash,

            /// <summary>
            /// FocusAttack Dash Cancel.
            /// </summary>
            FADC,

            /// <summary>
            /// FocusAttack Dash Cancel Back.
            /// </summary>
            FADCB,

            /// <summary>
            /// Hadoken motion (236).
            /// </summary>
            QCF,

            /// <summary>
            /// Shoryuken motion (623).
            /// </summary>
            DP,

            /// <summary>
            /// Tatsumaki motion (214).
            /// </summary>
            QCB,

            /// <summary>
            /// Dragon kick motion (214).
            /// </summary>
            RDP,

            /// <summary>
            /// Half circle forward (41236).
            /// </summary>
            HCF,

            /// <summary>
            /// Half circle backward (63214).
            /// </summary>
            HCB,

            /// <summary>
            /// Half circle forward up (12369).
            /// </summary>
            HCFU,

            /// <summary>
            /// Shinku Hadoken motion (236236).
            /// </summary>
            Super,

            /// <summary>
            /// Shink Tatsumaki motion (214214)
            /// </summary>
            ReverseSuper,

            /// <summary>
            /// Charge Down 55 frames (2..)
            /// </summary>
            ChargeD55,

            /// <summary>
            /// Charge Back 55 frames (4..)
            /// </summary>
            ChargeB55,

            /// <summary>
            /// Charge DownBack 55 frames (1..)
            /// </summary>
            ChargeDB55,

            /// <summary>
            /// The delta motion after charge (6..646)
            /// </summary>
            ChargeSuper,

            /// <summary>
            /// The delta motion after charge (6..319)
            /// </summary>
            ChargeDelta,

            /// <summary>
            /// Screw pile driver motion.
            /// </summary>
            SPD,

            /// <summary>
            /// Reverse screw pile driver motion.
            /// </summary>
            SPDB,
            
            /// <summary>
            /// Full circle motion.
            /// </summary>
            FullCircle,

            /// <summary>
            /// Full double circle motion.
            /// </summary>
            DoubleCircle
        }

        /// <summary>
        /// The events aggregator.
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// The send inputs flag.
        /// </summary>
        private bool _sendInputs;

        /// <summary>
        /// The currently selected time line item.
        /// </summary>
        private TimeLineItemViewModel _selectedTimeLineItem;

        /// <summary>
        /// The time line items.
        /// </summary>
        private BindableCollection<TimeLineItemViewModel> _timeLineItems;

        /// <summary>
        /// Gets or sets the currently selected time line item.
        /// </summary>
        public TimeLineItemViewModel SelectedTimeLineItem
        {
            get { return _selectedTimeLineItem; }
            set
            {
                if (_selectedTimeLineItem != null)
                {
                    _selectedTimeLineItem.DeHighlight();
                }

                _selectedTimeLineItem = value;

                if (value != null)
                {
                    _selectedTimeLineItem.Highlight();
                }
                NotifyOfPropertyChange(() => SelectedTimeLineItem);
            }
        }

        /// <summary>
        /// The collection of time line items.
        /// </summary>
        public BindableCollection<TimeLineItemViewModel> TimeLineItems
        {
            get
            {
                return _timeLineItems;
            }
            private set
            {
                _timeLineItems = value;
                NotifyOfPropertyChange(() => TimeLineItems);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating wether we wants to automatically switch to SF4.
        /// </summary>
        public bool SendInputs
        {
            get { return _sendInputs; }
            set
            {
                if (value != _sendInputs)
                {
                    _sendInputs = value;
                    NotifyOfPropertyChange(() => SendInputs);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether you can remove a time line item.
        /// </summary>
        public bool CanRemoveItem
        {
            get { return TimeLineItems.Count > 1; }
        }

        /// <summary>
        /// Adds a time line item to the end of the time line.
        /// </summary>
        public void AddTimeLineItem(string premadeCommand)
        {
            List<TimeLineItemViewModel> timeLineItemList = new List<TimeLineItemViewModel>();

            switch ((PremadeCommandsEnum)Enum.Parse(typeof(PremadeCommandsEnum), premadeCommand))
            {
                case PremadeCommandsEnum.Empty:
                    timeLineItemList.Add(new TimeLineItemViewModel());

                    break;
                case PremadeCommandsEnum.Dash:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral, WaitFrames = 16 });
                    break;
                case PremadeCommandsEnum.Backdash:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral, WaitFrames = 19 });
                    break;
                case PremadeCommandsEnum.FADC:
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Neutral });
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral, WaitFrames = 16 });
                    break;
                case PremadeCommandsEnum.FADCB:
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Neutral });
                    timeLineItemList.Add(new TimeLineItemViewModel { Medium_Punch = true, Medium_Kick = true, Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Neutral, WaitFrames = 19 });
                    break;
                case PremadeCommandsEnum.QCF:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    break;
                case PremadeCommandsEnum.DP:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    break;
                case PremadeCommandsEnum.QCB:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    break;
                case PremadeCommandsEnum.RDP:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    break;
                case PremadeCommandsEnum.HCF:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    break;
                case PremadeCommandsEnum.HCB:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    break;
                case PremadeCommandsEnum.HCFU:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    break;
                case PremadeCommandsEnum.Super:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    break;
                case PremadeCommandsEnum.ReverseSuper:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    break;
                case PremadeCommandsEnum.SPD:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    break;
                case PremadeCommandsEnum.SPDB:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpBack });
                    break;
                case PremadeCommandsEnum.FullCircle:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    break;

                case PremadeCommandsEnum.DoubleCircle:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Up });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    break;
                case PremadeCommandsEnum.ChargeD55:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Down, WaitFrames = 55 });
                    break;
                case PremadeCommandsEnum.ChargeB55:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back, WaitFrames = 55 });
                    break;
                case PremadeCommandsEnum.ChargeDB55:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack, WaitFrames = 55 });
                    break;
                case PremadeCommandsEnum.ChargeSuper:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back, WaitFrames = 55 });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Back });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.Forward });
                    break;
                case PremadeCommandsEnum.ChargeDelta:
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack, WaitFrames = 55 });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownForward });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.DownBack });
                    timeLineItemList.Add(new TimeLineItemViewModel { Direction = InputCommandModel.DirectionStateEnum.UpForward });
                    break;
            }

            foreach (TimeLineItemViewModel item in timeLineItemList)
            {
                item.Parent = this; 
                AddTimeLineItem(item);
            }
        }

        /// <summary>
        /// Adds a blank item after the current point in the time line.
        /// </summary>
        /// <param name="timeLineItemViewModel"></param>
        public void AddTimeLineItem(TimeLineItemViewModel timeLineItemViewModel)
        {
            TimeLineItems.Insert(TimeLineItems.IndexOf(SelectedTimeLineItem) + 1, timeLineItemViewModel);

            SelectedTimeLineItem = timeLineItemViewModel;

            NotifyOfPropertyChange(() => CanRemoveItem);
            NotifyOfPropertyChange(() => TimeLineItems);
        }

        /// <summary>
        /// Appends the opened file to the time line.
        /// </summary>
        public void AppendTimeLine()
        {
            OpenTimeLine(string.Empty, true);
        }

        /// <summary>
        /// Removes the time line item at the end or after the current selection.
        /// </summary>
        public void RemoveTimeLineItem()
        {
            if (CanRemoveItem)
            {
                int index = TimeLineItems.IndexOf(SelectedTimeLineItem);
                TimeLineItemViewModel toRemove = SelectedTimeLineItem;

                //If not last item, select next, otherwise select last
                if (SelectedTimeLineItem != TimeLineItems.Last())
                {
                    SelectedTimeLineItem = TimeLineItems[index + 1];
                }
                else
                {
                    SelectedTimeLineItem = TimeLineItems[TimeLineItems.Count() - 2];
                }

                TimeLineItems.Remove(toRemove);

                NotifyOfPropertyChange(() => CanRemoveItem);
                NotifyOfPropertyChange(() => TimeLineItems);
            }
        }

        /// <summary>
        /// Clears the time line.
        /// </summary>
        public void ClearTimeLine()
        {
            TimeLineItems.Clear();
            TimeLineItemViewModel newItem = new TimeLineItemViewModel(this);
            AddTimeLineItem(newItem);

            NotifyOfPropertyChange(() => CanRemoveItem);
            NotifyOfPropertyChange(() => TimeLineItems);
        }

        /// <summary>
        /// Opens a new time line from file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="append"></param>
        public void OpenTimeLine(string filePath, bool append = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = "SF4 Combo|*.cmb";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName == "")
                {
                    return;
                }
                filePath = openFileDialog.FileName;
            }

            List<string> lines = File.ReadAllLines(filePath).ToList();

            if (!append) ClearTimeLine();

            if (lines.First().StartsWith("FTComboFile"))
            {
                lines.RemoveAt(0);
                foreach (String line in lines)
                {
                    TimeLineItemViewModel timeLineItemViewModel = new TimeLineItemViewModel(this);
                    InputItemViewModel inputItem = InputItemViewModel.Deserialize(line);

                    timeLineItemViewModel.PlaySound = inputItem.PlaySound;
                    timeLineItemViewModel.WaitFrames = inputItem.WaitFrames;
                    timeLineItemViewModel.Direction = inputItem.Direction;

                    timeLineItemViewModel.Light_Punch = inputItem.Light_Punch;
                    timeLineItemViewModel.Medium_Punch = inputItem.Medium_Punch;
                    timeLineItemViewModel.Hard_Punch = inputItem.Hard_Punch;

                    timeLineItemViewModel.Light_Kick = inputItem.Light_Kick;
                    timeLineItemViewModel.Medium_Kick = inputItem.Medium_Kick;
                    timeLineItemViewModel.Hard_Kick = inputItem.Hard_Kick;

                    TimeLineItems.Add(timeLineItemViewModel);
                }
            }
            else
            {
                OldDeserialize(lines);
            }

            NotifyOfPropertyChange(() => CanRemoveItem);
        }


        public void OldDeserialize(List<string> lines)
        {
            List<Input> inputsOnHold = new List<Input>();
            foreach (String line in lines)
            {
                String[] tokens = line.Split('#');

                InputItemModel item = new InputItemModel();
                int numInputs;
                Input[] inputs;

                string inputType = tokens[0].ToString().ToUpper();
                switch (inputType)
                {
                    case "WAIT":
                        // Only hold inputs needed
                        item = new InputItemModel();
                        item.Inputs = inputsOnHold.ToArray();
                        break;

                    case "PRESS":

                        // Add hold inputs to given inputs
                        numInputs = int.Parse(tokens[2]);
                        inputs = new Input[numInputs];
                        for (int i = 0; i < numInputs; i++)
                        {
                            inputs[i] = InputItemModel.ParseInput(tokens[3 + i]);
                        }

                        item = new InputItemModel();
                        item.Inputs = inputsOnHold.Concat(inputs.ToList()).Distinct().ToArray();
                        break;

                    case "HOLD":

                        numInputs = int.Parse(tokens[2]);
                        for (int i = 0; i < numInputs; i++)
                        {
                            inputsOnHold.Add(InputItemModel.ParseInput(tokens[3 + i]));
                        }

                        item = new InputItemModel();
                        item.Inputs = inputsOnHold.Distinct().ToArray();
                        break;

                    case "RELEASE":

                        // First remove released inputs from on hold list
                        numInputs = int.Parse(tokens[2]);
                        inputs = new Input[numInputs];
                        for (int i = 0; i < numInputs; i++)
                        {
                            inputsOnHold.RemoveAll((x) => x == InputItemModel.ParseInput(tokens[3 + i]));
                        }

                        // Add remaining holds
                        item = new InputItemModel();
                        item.Inputs = inputsOnHold.ToArray();
                        break;

                    default:
                        throw new FormatException("Failed to deserialize TimelineItem, wrong string format: " + line);
                }

                item.PlaySound = Boolean.Parse(tokens[1]);
                item.Frames = int.Parse(tokens[2].ToString());

                //Setup TimeLineItemViewModel
                InputItemViewModel result = new InputItemViewModel();
                result.InputItem = item;
                result.Index = -1;

                TimeLineItemViewModel timeLineItemViewModel = new TimeLineItemViewModel(this);

                timeLineItemViewModel.PlaySound = result.PlaySound;
                timeLineItemViewModel.WaitFrames = result.WaitFrames;
                timeLineItemViewModel.Direction = result.Direction;

                timeLineItemViewModel.Light_Punch = result.Light_Punch;
                timeLineItemViewModel.Medium_Punch = result.Medium_Punch;
                timeLineItemViewModel.Hard_Punch = result.Hard_Punch;

                timeLineItemViewModel.Light_Kick = result.Light_Kick;
                timeLineItemViewModel.Medium_Kick = result.Medium_Kick;
                timeLineItemViewModel.Hard_Kick = result.Hard_Kick;

                TimeLineItems.Add(timeLineItemViewModel);
            }
        }

        /// <summary>
        /// Saves the time line.
        /// </summary>
        public void SaveTimeLine()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SF4 Combo|*.cmb";
            saveFileDialog.Title = "Save your combo file...";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    file.WriteLine("FTComboFile[v2.1]");
                    foreach (TimeLineItemViewModel item in TimeLineItems)
                    {
                        file.WriteLine(item.InputItemViewModel.InputItem.Serialize());
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeLineViewmodel"/> class.
        /// </summary>
        /// <param name="events">The events aggregator.</param>
        public TimeLineViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);

            TimeLineItems = new BindableCollection<TimeLineItemViewModel>();
            TimeLineItems.Add(new TimeLineItemViewModel(this));

            SelectedTimeLineItem = TimeLineItems.Last();

            _sendInputs = true;
        }
    }
}
