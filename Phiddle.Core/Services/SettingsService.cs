using System;
using System.IO;
using Phiddle.Core.Settings;
using Utf8Json;

namespace Phiddle.Core.Services
{
    public class SettingsService<T> : ISettingsService<T> where T : new()
    {
        private T settings;
        private readonly string fileName;
        private readonly string settingsPath;

        public T Settings
        {
            get
            {
                if (!Loaded)
                {
                    Load();
                }
    
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        public bool Loaded { get; private set; }

        public SettingsService()
        {
            try
            {
                Loaded = false;
                var appName = Constants.AppName.ToLower();
                fileName = appName + "." + typeof(T).Name.ToString().ToLower() + ".json";
                var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                settingsPath = Path.Combine(appDataFolder, appName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Save()
        {
            if (!Directory.Exists(settingsPath))
            {
                Directory.CreateDirectory(settingsPath);
            }

            var jsonBytes = JsonSerializer.Serialize(settings);
            var json = JsonSerializer.PrettyPrint(jsonBytes);
            var fullPath = Path.Combine(settingsPath, fileName);
            File.WriteAllText(fullPath, json);
        }

        public void Load()
        {
            var filePath = Path.Combine(settingsPath, fileName);
            var fileExists = File.Exists(filePath);

            if (fileExists)
            {
                var json = File.ReadAllText(filePath);
                settings = JsonSerializer.Deserialize<T>(json);
                Loaded = true;
            }
            else
            {
                // First time load, try to get defaults but indicate nothing loaded
                settings = new T();
                Save();
                Loaded = false;
            }
        }
    }
}
