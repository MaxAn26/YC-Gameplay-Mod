namespace YC.GameplayMod.Models;
public class BodyRestrictions {
    public ValueRestrictions<int> Areola { get; set; } = new();
    public ValueRestrictions<float> Arms { get; set; } = new();
    public ValueRestrictions<float> Belly { get; set; } = new();
    public ValueRestrictions<float> Biceps { get; set; } = new();
    public ValueRestrictions<float> Boobs { get; set; } = new();
    public ValueRestrictions<float> Booty { get; set; } = new();
    public ValueRestrictions<float> Calves { get; set; } = new();
    public ValueRestrictions<float> Dick { get; set; } = new();
    public ValueRestrictions<float> Hips { get; set; } = new();
    public ValueRestrictions<float> Muscle { get; set; } = new();
    public ValueRestrictions<float> Torso { get; set; } = new();
    public ValueRestrictions<float> Thighs { get; set; } = new();

    public class ValueRestrictions<T> {
        public T Min { get; set; }
        public T Max { get; set; }
    }
}
