using System.IO;

using Tomlet;
using Tomlet.Attributes;

namespace YC.GameplayMod.Configs;
internal class ModConfig {
    internal DickStraponVisibilityConfig DickStraponVisibility { get; set; } = new();
    internal RandomReverseConfig RandomReverse { get; set; } = new();
    internal SexChoiceRealismConfig SexChoiceRealism { get; set; } = new();

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
        Plugin.Log.Info($"Save '{MyPluginInfo.PLUGIN_GUID}' configuration to: '{path}'");
    }

    internal class DickStraponVisibilityConfig {
        [TomlPrecedingComment("Activates the modification")]
        internal bool Enabled { get; set; }
    }

    internal class RandomReverseConfig {
        [TomlPrecedingComment("Activates the modification")]
        internal bool Enabled { get; set; }

        [TomlPrecedingComment("Do not activate when the enemy's role matches the player's role")]
        internal bool IgnoreAtSameRoles { get; set; }

        [TomlPrecedingComment("Always activate when the enemy's role matches the player's role")]
        internal bool AlwaysAtSameRoles { get; set; }

        [TomlPrecedingComment("Always activate when positions has tag Command")]
        internal bool AlwaysInCommandPose { get; set; }

        [TomlPrecedingComment("Always activate when Target is charmed")]
        internal bool AlwaysWhenTargetCharmed { get; set; }

        [TomlPrecedingComment("Chance for animation reversal")]
        internal int Chance { get; set; }
    }

    internal class SexChoiceRealismConfig {
        [TomlPrecedingComment("Activates the modification")]
        internal bool Enabled { get; set; }

        [TomlPrecedingComment("Update SexMove.json")]
        internal bool UpdateMoves { get; set; }

        [TomlPrecedingComment("ONLY use preferred positions")]
        internal bool UsePlayerPreferredPositions { get; set; }
    }
}