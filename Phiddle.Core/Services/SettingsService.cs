using System;
using System.IO;
using Phiddle.Core.Settings;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Phiddle.Core.Services
{
    public class SettingsService<T> : ISettingsService<T> where T : new()
    {
        private T settings;
        private readonly string settingsName;
        private readonly string settingsFile;
        private readonly string settingsPath;
        private readonly ILogService log;

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
        public bool IsDefault { get; private set; }

        public SettingsService(ILogService log)
        {
            this.log = log;

            try
            {
                Loaded = false;
                IsDefault = false;
                var appName = Constants.AppName.ToLower();
                settingsName = typeof(T).Name.ToString().ToLower();
                settingsFile = appName + "." + settingsName + ".json";
                var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                settingsPath = Path.Combine(appDataFolder, appName);
            }
            catch (Exception ex)
            {
                log.Error("SettingsService", $"{settingsName}: failed to setup service", ex);
            }
        }

        public void Save()
        {
            if (!Directory.Exists(settingsPath))
            {
                Directory.CreateDirectory(settingsPath);
                log.Debug("SettingsService.Save", $"{settingsName}: settings directory created: {settingsPath}");
            }

            var jsonBytes = JsonSerializer.Serialize(settings);
            var json = JsonSerializer.PrettyPrint(jsonBytes);
            var filePath = Path.Combine(settingsPath, settingsFile);
            File.WriteAllText(filePath, json);
            log.Debug("SettingsService.Save", $"{settingsName}: settings file saved: {filePath}");
        }

        public void Load()
        {
            var filePath = Path.Combine(settingsPath, settingsFile);

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                settings = JsonSerializer.Deserialize<T>(json);
                Loaded = true;
                log.Debug("SettingsService.Load", $"Settings file loaded: {filePath}");

            }
            else
            {
                // First time load, use defaults
                settings = new T();
                Save();
                Loaded = true;
                IsDefault = true;
                log.Debug("SettingsService.Load", $"{settingsName}: default settings used");
            }
        }        
    }
}
