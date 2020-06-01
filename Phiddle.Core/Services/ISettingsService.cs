namespace Phiddle.Core.Services
{
    public interface ISettingsService<T>
    {
        T Settings { get; set; }
        bool Loaded { get; }
        bool IsDefault { get; }
        void Save();
        void Load();
    }
}
