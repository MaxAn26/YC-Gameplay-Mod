using BaseMod.Core.Utils;

namespace YC.GameplayMod.Models;
public class BodyProfile
{
    public int Id { get; set; }
    public string Name { get; set; }

    public BodySizeInt Areola { get; set; } = new();
    public BodySizeFloat Arms { get; set; } = new();
    public BodySizeFloat Belly { get; set; } = new();
    public BodySizeFloat Biceps { get; set; } = new();
    public BodySizeFloat Boobs { get; set; } = new();
    public BodySizeFloat Booty { get; set; } = new();
    public BodySizeFloat Calves { get; set; } = new();
    public BodySizeFloat Dick { get; set; } = new();
    public BodySizeFloat Hips { get; set; } = new();
    public BodySizeFloat Muscle { get; set; } = new();
    public BodySizeFloat Torso { get; set; } = new();
    public BodySizeFloat Thighs { get; set; } = new();

    public class BodySizeInt
    {
        public int Base { get; set; } = 1;
        public int Variation { get; set; } = 1;

        internal int GetSize(int extraVariation = 0) => Base + RandomUtils.Int32(-Variation + extraVariation, Variation + extraVariation);
    }

    public class BodySizeFloat
    {
        public float Base { get; set; } = 1.0f;
        public float Variation { get; set; } = 0.15f;

        internal float GetSize(float extraVariation = 0f) => Base + RandomUtils.Float(-Variation + extraVariation, Variation + extraVariation);
    }
}
