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
using System.Collections.Generic;
using System.Reflection;

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
        // Services to use for Core
        private ILogService logService;
        private IScreenService screenService;
        private ISettingsService<AppState> appStateService;
        private ISettingsService<AppSettings> appSettingsService;

        // Refresh timer
        private Timer timer;

        // UI components
        private HelpLines helpLines;
        private Window windowApp;
        private WindowTextInfo windowInfo;
        private WindowZoom windowZoom;

        // The available tools and actions
        private AppTools appTools;
        private AppActions appActions;

        // State
        private SKPoint lastPos;

        /// <summary>
        /// True if the tool is locked, false otherwise.
        /// What locked means vary by tool but can be seen as
        /// lock to grid/axis/proportion.
        /// </summary>
        public bool ToolLocked
        {
            get
            {
                return appTools.ActiveTool.Locked;
            }
            set
            {
                appTools.ActiveTool.Locked = value;
            }
        }

        #region Services

        /// <summary>
        /// The collection of services of Phiddle.
        /// Important: all services must be added before call to <see cref="Initialize"/>.
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// The service provider of Phiddle
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }


        #endregion

        #region UI Component properties

        /// <summary>
        /// The factor (of screen height) the windows will be scaled down with
        /// Example: a screen height 1200 and a factor of 6 will make any window
        /// 200 in height.
        /// </summary>
        public float WindowsSizeFactor { get; set; }

        public float WindowZoomFactor { get; set; }

        public SKPoint ZoomWindowLocation { get; set; }

        /// <summary>
        /// The size of the Zoom Window, local coordinates
        /// </summary>
        public SKSize ZoomWindowSize 
        { 
            get
            {
                var screen = screenService.Dimensions();
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
                var screen = screenService.Dimensions();
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
                var td = screenService.Dimensions();
                var size = new SKSize(td.Width, td.Height);
                return size;
            }
        }

        #endregion

        #region Start/Stop/Initialization
        /// <summary>
        /// Create an instance of Phiddle Core.
        /// Important: Use <see cref="Initialize"/> to actually initialize Phiddle to run.
        /// </summary>
        public PhiddleCore()
        {
            Services = new ServiceCollection();
        }

        /// <summary>
        /// Initialize Phiddle to run. This means creating all UI compontents and make
        /// the service provider ready to serve.
        /// </summary>
        public void Initialize()
        {
            lastPos = SKPoint.Empty;

            // Setup services            
            InitializeServices(Services);
            ServiceProvider = Services.BuildServiceProvider();
            screenService = ServiceProvider.GetRequiredService<IScreenService>();
            logService = ServiceProvider.GetRequiredService<ILogService>();

            appSettingsService = ServiceProvider.GetRequiredService<SettingsService<AppSettings>>();
            appStateService = ServiceProvider.GetRequiredService<SettingsService<AppState>>();

            // Actions
            appActions = InitializeActions();

            // Screen
            var s = screenService.Dimensions();

            // Tools
            appTools = new AppTools(s, appStateService.Settings, appSettingsService.Settings);
            helpLines = new HelpLines(s, appStateService.Settings, appSettingsService.Settings.PaintHelpLines);

            WindowsSizeFactor = appSettingsService.Settings.WindowSizeFactor;
            WindowZoomFactor = appSettingsService.Settings.WindowZoomFactor;

            // Windows
            windowApp = new Window(SKPoint.Empty, ToolWindowSize, appSettingsService.Settings.WindowApp);
            windowInfo = new WindowTextInfo(SKPoint.Empty, InfoWindowSize, appSettingsService.Settings.WindowInfo); 
            windowZoom = new WindowZoom(SKPoint.Empty, ZoomWindowSize, appSettingsService.Settings.WindowZoom) 
            { 
                CrosshairVisible = !helpLines.Visible }
            ;

            // Calculate initial locations of windows with relative positions
#if DEBUG
            var wm = appSettingsService.Settings.WindowMargin + windowApp.PaintBorder.StrokeWidth;
#else
            var wm = appSettingsService.Settings.WindowMargin;
#endif
            var zx = s.Right - windowZoom.Bounds.Width - wm * 2;
            var zy = s.Bottom - windowZoom.Bounds.Height - wm * 2;
            ZoomWindowLocation = new SKPoint(zx, zy);

            var ix = s.Right - windowInfo.Bounds.Width - wm * 2;
            var iy = s.Bottom - windowInfo.Bounds.Height - windowZoom.Bounds.Height - wm * 4;
            InfoWindowLocation = new SKPoint(ix, iy);

            // Setup refresh timer
            timer = new Timer(50) { AutoReset = true };
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);

            logService.Debug("Initialize", $"Core initialized with refresh rate = {1000 / timer.Interval:0} FPS");
        }

        /// <summary>
        /// Start Phiddle Core. 
        /// Note: Phiddle will currently run without being started but the Zoom Window 
        /// will not be updated if not.
        /// </summary>
        public void StartUp()
        {
            timer.Start();
            logService.Debug("StartUp", "Core running");
        }

        /// <summary>
        /// Stop Phiddle Core.
        /// </summary>
        public void ShutDown()
        {
            appStateService.Settings.HelpLinesVisible = helpLines.Visible;
            appStateService.Settings.WindowInfoVisible = windowInfo.Visible;
            appStateService.Settings.WindowZoomVisible = windowZoom.Visible;
            appStateService.Settings.ActiveTool = appTools.ActiveTool.ToolId;
            appStateService.Settings.LabelLocation = appTools.LabelLocation; 
            appStateService.Settings.MarksVisible = appTools.MarksVisible;
            appStateService.Settings.ToolWideLinesOn = appTools.WideLinesOn;
            appStateService.Save();

            timer.Stop();
            logService.Debug("ShutDown", "Core stopped");
        }

        #endregion

        #region User Input

        public void InvokeAction(ActionId actionId) => appActions.Invoke(actionId);

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
            if (!appTools.ActiveTool.Visible)
            {
                return;
            }

            // Update tool with new mouse position. Effect of new position depends on tool state
            if (appTools.ActiveTool.Resizing)
            {
                screenService.MouseState = MouseState.Normal;
                appTools.ActiveTool.Resize(p);
            }
            else if (appTools.ActiveTool.Moving)
            {
                screenService.MouseState = MouseState.Moving;
                appTools.ActiveTool.Move(p - lastPos);
            }
            else
            {
                // We have a visible but 'passive' tool. Check position against tool bounds and update cursor
                appTools.ActiveTool.CheckBounds(p);
                screenService.MouseState = appTools.ActiveTool.Movable || appTools.ActiveTool.Resizable ? MouseState.CanGrip : MouseState.Normal;
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
            appTools.ActiveTool.NextAction(p);
        }

        #endregion

        #region UI Drawing

        /// <summary>
        /// Draw Phiddle Core Tool at canvas <paramref name="c"/>
        /// </summary>
        /// <param name="c">Canvas to draw tool on</param>
        public void DrawTool(SKCanvas c)
        {
            helpLines.Draw(c);

#if DEBUG
            windowApp.Draw(c);
#endif
            appTools.ActiveTool.Draw(c);
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

        #endregion

        #region UI Updating

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
            var screenshot = screenService.Capture(rect);
            windowZoom.UpdateZoom(screenshot, WindowZoomFactor);

            // Request repaint of zoom window by using any point within it
            var invalidateAtPos = new SKPointI((int)ZoomWindowLocation.X + 1, (int)ZoomWindowLocation.Y + 1);
            screenService.Invalidate(invalidateAtPos);
        }

        /// <summary>
        /// Update the Info Window with new mouse position (and state of current tool).
        /// </summary>
        /// <param name="p">New mouse position to show</param>
        private void UpdateInfoWindow(SKPoint p)
        {
            windowInfo.ReportSelectedTool(appTools.ActiveTool);
            windowInfo.ReportMousePosition(p);
            windowInfo.ReportLabelPlacement(appTools.ActiveTool);
            windowInfo.ReportMeasurements(appTools.ActiveTool);
            var invalidateAtPos = new SKPointI((int)InfoWindowLocation.X + 1, (int)InfoWindowLocation.Y + 1);
            screenService.Invalidate(invalidateAtPos);
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
            var zoomAtPos = appTools.ActiveTool.Resizing 
                ? appTools.ActiveTool.ActiveEndpoint.Pos 
                : screenService.MousePosition();
            UpdateZoomWindow(zoomAtPos);
        }

        #endregion

        #region Private Methods

        private void InitializeServices(IServiceCollection services)
        {
            services.AddSingleton<ILogService, LogServiceConsole>();
            services.AddSingleton<SettingsService<AppState>>();
            services.AddSingleton<SettingsService<AppSettings>>();
        }

        private AppActions InitializeActions()
        {
            var phiddleActions = new Dictionary<ActionId, ActionDelegate>(Enum.GetValues(typeof(ActionId)).Length)
            {
                { ActionId.ApplicationExit, () => ShutDown()},
                { ActionId.HelpLinesToggleVisible, () => { helpLines.Visible = !helpLines.Visible; windowZoom.CrosshairVisible = !helpLines.Visible; } },
                { ActionId.LabelTogglePlacement, () => appTools.ToggleLabelPlacement() },
                { ActionId.ToolMarksGoldenRatioToggleVisible, () => appTools.ToggleToolMarksVisibility(MarkId.Phi) },
                { ActionId.ToolMarksEndpointToggleVisible, () => appTools.ToggleToolMarksVisibility(MarkId.Endpoint) },
                { ActionId.ToolMarksMiddleToggleVisible, () => appTools.ToggleToolMarksVisibility(MarkId.Middle) },
                { ActionId.ToolMarksThirdToggleVisible, () => appTools.ToggleToolMarksVisibility(MarkId.Third) },
                { ActionId.ToolSelectNext, () => { appTools.SelectNextTool(); windowInfo.ReportSelectedTool(appTools.ActiveTool); windowInfo.ReportMeasurements(appTools.ActiveTool);} },
                { ActionId.WindowInfoToggleVisible, () => windowInfo.Visible = !windowInfo.Visible },
                { ActionId.WindowZoomToggleVisible, () => { windowZoom.Visible = !windowZoom.Visible;if (windowZoom.Visible) timer.Start(); else timer.Stop();}},
                { ActionId.ToolToggleThickness, () =>  appTools.WideLinesOn = !appTools.WideLinesOn },
            };

            return new AppActions() { Actions = phiddleActions };
        }

        #endregion
    }
}
