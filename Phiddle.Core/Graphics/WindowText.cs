using Phiddle.Core.Extensions;
using SkiaSharp;
using System;

namespace Phiddle.Core.Graphics
{
    public class WindowText : WindowBase
    {
        private float[] tabStops;
        public float[] TabStops
        {
            get
            {
                return tabStops;
            }
            set
            {
                var oldSize = tabStops.Length;
                var newSize = oldSize + value.Length;
                Array.Resize(ref tabStops, newSize);
                Array.Copy(value, 0, tabStops, oldSize, value.Length);
            }
        }
        public string Text 
        { 
            get
            {
                return string.Join("\n", Lines);
            }
            set
            {
                Lines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.None);
            }
        }
        public string[] Lines { get; set; }
        public SKPaint PaintLines { get; set; } = Defaults.WindowBasePaintText;

        public WindowText(SKRect bounds) : base(bounds)
        {
            PaintBorder = Defaults.WindowBasePaintBorder;
            PaintBackground =  Defaults.WindowBasePaintBackground;
            tabStops = new float[] { 10f }; // Left margin
            Visible = true;
            Transparent = false;
        }

        public override void Draw(SKCanvas c)
        {
            // If hidden, draw nothing
            if (!Visible)
            {
                return;
            }

            // Draw frame and any other basics
            base.Draw(c);

            // No text content and we're done
            if (Lines == null || Lines.Length == 0)
            {
                return;
            }


            // Margins and padding - move to defaults/settings later on
            var xPad = 10f;
            var yStart = 20f;
            var yPad = 10f;
            var yNext = 0f;

            // Draw vertical lines between tabs to make columns easier to read
            for (int i = 1; i < TabStops.Length; i++)
            {
                var x0 = TabStops[i];
                var x1 = x0;
                var y0 = yStart / 2;
                var y1 = Bounds.Bottom - y0;
                c.DrawLine(x0, y0, x1, y1, PaintLines);
            }

            // Draw each line
            foreach (var line in Lines)
            {
                if (line == null)
                {
                    continue;
                }

                // Each tab ('\t') splits line into columns, to be matched with any tab stops
                var columns = line.Split(new char[]{ '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int i;

                // Y-pos given for this line
                var yPos = yStart + yNext;

                // Loop all but last column or tab stop, whatever comes first
                for (i = 0; i < Math.Min(columns.Length, tabStops.Length) - 1; i++)
                {
                    // Match column text and tab stop
                    var text = columns[i];
                    var xPos = tabStops[i] + (i > 0 ? xPad : 0);
                    c.DrawText(text, xPos, yPos, PaintLines);
                }
                
                // Any left over columns joined together; either because it's the last column or we're out of tab stops
                if (i < columns.Length)
                {
                    var startIndex = i;
                    var count = columns.Length - i;
                    var text = string.Join(" ", columns,startIndex, count);
                    var xPos = tabStops[startIndex] + (i > 0 ? xPad : 0);
                    c.DrawText(text, xPos, yPos, PaintLines);
                }

                // Prepare for next line
                var b = PaintLines.GetTextBounds(line);
                yNext += b.Height + yPad;
            }
        }
    }
}
