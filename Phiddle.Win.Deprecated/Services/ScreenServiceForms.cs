using Phiddle.Core.Services;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Phiddle.Win.Extensions;
using Phiddle.Core;

namespace Phiddle.Win.Services
{
    /// <summary>
    /// A ScreenService dependent on (Windows) Forms in order to implement IScreenService
    /// </summary>
    public class ScreenServiceForms : IScreenService
    {
        private MouseState mouseState;
        public PhiddleForm PhiddleForm { get; set; }
        public MouseState MouseState
        {
            get
            {
                return mouseState;
            }
            set
            {
                var control = PhiddleForm.GetChildAtPoint(Cursor.Position);
                switch (value)
                {
                    case MouseState.Normal:
                        control.Cursor = Cursors.Default;
                        break;
                    case MouseState.CanGrip:
                        control.Cursor = Cursors.Hand;
                        break;
                    case MouseState.Moving:
                        control.Cursor = Cursors.SizeAll;
                        break;
                    case MouseState.Resizing:
                        control.Cursor = Cursors.SizeAll;
                        break;
                    case MouseState.Blocked:
                        control.Cursor = Cursors.No;
                        break;
                    default:
                        control.Cursor = Cursors.Default;
                        break;
                }
                mouseState = value;
            }
        }

        /// <summary>
        /// Capture screen defined by given rectangle
        /// </summary>
        /// <param name="rect">Area to capture</param>
        /// <returns>Captured area as an image</returns>
        public SKImage Capture(SKRectI rect)
        {
            var r = rect.ToDrawingRect();

            using (var bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(r.Location, Point.Empty, r.Size, CopyPixelOperation.SourceCopy);
                }
                return bitmap.ToSKImage();
            }
        }

        /// <summary>
        /// Get dimensions of screen currently in use
        /// </summary>
        /// <returns>Rectangle with bounds of screen</returns>
        public SKRectI Dimensions()
        {
            var screen = Screen.FromPoint(Cursor.Position).Bounds;
            return new SKRectI(0, 0, screen.Width, screen.Height);
        }

        /// <summary>
        /// Get position of mouse
        /// </summary>
        /// <returns></returns>
        public SKPointI MousePosition()
        {
            return GetMousePosition(false);
        }

        private SKPointI GetMousePosition(bool useHotSpot)
        {
            if (useHotSpot)
            {
                var hotSpot = Cursor.Current.HotSpot;
                var position = new Point(Cursor.Position.X + hotSpot.X, Cursor.Position.Y + hotSpot.Y);
                return position.ToSKPoint();
            }
            else
            {
                return Cursor.Position.ToSKPoint();
            }
        }

        /// <summary>
        /// Invalidate screen at current position, which in this case means the Windows Forms control that is 
        /// at the given position.
        /// </summary>
        /// <param name="pos">Position to invalidate</param>
        public void Invalidate(SKPointI pos)
        {
            if (PhiddleForm.IsDisposed)
            {
                PhiddleForm.Log.Warning("ScreenServiceForms.Invalidate", "Form is allready disposed");
            }

            try
            {


                PhiddleForm.InvokeIfRequired(() =>
                {
                    var control = PhiddleForm.GetChildAtPoint(pos.ToDrawingPoint());

                    if (control != null)
                    {
                        control.Invalidate();
                    }
                });
            }
            catch (System.Exception ex)
            {
                PhiddleForm.Log.Error("ScreenServiceForms.Invalidate", "Failed to invoke control", ex);
            }

        }

    }
}
