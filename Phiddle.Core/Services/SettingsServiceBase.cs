using System;
using System.IO;
using Phiddle.Core.Settings;

namespace Phiddle.Core.Services
{
    public abstract class SettingsServiceBase : ISettingsService
    {
        public ISettings Settings { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }

        protected string FileSuffix;

        public SettingsServiceBase()
        {
            FileName = Constants.AppName.ToLower();
            FileSuffix = ".json";
            FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public virtual void Save()
        {
            
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        public virtual bool Load()
        {
            var appName = Constants.AppName.ToLower();
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var basePath = Path.Combine(appDataFolder, appName);
            var filePath = Path.Combine(basePath, appName + FileSuffix);

            return File.Exists(filePath);
        }
    }
}
