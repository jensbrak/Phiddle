using System;
using Phiddle.Core.Services;
using Phiddle.Core.Settings;
using System.Text.Json;
using System.IO;
using System.Text.Encodings.Web;


namespace Phiddle.Mac.Services
{
    public class AppStateServiceMac : SettingsServiceBase
    {
        public AppStateServiceMac()
        {
            FileSuffix = ".state" + base.FileSuffix;
        }

        public override bool Load()
        {
            var settingsFileExist = base.Load();

            if (settingsFileExist)
            {
                var fullPath = Path.Combine(FolderPath, FileName + FileSuffix);
                var json = File.ReadAllText(fullPath);
                Settings = JsonSerializer.Deserialize<PhiddleState>(json);
            }
            else
            {
                // Defults
                Settings = new PhiddleState();
            }

            return true;
        }

        public override void Save()
        {
            // Assure settings path is present
            base.Save();

            // Save...
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var json = JsonSerializer.Serialize((PhiddleState)Settings, options);
            var fullPath = Path.Combine(FolderPath, FileName + FileSuffix);
            File.WriteAllText(fullPath, json);
        }
    }
}
