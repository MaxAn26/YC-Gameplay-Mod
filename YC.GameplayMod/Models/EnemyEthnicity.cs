using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YC.GameplayMod.Models;
public class EnemyEthnicity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<int> RandomEthnicity { get; set; } = [];

    [JsonIgnore]
    public bool IsRandomEthnicity => RandomEthnicity.Count > 0;

    public Dictionary<string, int> BodyWeights { get; set; } = [];

    [JsonIgnore]
    public Dictionary<BodyProfile, int> BodyProfileWeights { get; set; } = [];

    public List<Color> EyesColors { get; set; } = [];
    public List<Color> HairColors { get; set; } = [];
    public List<Color> SkinTones { get; set; } = [];
    public List<Color> MakeUpColors { get; set; } = [];

    public List<int> FaceStyle { get; set; } = [];
    public List<int> EyesStyle { get; set; } = [];
    public List<int> NoseStyle { get; set; } = [];
    public List<int> BrowStyle { get; set; } = [];
    public List<int> MouthStyle { get; set; } = [];
    public List<int> MouthLength { get; set; } = [];
    public List<int> LipsForward { get; set; } = [];
    public List<int> LipsSize { get; set; } = [];
    public List<int> EarsStyle { get; set; } = [];

    public class Color
    {
        [JsonPropertyName("r")]
        public float R { get; set; }

        [JsonPropertyName("g")]
        public float G { get; set; }

        [JsonPropertyName("b")]
        public float B { get; set; }

        [JsonPropertyName("a")]
        public float A { get; set; }

        public static Color FromUnityColor(UnityEngine.Color color) => new Color { R = color.r, G = color.g, B = color.b, A = color.a };

        public UnityEngine.Color ToUnityColor() => new UnityEngine.Color { r = R, g = G, b = B, a = A };
    }
}
