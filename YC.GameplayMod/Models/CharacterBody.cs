namespace YC.GameplayMod.Models;
internal struct CharacterBody
{
    internal int Areola;
    internal float Arms;
    internal float Belly;
    internal float Biceps;
    internal float Boobs;
    internal float Booty;
    internal float Calves;
    internal float Dick;
    internal float Hips;
    internal float Muscle;
    internal float Thighs;
    internal float Torso;

    public override readonly string ToString() => $"Muscle: {Muscle}, Torso: {Torso}, Hips: {Hips}, Belly: {Belly}, Arms: {Arms}, Biceps: {Biceps}, Thighs: {Thighs}, Calves: {Calves}, Boobs: {Boobs}, Booty: {Booty}, Dick: {Dick}";
}
