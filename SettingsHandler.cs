using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace powerLabel
{
    public class SettingsHandler
    {

        public class Settings
        {
            public List<string> employees { get; set; }
            public string printerHost { get; set; }
            public string printerShareName { get; set; }

            public string databaseIP { get; set; }
            public string databaseName { get; set; }
            public string databaseUser { get; set; }
            public string databasePass { get; set; }

            public Settings()
            {
                employees = new List<string>();
            }

        }

        public static void WriteSettings(Settings settings)
        {
            string fileName = "settings.json";

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(settings, options);

            File.WriteAllText(fileName, jsonString);
        }

        public static Settings ReadSettings()
        {
            string fileName = "settings.json";

            string jsonString = File.ReadAllText(fileName);
            try
            {
                Settings output = JsonSerializer.Deserialize<Settings>(jsonString);
                return output;
            }
            catch (Exception)
            {
                WriteSettings(new Settings());
                return (new Settings());
            }
        }
    }

}
    
