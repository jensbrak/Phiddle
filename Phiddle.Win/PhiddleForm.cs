using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Phiddle.Core;
using Phiddle.Core.Services;
using Phiddle.Core.Settings;
using Phiddle.Win.Services;
using Phiddle.Win.Settings;

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
        private AppInput appInput;
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
            phiddle.Services.AddSingleton(screen);
            phiddle.Services.AddSingleton<LoggingService>();
            phiddle.Services.AddSingleton<SettingsService<AppInput>>();
            phiddle.Initialize();

            // Get the services we need right away
            Log = PhiddleCore.ServiceProvider.GetRequiredService<LoggingService>();
            var settingsService = PhiddleCore.ServiceProvider.GetRequiredService<SettingsService<AppInput>>();

            appInput = settingsService.Settings;

            if (!settingsService.Loaded)
            {
                settingsService.Settings = AppInputWin.Defaults;
                settingsService.Save();
                appInput = AppInputWin.Defaults;
            }
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
            var keyCode = Convert.ToUInt16(e.KeyCode);
            if (!appInput.Keys.ContainsKey(keyCode))
            {
                return;
            }

            var action = appInput.Keys[keyCode];
            phiddle.InvokeAction(action);

            if (action == ActionId.ApplicationExit)
            {
                Close();
            }
            else
            {
                // NeedsDisplay = true;
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
