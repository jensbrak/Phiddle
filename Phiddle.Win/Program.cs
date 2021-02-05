using System;
using System.Windows.Forms;

namespace Phiddle.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Issues with .NET5, scaling and Transparency...
            // Findings so far:
            // https://github.com/dotnet/winforms/issues/135  (issue leading to addition of SetHighDpiMode)
            // https://github.com/dotnet/winforms/commit/4608b70a8534b8168c718431b316ba7a6cecfa16 (actual commit AFAICS)
            // https://github.com/dotnet/winforms/blob/33e683aaf6a4ecaa87d31d3ee98de00f6db1ea2f/src/System.Windows.Forms.Primitives/src/System/Windows/Forms/Internals/DpiHelper.cs#L363 (current code)
            // https://github.com/dotnet/winforms/commit/c8f07293e07ba94d886698328d8d504eb527c6a1 (template source code)
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PhiddleForm());
        }
    }
}
