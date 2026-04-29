using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YC.Unloader.Models;
public class ColorsList
{
    public List<Color> EyesColors { get; set; } = [];
    public List<Color> HairColors { get; set; } = [];
    public List<Color> SkinTones { get; set; } = [];

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
