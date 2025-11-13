using System.IO;

using Tomlet;
using Tomlet.Attributes;

namespace YC.EnemyRandomizerMod.Configs;
public class ModConfig {
    public EnemyBodyRandomizerConfig EnemyBodyRandomizer { get; set; } = new();

    public static ModConfig Load( string path ) {
        ModConfig config;
        if (File.Exists(path)) {
            string tomlText = File.ReadAllText(path);
            config = TomletMain.To<ModConfig>(tomlText);
        } else {
            config = new ModConfig();
            Save( path, config);
        }

        return config;
    }

    public static void Save(string path, ModConfig modConfig) {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        string tomlText = TomletMain.TomlStringFrom(modConfig);
        File.WriteAllText(path, tomlText);
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