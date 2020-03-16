using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Phiddle.Core;
using Phiddle.Core.Services;
using Phiddle.Win.Services;

namespace Phiddle.Win
{
    /// <summary>
    /// A form that provides surface for drawing Phiddle on desktop and acts as a proxy between Windows and Phiddle.
    /// Any Windows platform specific code goes here or in sub classes of PhiddleForm.
    /// </summary>
    public partial class PhiddleForm : Form
    {
        private SKGLControl controlTool;
        private SKGLControl controlInfo;
        private SKGLControl controlZoom;
        private PhiddleCore phiddle;
        private IScreenService screen;
        public ILoggingService Log { get; set; }

        public PhiddleForm()
        {
            // Order matter
            InitializeComponent();
            InitializePhiddleForm();
            InitializePhiddleCore();
            InitializeSkiaControls();
        }

        private void InitializePhiddleForm()
        {
            screen = new ScreenServiceForms() { PhiddleForm = this };
            StartPosition = FormStartPosition.Manual;
            var bounds = screen.Dimensions().ToDrawingRect();
            Size = bounds.Size;
            Location = bounds.Location;
        }

        private void InitializePhiddleCore()
        {
            // Create the Core, add our services and initialize Core with it
            phiddle = new PhiddleCore();
            phiddle.ServiceCollection.AddSingleton(screen);
            phiddle.ServiceCollection.AddSingleton<LoggingService>();
            phiddle.Initialize();

            // Get a log object from Core to use
            Log = phiddle.ServiceProvider.GetRequiredService<LoggingService>();
        }

        private void InitializeSkiaControls()
        {
            controlTool = new SKGLControl
            {
                Name = "PhiddleTool",
                Bounds = this.Bounds,
                Dock = DockStyle.Fill,
            };

            controlTool.PaintSurface += new EventHandler<SKPaintGLSurfaceEventArgs>(HandlePaintGLSurfaceTool);
            controlTool.MouseMove += new MouseEventHandler(HandleMouseMove);
            controlTool.MouseClick += new MouseEventHandler(HandleMouseClick);
            Controls.Add(controlTool);

            var il = phiddle.InfoWindowLocation;
            var ib = phiddle.InfoWindowSize;
            controlInfo = new SKGLControl
            {
                Name = "PhiddleInfo",
                Location = new Point((int)il.X, (int)il.Y),
                Size = new Size((int)ib.Width, (int)ib.Height),
            };
            controlInfo.PaintSurface += new EventHandler<SKPaintGLSurfaceEventArgs>(HandlePaintGLSurfaceInfo);
            Controls.Add(controlInfo);

            var zl = phiddle.ZoomWindowLocation;
            var zb = phiddle.ZoomWindowSize;
            controlZoom = new SKGLControl
            {
                Name = "PhiddleZoom",
                Location = new Point((int)zl.X, (int)zl.Y),
                Size = new Size((int)zb.Width, (int)zb.Height)
            };
            controlZoom.PaintSurface += new EventHandler<SKPaintGLSurfaceEventArgs>(HandlePaintGLSurfaceZoom);
            Controls.Add(controlZoom);
        }


        private void HandleFormLoad(object sender, EventArgs e)
        {
            // Ready to start Core
            phiddle.Start();
        }

        private void HandlePaintGLSurfaceTool(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // See https://github.com/mono/SkiaSharp/issues/920
            ((SKGLControl)sender).MakeCurrent();
            var bg = TransparencyKey.ToSKColor();
            e.Surface.Canvas.Clear(bg);
            phiddle.DrawTool(e.Surface.Canvas);
        }

        private void HandlePaintGLSurfaceInfo(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // See https://github.com/mono/SkiaSharp/issues/920
            var control = (SKGLControl)sender;
            control.BringToFront();
            control.MakeCurrent();
            var bg = TransparencyKey.ToSKColor();
            e.Surface.Canvas.Clear(bg);
            phiddle.DrawInfoWindow(e.Surface.Canvas);
        }

        private void HandlePaintGLSurfaceZoom(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // See https://github.com/mono/SkiaSharp/issues/920
            var control = (SKGLControl)sender;
            control.BringToFront();
            control.MakeCurrent();
            var bg = TransparencyKey.ToSKColor();
            e.Surface.Canvas.Clear(bg);
            phiddle.DrawZoomWindow(e.Surface.Canvas);
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    phiddle.Stop();
                    Close();
                    break;
                case Keys.Space:
                    phiddle.SelectNextTool();
                    break;
                case Keys.L:
                    phiddle.ToggleLabelPlacement();
                    break;
                case Keys.Z:
                    phiddle.ZoomWindowVisible = !phiddle.ZoomWindowVisible;
                    break;
                case Keys.I:
                    phiddle.InfoWindowVisible = !phiddle.InfoWindowVisible;
                    break;
                case Keys.H:
                    phiddle.HelpLinesVisible = !phiddle.HelpLinesVisible;
                    break;
                case Keys.G:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.GoldenRatio);
                    break;
                case Keys.E:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Endpoint);
                    break;
                case Keys.M:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Middle);
                    break;
                case Keys.T:
                    phiddle.ToggleToolMarks(Core.Measure.MarkCategory.Third);
                    break;
                default:
                    break;
            }
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            phiddle.ToolLocked = ModifierKeys.HasFlag(Keys.Control);
            phiddle.MouseMoved(e.Location.ToSKPoint());
            controlZoom.Invalidate();
            controlTool.Invalidate();
            controlInfo.Invalidate();
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                phiddle.MouseClicked(e.Location.ToSKPoint());
            }
            controlTool.Invalidate();
        }
    }
}
