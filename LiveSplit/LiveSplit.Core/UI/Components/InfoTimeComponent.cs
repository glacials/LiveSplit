﻿using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.Components
{
    public class InfoTimeComponent : InfoTextComponent
    {
        private TimeSpan? timeValue;
        public TimeSpan? TimeValue 
        {
            get 
            { 
                return timeValue; 
            } 
            set
            {
                timeValue = value;
                InformationValue = Formatter.Format(timeValue);
            }
        }
        public ITimeFormatter Formatter { get; set; }

        public override void PrepareDraw(Model.LiveSplitState state, LayoutMode mode)
        {
            ValueLabel.IsMonospaced = true;
            ValueLabel.Font = state.LayoutSettings.TimesFont;
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            NameLabel.Font = state.LayoutSettings.TextFont;
            if (mode == LayoutMode.Vertical)
            {
                NameLabel.VerticalAlignment = StringAlignment.Center;
                ValueLabel.VerticalAlignment = StringAlignment.Center;
            }
            else
            {
                NameLabel.VerticalAlignment = StringAlignment.Near;
                ValueLabel.VerticalAlignment = StringAlignment.Far;
            }
        }

        public InfoTimeComponent(String informationName, TimeSpan? timeValue, ITimeFormatter formatter)
            : base(informationName, "")
        {
            Formatter = formatter;
            TimeValue = timeValue;
        }
    }
}
