using Phiddle.Core.Measure;
using System;

namespace Phiddle.Core
{
    public class ToolSet
    {
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

        public ToolSet()
        {
            labelLocation = Defaults.ToolLabelLocation;
            selected = 0;
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
    }
}
