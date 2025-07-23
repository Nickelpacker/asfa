using System;
using System.IO;
using System.Xml.Serialization;
using VRage.FileSystem;
using VRage.Utils;

namespace ClientPlugin.Settings
{
    public static class ConfigStorage
    {
        private static readonly string ConfigFileName = string.Concat(Plugin.Name, ".cfg");
        private static string ConfigFilePath => Path.Combine(MyFileSystem.UserDataPath, "Storage", ConfigFileName);

        public static void Save(GpsClipboardConfig config)
        {
            var path = ConfigFilePath;
            using (var text = File.CreateText(path))
                new XmlSerializer(typeof(GpsClipboardConfig)).Serialize(text, config);
        }

        public static GpsClipboardConfig Load()
        {
            var path = ConfigFilePath;
            if (!File.Exists(path))
            {
                return GpsClipboardConfig.Default;
            }

            var xmlSerializer = new XmlSerializer(typeof(GpsClipboardConfig));
            try
            {
                MyLog.Default.WriteLineAndConsole($"{Plugin.TG} Config file loaded");
                using (var streamReader = File.OpenText(path))
                    return (GpsClipboardConfig)xmlSerializer.Deserialize(streamReader) ?? GpsClipboardConfig.Default;
            }
            catch (Exception)
            {
                MyLog.Default.Warning($"{ConfigFileName}: Failed to read config file: {ConfigFilePath}");
            }
            
            return GpsClipboardConfig.Default;
        }
        
    }
}