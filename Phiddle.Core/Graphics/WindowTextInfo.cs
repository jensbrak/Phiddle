using Phiddle.Core.Extensions;
using Phiddle.Core.Measure;
using Phiddle.Core.Settings;
using SkiaSharp;
using System;

namespace Phiddle.Core.Graphics
{
    public class WindowTextInfo : WindowText, IPosition
    {
        private enum LineNumbers
        {
            SelectedTool,
            LabelPlacement,
            MousePosition,
            Measurements,
        }

        public string MousePosition 
        { 
            get => Lines[(int)LineNumbers.MousePosition]; 
            set => Lines[(int)LineNumbers.MousePosition] = value; 
        }
        public string SelectedTool 
        { 
            get => Lines[(int)LineNumbers.SelectedTool]; 
            set => Lines[(int)LineNumbers.SelectedTool] = value; 
        }
        public string LabelPlacement
        {
            get => Lines[(int)LineNumbers.LabelPlacement];
            set => Lines[(int)LineNumbers.LabelPlacement] = value;
        }
        public WindowTextInfo(SKPoint pos, SKSize size, SettingsWindowText settings) : base(pos, size, settings)
        {
            Enabled = true;
            Lines = new string[Enum.GetNames(typeof(LineNumbers)).Length + Enum.GetNames(typeof(Measurement)).Length - 1];
            TabStops = new float[] { settings.TextLeftMargin };
        }

        public void Refresh(SKPoint pos)
        {
            MousePosition = $"Mouse:\t{string.Format("{0:F1}, {1:F1}", pos.X, pos.Y)}";
        }

        public void ReportLabelPlacement(MultiTool tool)
        {
            //LabelPlacement = $"Label:\t{tool.LabelLocation.GetDisplayName()}";
        }

        public void ReportSelectedTool(ITool tool)
        {
            SelectedTool = $"Tool: \t{tool}";
        }

        public void ReportMeasurements(MultiTool tool)
        {
            var i = 0;

            // Clear previous
            foreach (var c in Enum.GetNames(typeof(Measurement)))
            {
                Lines[(int)LineNumbers.Measurements + i++] = string.Empty;
            }

            i = 0;

            // Update
            foreach (var m in tool.Measurements)
            {
                // Get enum descriptive name and trim down to max X characters
                var mName = m.Key.GetDisplayName();
                var mNameTrimmed = mName.Length > 7 ? mName.Substring(0, 7) : mName;
                Lines[(int)LineNumbers.Measurements + i++] = $"{mNameTrimmed}:\t{m.Value}";
            }
        }
    }
}
