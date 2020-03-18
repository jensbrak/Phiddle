using System;
namespace Phiddle.Core.Settings
{
    public abstract class SettingsBase : ISettings
    {
        public string FilePath { get; private set; }

        public SettingsBase(string filePath)
        {
            FilePath = filePath;
        }

        

        public abstract void Save();
    }
}
