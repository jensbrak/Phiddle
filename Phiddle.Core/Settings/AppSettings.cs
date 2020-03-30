using Phiddle.Core.Measure;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Phiddle.Core.Settings
{
    public class AppSettings
    {
        // Windows
        // .. Sizes & Positions
        public float WindowBaseSizeFactor { get; set; } = 5f;
        public float WindowZoomZoomFactor { get; set; } = 5f;
        public float WindowZoomCrosshairSize { get; set; } = 20f;
        public float WindowInfoTextLeftMargin { get; set; } = 65f;
        // .. Paints
        public SKPaint WindowBaseBorder { get; set; } = new SKPaint() { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 1f };
        public SKPaint WindowBaseBackground { get; set; }
        public SKPaint WindowBaseText { get; set; }
        public SKPaint WindowZoomCrosshair { get; set; }
        public SKPaint HelpLines { get; set; }

        // Tools
        // .. Sizes & Positions
        public float ToolBoundsPad { get; set; } = 4f;
        // .. Paints
        public SKPaint ToolBase { get; set; }
        public SKPaint ToolBaseBounds { get; set; }
        public SKPaint ToolBaseEndpoint { get; set; }

        // Marks
        //.. Sizes & Positions
        public float ToolMarkSize { get; set; } = 10f;
        public float ToolEndpointSize { get; set; } = 12f;
        // .. Paints
        public SKPaint MarkPhi { get; set; }
        public SKPaint MarkMiddle { get; set; }
        public SKPaint MarkThird { get; set; }

        // Labels
        // .. Sizes & Positions
        public float LabelTextPad { get; set; } = 4f;
        // .. Paints
        public SKPaint LabelBackground { get; set; }
        public SKPaint LabelText { get; set; }
    }
}


