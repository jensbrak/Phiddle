using Microsoft.Extensions.DependencyInjection;
using Phiddle.Core.Extensions;
using Phiddle.Core.Graphics;
using Phiddle.Core.Services;
using Phiddle.Core.Settings;
using Phiddle.Core.Measure;
using SkiaSharp;
using System;
using System.Timers;
using System.IO;

namespace Phiddle.Core
{
    /// <summary>
    /// PhiddleCore provides all platform independent functionality of Phiddle.
    /// Platform specific code access PhiddleCore using its interface directly.
    /// PhiddleCore access platform specific code by using registred services.
    /// 
    /// Note: PhiddleCore use SkiaSharp for all rendering, along with SkiaSharp
    /// entities for dealing with 2D graphics. It is, however, unaware of what
    /// context is being used for the rendering. The interface between PhiddleCore
    /// and the surface in use by SkiaSharp is the SKCanvas object that is provided
    /// in the drawing methods of PhiddleCore.
    /// 
    /// Basic concepts of Phiddle Core:
    /// 
    /// <list type="table">
    /// <item>Tool: a screen pixel measuring tool</item>
    /// <item>Info Window: window with information with current state of Phiddle</item>
    /// <item>Zoom Window: window with a zoomed in portion of the screen (where mouse is)</item>
    /// <item>Label: A minimalistic window with measuring data that follow the tool</item>
    /// <item>Help lines: screen wide cross intersecting at mouse position</item>
    /// <item>"Passive": A tool that has been placed on screen and is not moved or resized</item>
    /// <item>Endpoint: Grip to resize a passive tool. Each tool has two endpoints: upper left and bottom right corner</item>
    /// </list>
    /// 
    /// Note on state of a tool:
    /// 
    /// <list type="bullet">
    /// <item>Mouse click will advance Phiddle and its current tool to next state</item>
    /// <item>Current state along with where mouse click occur affect what will happen next:</item>
    /// <item>No tool visible: start measuring with tool at current mouse position</item>
    /// <item>Tool resizing: stop resize at current mouse position</item>
    /// <item>Tool passive and mouse outside tool bounds:  hide it</item>
    /// <item>Tool passive and mouse inside tool bounds: start moving it</item>
    /// <item>Tool passive and mouse between endpoint bounds (either one of them): start resizing at selected endpoint </item>
    /// </list>
    /// </summary>
    public class PhiddleCore
    {
        private Timer timer;

        private WindowBase windowTool;
        private WindowInfo windowInfo;
        private WindowZoom windowZoom;
        private ToolSet toolSet;
        private HelpLines helpLines;
        private SKPoint lastPos;
        
        /// <summary>
        /// The collection of services of Phiddle.
        /// Important: all services must be added before call to <see cref="Initialize"/>.
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// The service provider of Phiddle
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Phiddle Core logging instance.
        /// </summary>
        public ILoggingService Log { get; private set; }

        /// <summary>
        /// Screen service instance.
        /// </summary>
        public IScreenService Screen { get; private set; }


        /// <summary>
        /// True if help lines (at mouse cursor) is visible, false otherwise
        /// Note: if visible, the crosshair in Zoom Window is hidden, since
        /// it will be obscured by the help lines.
        /// </summary>
        public bool HelpLinesVisible
        {
            get
            {
                return helpLines.Visible;
            }
            set
            {
                helpLines.Visible = value;
                windowZoom.CrosshairVisible = !value;
            }
        }

        /// <summary>
        /// True if Info Window is visible, false otherwise
        /// </summary>
        public bool InfoWindowVisible
        {
            get
            {
                return windowInfo.Visible;
            }
            set
            {
                windowInfo.Visible = value;
            }
        }

        /// <summary>
        /// True if Zoom window is visible, false otherwise
        /// </summary>
        public bool ZoomWindowVisible 
        { 
            get
            {
                return windowZoom.Visible;
            }
            set
            {
                windowZoom.Visible = value;

                if (windowZoom.Visible)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// True if the tool is locked, false otherwise.
        /// What locked means vary by tool but can be seen as
        /// lock to grid/axis/proportion.
        /// </summary>
        public bool ToolLocked
        {
            get
            {
                return toolSet.SelectedTool.Locked;
            }
            set
            {
                toolSet.SelectedTool.Locked = value;
            }
        }

        /// <summary>
        /// The factor (of screen height) the windows will be scaled down with
        /// Example: a screen height 1200 and a factor of 6 will make any window
        /// 200 in height.
        /// </summary>
        public float WindowsSizeFactor { get; set; } = Defaults.WindowsSizeFactor;

        public float WindowZoomFactor { get; set; } = Defaults.WindowZoomFactor;

        public SKPoint ZoomWindowLocation { get; set; }

        /// <summary>
        /// The size of the Zoom Window, local coordinates
        /// </summary>
        public SKSize ZoomWindowSize 
        { 
            get
            {
                var screen = Screen.Dimensions();
                var ratio = (float)screen.Width / screen.Height;
                var zw = screen.Width / (WindowsSizeFactor * ratio);
                var zh = screen.Height / WindowsSizeFactor;
                var size = new SKSize(zw, zh);
                return size;
            }
        }

        public SKPoint InfoWindowLocation { get; set; }

        /// <summary>
        /// The size of the Info Window, local coordinates
        /// </summary>
        public SKSize InfoWindowSize
        {
            get
            {
                var screen = Screen.Dimensions();
                var ratio = (float)screen.Width / screen.Height;
                var iw = screen.Width / (WindowsSizeFactor * ratio);
                var ih = screen.Height / WindowsSizeFactor;
                var size = new SKSize(iw, ih);
                return size;
            }
        }

        public SKPoint ToolWindowLocation { get; set; }

        /// <summary>
        /// The size of the Tool Window
        /// </summary>
        public SKSize ToolWindowSize
        {
            get
            {
                var td = Screen.Dimensions();
                var size = new SKSize(td.Width, td.Height);
                return size;
            }
        }

        /// <summary>
        /// Create an instance of Phiddle Core.
        /// Important: Use <see cref="Initialize"/> to actually initialize Phiddle to run.
        /// </summary>
        public PhiddleCore()
        {
            Services = new ServiceCollection();
            ConfigureServices(Services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
            /*var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("phiddlesettings.json", false)
            .Build();*/
            services.AddSingleton<ILoggingService, LoggingServiceConsole>();
            //services.AddOptions();
            //services.Configure<PhiddleSettings>(config);
        }

        /// <summary>
        /// Initialize Phiddle to run. This means creating all UI compontents and make
        /// the service provider ready to serve.
        /// </summary>
        public void Initialize()
        {
            lastPos = SKPoint.Empty;

            // Setup services
            ServiceProvider = Services.BuildServiceProvider();
            Screen = ServiceProvider.GetRequiredService<IScreenService>();
            Log = ServiceProvider.GetRequiredService<ILoggingService>();
            
            // Create tools
            toolSet = new ToolSet();

            // Create UI components
#if DEBUG
            windowTool = new WindowBase(ToolWindowSize.CombineWith(ToolWindowLocation));
#endif
            helpLines = new HelpLines(Screen.Dimensions());
            windowInfo = new WindowInfo(InfoWindowSize.CombineWith(InfoWindowLocation)); 
            windowZoom = new WindowZoom(ZoomWindowSize.CombineWith(ZoomWindowLocation)) { CrosshairVisible = !helpLines.Visible };

            // Calculate initial locations of UI components
            var s = Screen.Dimensions();
            ZoomWindowLocation = new SKPoint(s.Right - windowZoom.Bounds.Width - 4, s.Bottom - windowZoom.Bounds.Height - 4);
            InfoWindowLocation = new SKPoint(s.Right - windowInfo.Bounds.Width - 4, s.Bottom - windowInfo.Bounds.Height - windowZoom.Bounds.Height - 7);

            // Setup refresh timer
            timer = new Timer(50) { AutoReset = true };
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);

            Log.Debug("Initialize", $"Core initialized with refresh rate = {1000 / timer.Interval:0} FPS");
        }

        /// <summary>
        /// Start Phiddle Core. 
        /// Note: Phiddle will currently run without being started but the Zoom Window 
        /// will not be updated if not.
        /// </summary>
        public void Start()
        {
            // Initial update
            //var pos = Screen.MousePosition();
            //UpdateInfoWindow(pos);
            //UpdateZoomWindow(pos);

            timer.Start();
            Log.Debug("Start", "Core running");
        }

        /// <summary>
        /// Stop Phiddle Core.
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            Log.Debug("Exit", "Core stopped");
        }

        /// <summary>
        /// Update Phiddle Core with a mouse position <paramref name="p"/>
        /// </summary>
        /// <param name="p">Position of mouse (cursor)</param>
        public void MouseMoved(SKPoint p)
        {
            // Always make info window show latest position and help lines are properly positioned
            UpdateInfoWindow(p);
            helpLines.Pos = p;

            // Nothing else to update?
            if (!toolSet.SelectedTool.Visible)
            {
                return;
            }

            // Update tool with new mouse position. Effect of new position depends on tool state
            if (toolSet.SelectedTool.Resizing)
            {
                Screen.MouseState = MouseState.Normal;
                toolSet.SelectedTool.Resize(p);
            }
            else if (toolSet.SelectedTool.Moving)
            {
                Screen.MouseState = MouseState.Moving;
                toolSet.SelectedTool.Move(p - lastPos);
            }
            else
            {
                // We have a visible but 'passive' tool. Check position against tool bounds and update cursor
                toolSet.SelectedTool.CheckBounds(p);
                Screen.MouseState = toolSet.SelectedTool.Movable || toolSet.SelectedTool.Resizable ? MouseState.CanGrip : MouseState.Normal;
            }

            // Store pos for next round
            lastPos = p;
        }

        /// <summary>
        /// Update Phiddle Core with a mouse click at position <paramref name="p"/>.
        /// </summary>
        /// <param name="p">Position where (mouse) clicked occured</param>
        public void MouseClicked(SKPoint p)
        {
            // Let tool decide what mouse click means and update itself
            toolSet.SelectedTool.NextAction(p);
        }

        /// <summary>
        /// Select next tool of Phiddle Core. 
        /// Note: If previous tool was last one available, start over with first one.
        /// </summary>
        public void SelectNextTool()
        {
            toolSet.SelectNextTool();
            windowInfo.ReportSelectedTool(toolSet.SelectedTool);
            windowInfo.ReportMeasurements(toolSet.SelectedTool);
        }

        /// <summary>
        /// Select next label placement of the tools.
        /// Note: if previous placement was last one available, start over with the first one.
        /// </summary>
        public void ToggleLabelPlacement()
        {
            toolSet.ToggleLabelPlacement();
        }

        public void ToggleToolMarks(MarkCategory c)
        {
            toolSet.ToggleToolMarks(c);
        }

        /// <summary>
        /// Draw Phiddle Core Tool at canvas <paramref name="c"/>
        /// </summary>
        /// <param name="c">Canvas to draw tool on</param>
        public void DrawTool(SKCanvas c)
        {
            helpLines.Draw(c);

#if DEBUG
            windowTool.Draw(c);
#endif
            toolSet.SelectedTool.Draw(c);
        }

        /// <summary>
        /// Draw Phiddle Core Info Window at canvas <paramref name="c"/>
        /// </summary>
        /// <param name="c">Canvas to draw Info Window on</param>
        public void DrawInfoWindow(SKCanvas c)
        {
            windowInfo.Draw(c);
        }

        /// <summary>
        /// Draw Phiddle Core Zoom Window at canvas <paramref name="c"/>
        /// </summary>
        /// <param name="c">Canvas to draw Zoom Window on</param>
        public void DrawZoomWindow(SKCanvas c)
        {
            windowZoom.Draw(c);
        }

        /// <summary>
        /// Update the Zoom Window with a new mouse position. This means mouse have been moved and 
        /// the Zoom Window needs to refresh its contents.
        /// </summary>
        /// <param name="p">New mouse position to zoom in at</param>
        private void UpdateZoomWindow(SKPoint p)
        {
            // Do we need to update at all?
            if (!windowZoom.Visible)
            {
                return;
            }

            // Get bounds of screen area to capture 
            var w = windowZoom.Bounds.Width / WindowZoomFactor;
            var h = windowZoom.Bounds.Height / WindowZoomFactor;
            var x = (int)(p.X - w / 2f);
            var y = (int)(p.Y - h / 2f);
            var rect = new SKRectI(x, y, x + (int)w, y + (int)h);

            // Perform the capture and update zoom window with it
            var screenshot = Screen.Capture(rect);
            windowZoom.UpdateZoom(screenshot, WindowZoomFactor);

            // Request repaint of zoom window by using any point within it
            var invalidateAtPos = new SKPointI((int)ZoomWindowLocation.X + 1, (int)ZoomWindowLocation.Y + 1);
            Screen.Invalidate(invalidateAtPos);
        }

        /// <summary>
        /// Update the Info Window with new mouse position (and state of current tool).
        /// </summary>
        /// <param name="p">New mouse position to show</param>
        private void UpdateInfoWindow(SKPoint p)
        {
            windowInfo.ReportSelectedTool(toolSet.SelectedTool);
            windowInfo.ReportMousePosition(p);
            windowInfo.ReportLabelPlacement(toolSet.SelectedTool);
            windowInfo.ReportMeasurements(toolSet.SelectedTool);
            var invalidateAtPos = new SKPointI((int)InfoWindowLocation.X + 1, (int)InfoWindowLocation.Y + 1);
            Screen.Invalidate(invalidateAtPos);
        }

        /// <summary>
        /// Process a 'heart beat' of Phiddle Core. Currently, this means to update Zoom Window
        /// with new content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Zoom at mousepos, unless we're resizing a locked tool. If so: get locked position of tool
            var zoomAtPos = toolSet.SelectedTool.Resizing ? toolSet.SelectedTool.ActiveEndpoint.Pos : Screen.MousePosition();
            UpdateZoomWindow(zoomAtPos);
        }
    }
}
