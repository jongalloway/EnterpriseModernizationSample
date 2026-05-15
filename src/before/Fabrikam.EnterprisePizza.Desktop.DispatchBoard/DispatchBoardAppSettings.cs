using System;
using System.Configuration;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public static class DispatchBoardAppSettings
    {
        private const string StoreNumberKey = "DispatchBoard.StoreNumber";
        private const string TerminalIdKey = "DispatchBoard.TerminalId";
        private const string AutoRefreshSecondsKey = "DispatchBoard.AutoRefreshSeconds";
        private const string IncludeDriverNotesKey = "DispatchBoard.IncludeDriverNotes";

        public static DispatchBoardSettings Load()
        {
            var settings = new DispatchBoardSettings
            {
                StoreNumber = ReadString(StoreNumberKey, "014"),
                DispatchTerminalId = ReadString(TerminalIdKey, "TERM-02"),
                AutoRefreshSeconds = ReadInt(AutoRefreshSecondsKey, DispatchBoardSettings.DefaultAutoRefreshSeconds),
                IncludeDriverNotes = ReadBool(IncludeDriverNotesKey, true)
            };

            settings.AutoRefreshSeconds = ClampAutoRefreshSeconds(settings.AutoRefreshSeconds);
            return settings;
        }

        public static void Save(DispatchBoardSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            settings.AutoRefreshSeconds = ClampAutoRefreshSeconds(settings.AutoRefreshSeconds);

            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            SaveValue(configuration, StoreNumberKey, settings.StoreNumber);
            SaveValue(configuration, TerminalIdKey, settings.DispatchTerminalId);
            SaveValue(configuration, AutoRefreshSecondsKey, settings.AutoRefreshSeconds.ToString());
            SaveValue(configuration, IncludeDriverNotesKey, settings.IncludeDriverNotes.ToString().ToLowerInvariant());
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static int ClampAutoRefreshSeconds(int autoRefreshSeconds)
        {
            if (autoRefreshSeconds < DispatchBoardSettings.MinimumAutoRefreshSeconds)
            {
                return DispatchBoardSettings.MinimumAutoRefreshSeconds;
            }

            if (autoRefreshSeconds > DispatchBoardSettings.MaximumAutoRefreshSeconds)
            {
                return DispatchBoardSettings.MaximumAutoRefreshSeconds;
            }

            return autoRefreshSeconds;
        }

        private static string ReadString(string key, string fallback)
        {
            var configured = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(configured) ? fallback : configured.Trim();
        }

        private static int ReadInt(string key, int fallback)
        {
            int parsed;
            return int.TryParse(ConfigurationManager.AppSettings[key], out parsed) ? parsed : fallback;
        }

        private static bool ReadBool(string key, bool fallback)
        {
            bool parsed;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out parsed) ? parsed : fallback;
        }

        private static void SaveValue(Configuration configuration, string key, string value)
        {
            var settings = configuration.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
        }
    }
}
