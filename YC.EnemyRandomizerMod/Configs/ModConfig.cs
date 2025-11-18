using System.IO;

using Tomlet;
using Tomlet.Attributes;

namespace YC.EnemyRandomizerMod.Configs;
public class ModConfig {
    public EnemyBodyRandomizerConfig EnemyBodyRandomizer { get; set; } = new();

    internal static ModConfig Load() {
        ModConfig config;
        string path = Path.Combine( Plugin.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");
        if (File.Exists(path)) {
            string tomlText = File.ReadAllText(path);
            config = TomletMain.To<ModConfig>(tomlText);
        } else {
            config = new ModConfig();
            config.Save();
        }

        return config;
    }

    internal void Save() {
        string path = Path.Combine( Plugin.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        string tomlText = TomletMain.TomlStringFrom(this);
        File.WriteAllText(path, tomlText);
        Plugin.Log.Info($"Save '{MyPluginInfo.PLUGIN_GUID}' configuration to: '{path}'" );
    }

    public class EnemyBodyRandomizerConfig {
        [TomlPrecedingComment("Activates the modification")]
        public bool Enabled { get; set; }

        [TomlPrecedingComment("Randomize companions")]
        public bool RandomizeCompanions { get; set; }

        [TomlPrecedingComment("Chance for female character with active or mixed role become futanari")]
        public int ChanceForFuta { get; set; } = 35;

        [TomlPrecedingComment("Chance for female futa character get full futa (dick + balls)")]
        public int ChanceForFullFuta { get; set; } = 50;
    }
}