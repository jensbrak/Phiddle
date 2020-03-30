using Phiddle.Core.Measure;
using Phiddle.Core.Settings;
using Phiddle.Core.Services;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Phiddle.Core
{
    public class AppTools : IDisposable
    {
        private SettingsService<AppState> state;
        private readonly ToolBase[] tools;
        private int selected;
        private LabelLocation labelLocation;

        public ToolBase SelectedTool 
        { 
            get
            {
                return tools[selected];
            }
        }

        public AppTools(SettingsService<AppState> appState)
        {
            state = appState;
            labelLocation = appState.Settings.LabelLocation;
            selected = appState.Settings.CurrentTool;
            
            tools = new ToolBase[]
            {
                new ToolLine() { LabelLocation = labelLocation },
                new ToolRect() { LabelLocation = labelLocation },
                new ToolOval() { LabelLocation = labelLocation },
            };
        }

        public void SelectNextTool()
        {
            selected = ++selected % tools.Length;
        }

        public void ToggleLabelPlacement()
        {
            labelLocation = (LabelLocation)((int)++labelLocation % Enum.GetNames(typeof(LabelLocation)).Length);

            foreach (var tool in tools)
            {
                tool.LabelLocation = labelLocation;
            }
        }

        public void ToggleToolMarks(MarkCategory c)
        {
            Console.WriteLine("ToggleToolMarks for " + c);
            foreach (var t in tools)
            {
                t.ToggleMarks(c);
            }
        }

        public void Dispose()
        {
            state.Settings.CurrentTool = selected;
            state.Settings.LabelLocation = labelLocation;
            state.Save();
        }
    }
}
