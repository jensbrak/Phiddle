using Phiddle.Core.Measure;
using Phiddle.Core.Settings;
using Phiddle.Core.Services;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using SkiaSharp;

namespace Phiddle.Core
{
    public class AppTools
    {
        private readonly Tool[] toolSet;
        private int iActiveTool;
        private bool wideLinesOn;
        private MarkId marksVisible;

        public bool WideLinesOn
        {
            get => wideLinesOn;
            set
            {
                wideLinesOn = value;
                foreach (var tool in toolSet)
                {
                    tool.WideLinesOn = wideLinesOn;
                }
            }  
        }

        public LabelLocation LabelLocation { get; set; }
        public MarkId MarksVisible 
        { 
            get => marksVisible;
            set
            {
                marksVisible = value;
                foreach (var tool in toolSet)
                {
                    tool.SetMarksVisibility(marksVisible);
                }
            }
        }

        public Tool ActiveTool
        {
            get
            {
                return toolSet[iActiveTool];
            }
        }

        public AppTools(SKRectI screenDimensions, AppState appState, AppSettings settings)
        {
            iActiveTool = (int)appState.ActiveTool;
            LabelLocation = appState.LabelLocation;

            toolSet = new Tool[]
            {
                new ToolLine(settings.Tool) { LabelLocation = LabelLocation },
                new ToolRect(settings.Tool) { LabelLocation = LabelLocation },
                new ToolOval(settings.Tool) { LabelLocation = LabelLocation },
            };

            WideLinesOn = appState.ToolWideLinesOn;
            MarksVisible = appState.MarksVisible;
        }

        public void SelectNextTool()
        {
            iActiveTool = ++iActiveTool % toolSet.Length;
        }

        public void ToggleLabelPlacement()
        {
            LabelLocation = (LabelLocation)((int)++LabelLocation % Enum.GetNames(typeof(LabelLocation)).Length);

            foreach (var tool in toolSet)
            {
                tool.LabelLocation = LabelLocation;
            }
        }

        public void ToggleToolMarksVisibility(MarkId toolmarkToToggle)
        {
            MarksVisible ^= toolmarkToToggle;
        }
    }
}
