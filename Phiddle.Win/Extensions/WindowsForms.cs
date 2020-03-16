using System.ComponentModel;
using System.Windows.Forms;

namespace Phiddle.Win.Extensions
{
    /// <summary>
    /// Extensions to Windows Forms.
    /// </summary>
    public static class WindowsForms
    {
        /// <summary>
        /// Perform an action on an object that may be on another thread.
        /// Intended use: access Windows Forms components from outside of UI thread.
        /// </summary>
        /// <param name="obj">Component to access</param>
        /// <param name="action">Action to perform</param>
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                obj.Invoke(action, new object[0]);
            }
            else
            {
                action();
            }
        }
    }
}
