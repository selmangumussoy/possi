using System.Text.Json;
using WinFormsApp1.Models;

namespace WinFormsApp1.Service
{
    public static class ConfigService
    {
        private static readonly string DbConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static void SaveDbSettings(DbSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DbConfigFile, json);
        }

        public static DbSettings LoadDbSettings()
        {
            if (!File.Exists(DbConfigFile))
                return new DbSettings();

            var json = File.ReadAllText(DbConfigFile);
            return JsonSerializer.Deserialize<DbSettings>(json) ?? new DbSettings();
        }
    }
}