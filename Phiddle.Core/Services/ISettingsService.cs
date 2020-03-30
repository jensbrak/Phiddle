namespace Phiddle.Core.Services
{
    public interface ISettingsService<T>
    {
        T Settings { get; set; }
        bool Loaded { get; }
        void Save();
        void Load();
    }
}
