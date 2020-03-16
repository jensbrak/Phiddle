
using Phiddle.Core.Measure;
using SkiaSharp;

namespace Phiddle.Core
{
    public static class Defaults
    {
        // Text and symbol formats
        public static string PositionFormat { get; set; } = "{0}, {1}";

        // Sizes, positions and states
        public static float WindowsSizeFactor { get; set; } = 5f;
        public static float WindowZoomFactor { get; set; } = 5f;
        public static bool HelpLinesVisible { get; set; } = false;
        public static bool WindowZoomVisible { get; set; } = true;
        public static bool WindowZoomCrosshairVisible { get; set; } = true;
        public static bool WindowInfoVisible { get; set; } = true;
        public static bool ToolMarkVisibleEndpoint { get; set; } = true;
        public static bool ToolMarkVisibleGoldenRatio { get; set; } = true;
        public static bool ToolMarkVisibleThird { get; set; } = false;
        public static bool ToolMarkVisibleMiddle { get; set; } = false;
        public static LabelLocation ToolLabelLocation { get; set; } = LabelLocation.Off;
        public static float WindowZoomCrosshairSize { get; set; } = 20f;
        public static float WindowInfoTextLeftMargin { get; set; } = 65f;
        public static float ToolMarkSize { get; set; } = 10f;
        public static float ToolEndpointSize { get; set; } = 12f;
        public static float ToolBoundsVisualPadding { get; set; } = 4f;
        public static float LabelTextPad { get; set; } = 4f;

        // Paint properties for all elements that can be drawn
        public static SKPaint WindowBasePaintBorder { get; set; } = new SKPaint()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
        };

        public static SKPaint WindowZoomPaintCrosshair { get; set; } = new SKPaint()
        {
            Color = SKColors.Gold,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f,
        };
        public static SKPaint HelpLinesPaint { get; set; } = new SKPaint()
        {
            Color = SKColors.Gold,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
        };
        public static SKPaint WindowBasePaintBackground { get; set; } = new SKPaint()
        {
            Color = new SKColor(0x70, 0x80, 0x90, 0xA0),
            Style = SKPaintStyle.Fill,
            StrokeWidth = 1f,
            BlendMode = SKBlendMode.Multiply,
        };
        public static SKPaint LabelPaintBackgroud { get; set; } = new SKPaint()
        {
            Color = new SKColor(0x70, 0x80, 0x90, 0xA0),
            Style = SKPaintStyle.Fill,
            StrokeWidth = 1f,
            BlendMode = SKBlendMode.Multiply,
        };
        public static SKPaint ToolBasePaint { get; set; } = new SKPaint()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            IsAntialias = true,
        };
        public static SKPaint ToolBasePaintBounds { get; set; } = new SKPaint()
        {
            Color = SKColors.PaleGoldenrod,
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash(new float[] { 10f, 5f }, 15f),
            StrokeWidth = 1f,
        };
        public static SKPaint ToolBasePaintEndpoint { get; set; } = new SKPaint()
        {
            Color = SKColors.PaleGoldenrod,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
        };
        public static SKPaint ToolPaintMarkPhi { get; set; } = new SKPaint()
        {
            Color = SKColors.Gold,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            IsAntialias = true,
        };
        public static SKPaint LabelPaintText { get; set; } = new SKPaint()
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            StrokeWidth = 1f,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Menlo Regular", SKFontStyleWeight.Normal, SKFontStyleWidth.Expanded, SKFontStyleSlant.Upright),
            TextSize = 12f,
        };
        public static SKPaint WindowBasePaintText { get; set; } = new SKPaint()
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            StrokeWidth = 1f,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Menlo Regular", SKFontStyleWeight.Normal, SKFontStyleWidth.Expanded, SKFontStyleSlant.Upright),
            TextSize = 12f,
        };
    }
}
