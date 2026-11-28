using System.IO;
using System.Text.Json;
using UPCD.App.Models;

namespace UPCD.App.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UniversalPCDoctor", "settings.json");

    public async Task<AppSettings> LoadAsync()
    {
        try
        {
            if (!File.Exists(_path)) return new AppSettings();

            await using var stream = File.OpenRead(_path);
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream) ?? new AppSettings();
            settings.Normalize();
            return settings;
        }
        catch
        {
            // Corrupt or inaccessible settings must never prevent the app from starting.
            return new AppSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Normalize();

        var directory = Path.GetDirectoryName(_path)!;
        Directory.CreateDirectory(directory);

        // A unique temporary file prevents overlapping saves from sharing the same .tmp file.
        var tempPath = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(
                tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, settings, JsonOptions);
                await stream.FlushAsync();
            }

            File.Move(tempPath, _path, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
