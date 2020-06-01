using Phiddle.Core.Settings;
using Phiddle.Core.Measure;
using SkiaSharp;
using System.Collections.Generic;

namespace Phiddle.Core.Settings
{
    public class SettingsPaint : ISettings
    {
        public SKColor Color { get; set; } = SKColors.Red;
        public SKPaintStyle Style { get; set; } = SKPaintStyle.Stroke;
        public float StrokeWidth { get; set; } = 1f;
        public bool IsAntialias { get; set; } = false;
        public string TypeFaceFamilyName { get; set; } = "Menlo Regular";
        public float TextSize { get; set; } = 12f;
        public SKBlendMode BlendMode { get; set; } = SKBlendMode.SrcOver;
        public SKPaint ToSKPaint()
        {
            var paint = new SKPaint()
            {
                Color = Color,
                Style = Style,
                StrokeWidth = StrokeWidth,
                IsAntialias = IsAntialias,
                Typeface = SKTypeface.FromFamilyName(TypeFaceFamilyName, SKFontStyleWeight.Normal, SKFontStyleWidth.Expanded, SKFontStyleSlant.Upright),
                TextSize = TextSize,
                BlendMode = BlendMode,
            };

            return paint;
        }
    }

    public class SettingsMark : ISettings
    {
        public float[] Pos { get; set; } = new float[] { };
        public float Size { get; set; } = 10f;
        public SettingsPaint PaintMark { get; set; } = new SettingsPaint();
    }

    public class SettingsTool : ISettings
    {
        public float BoundsPad { get; set; } = 4f;
        public SettingsToolFrame SettingsToolFrame{ get; set; } = new SettingsToolFrame();
        public SettingsPaint PaintTool { get; set; } = new SettingsPaint()
        {
            IsAntialias = true,
        };
        public SettingsPaint PaintEndpoint { get; set; } = new SettingsPaint()
        {
            Color = SKColors.Goldenrod,
        };
        public SettingsWindowLabel Label { get; set; } = new SettingsWindowLabel();
        public float SizeEndpoint { get; set; } = 12f;
        public Dictionary<MarkId, SettingsMark> Marks { get; set; } = new Dictionary<MarkId, SettingsMark>()
        {
            {
                MarkId.Phi, new SettingsMark()
                {
                    Pos = new float[] { 1f - Constants.PhiInv, Constants.PhiInv },
                    PaintMark = new SettingsPaint() { Color = SKColors.Gold, IsAntialias = true, },
                }
            },
            {
                MarkId.Middle, new SettingsMark()
                {
                    Pos = new float[] { 0.5f },
                    PaintMark = new SettingsPaint() { IsAntialias = true, },
                }
            },
            {
                MarkId.Third, new SettingsMark()
                {
                    Pos = new float[] { 1f / 3f, 2f / 3f },
                    PaintMark = new SettingsPaint() { IsAntialias = true, },
                }
            },
            {
                MarkId.Endpoint, new SettingsMark()
                {
                    Pos = new float[] { 0f, 1f },
                    PaintMark = new SettingsPaint() { IsAntialias = true, },
                }
            },
        };
    }

    public abstract class SettingsWindow : ISettings
    {
        public abstract SettingsPaint PaintBorder { get; set; }
        public abstract SettingsPaint PaintText { get; set; }
        public abstract SettingsPaint PaintBackground { get; set; }
        public abstract bool Transparent { get; set; }
    }

    public class SettingsWindowZoom : SettingsWindow
    {
        public override SettingsPaint PaintBorder { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintText { get; set; } = new SettingsPaint()
        {
            Color = SKColors.WhiteSmoke,
            IsAntialias = true,
        };
        public override SettingsPaint PaintBackground { get; set; } = new SettingsPaint()
        {
            Color = new SKColor(0x70, 0x80, 0x90, 0xA0),
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Multiply,
        };
        public override bool Transparent { get; set; } = true;

        public float CrossHairSize { get; set; } = 20f;
        public SettingsPaint PaintCrosshair { get; set; } = new SettingsPaint()
        {
            Color = SKColors.Gold,
            StrokeWidth = 3f,
        };
    }

    public class SettingsWindowText : SettingsWindow
    {
        public override SettingsPaint PaintBorder { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintText { get; set; } = new SettingsPaint()
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };
        public override SettingsPaint PaintBackground { get; set; } = new SettingsPaint()
        {
            Color = new SKColor(0x70, 0x80, 0x90, 0xA0),
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Multiply,
        };
        public override bool Transparent { get; set; } = false;
        public float TextLeftMargin { get; set; } = 65f;
    }

    public class SettingsWindowLabel : SettingsWindow
    {
        public override SettingsPaint PaintBorder { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintText { get; set; } = new SettingsPaint()
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };
        public override SettingsPaint PaintBackground { get; set; } = new SettingsPaint()
        {
            Color = new SKColor(0x70, 0x80, 0x90, 0xA0),
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Multiply,
        };
        public override bool Transparent { get; set; } = false;
        public float TextPad { get; set; } = 4f;
    }

    public class SettingsToolFrame : SettingsWindow
    {
        public override SettingsPaint PaintBorder { get; set; } = new SettingsPaint()
        {
            Color = SKColors.Goldenrod,
        };

        public override SettingsPaint PaintText { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintBackground { get; set; } = new SettingsPaint();
        public override bool Transparent { get; set; } = true;

    }

    public class SettingsWindowApp : SettingsWindow
    {
        public override SettingsPaint PaintBorder { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintText { get; set; } = new SettingsPaint();
        public override SettingsPaint PaintBackground { get; set; } = new SettingsPaint();
        public override bool Transparent { get; set; } = true;

    }
    public class AppSettings : ISettings
    {
        public float WindowSizeFactor { get; set; } = 5f;
        public float WindowZoomFactor { get; set; } = 5f;
        public float WindowMargin { get; set; } = 1f;
        public SettingsWindowApp WindowApp { get; set; } = new SettingsWindowApp();
        public SettingsWindowText WindowInfo { get; set; } = new SettingsWindowText();
        public SettingsWindowZoom WindowZoom { get; set; } = new SettingsWindowZoom();
        public SettingsTool Tool { get; set; } = new SettingsTool();
        public SettingsPaint PaintHelpLines { get; set; } = new SettingsPaint()
        {
            Color = SKColors.Gold
        };
    }
}