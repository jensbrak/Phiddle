using Phiddle.Core.Settings;

namespace Phiddle.Core.Services
{
    public interface ISettingsService
    {
        ISettings Settings { get; set; }
        string FolderPath { get; set; }
        string FileName { get; set; }
        void Save();
        bool Load();
    }
}
