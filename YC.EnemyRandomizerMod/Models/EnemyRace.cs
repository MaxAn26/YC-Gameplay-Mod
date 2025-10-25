using System.Collections.Generic;

namespace YC.EnemyRandomizerMod.Models;
public class EnemyRace {
    public string Name { get; set; }

    public List<string> SkinColors { get; set; } = [];
    public List<string> HairColors { get; set; } = [];
    public List<string> HairFantasyColors { get; set; } = [];
    public List<string> EyesColors { get; set; } = [];
    public List<string> EyesFantasyColors { get; set; } = [];
}