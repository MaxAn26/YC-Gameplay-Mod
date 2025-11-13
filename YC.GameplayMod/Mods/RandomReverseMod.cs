using System;

using BaseMod.Core.Utils;

using Il2Cpp;

using UnityEngine.SceneManagement;

using YC.GameplayMod.Configs;

namespace YC.GameplayMod.Mods;
internal class RandomReverseMod {
    #region Configuration
    internal static bool Enabled;
    internal static bool IgnoreAtSameRoles;
    internal static bool AlwaysAtSameRoles;
    internal static bool AlwaysInCommandPose;
    internal static bool AlwaysWhenTargetCharmed;
    internal static int  Chance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled;
    #endregion

    internal static void Load(ModConfig config) {
        try {
            Enabled = config.RandomReverse.Enabled;
            IgnoreAtSameRoles = config.RandomReverse.Enabled;
            AlwaysAtSameRoles = !config.RandomReverse.AlwaysAtSameRoles;
            AlwaysInCommandPose = config.RandomReverse.AlwaysInCommandPose;
            AlwaysWhenTargetCharmed = config.RandomReverse.AlwaysWhenTargetCharmed;
            Chance = config.RandomReverse.Chance;

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Apply(SexEncounter sexEncounter) {
        try {
            if (!Enabled || SceneManager.GetActiveScene().buildIndex < 2)
                return;

            if (sexEncounter.TargetSex.IsBoundHeavyRestraint > 0)
                return;

            if (IgnoreAtSameRoles && sexEncounter.CasterActive == sexEncounter.TargetActive)
                return;

            if (AlwaysAtSameRoles && sexEncounter.CasterActive == sexEncounter.TargetActive) {
                ActivateReverse(sexEncounter);
                return;
            }

            if (AlwaysWhenTargetCharmed && sexEncounter.TargetSex.IsCharmed) {
                ActivateReverse(sexEncounter);
                return;
            }

            if (AlwaysInCommandPose) {
                if (sexEncounter.CurrentMove?.isCommand == true) {
                    ActivateReverse(sexEncounter);
                    return;
                } else if (SexChoiceRealismMod.LastMove?.ID == sexEncounter.SexID && SexChoiceRealismMod.LastMove?.IsCommand == true) {
                    ActivateReverse(sexEncounter);
                    return;
                }
            }

            if (RandomUtils.Chance(Chance )) {
                ActivateReverse(sexEncounter);
                return;
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    static void ActivateReverse(SexEncounter sexEncounter) {
        sexEncounter.ReverseMode = !sexEncounter.ReverseMode;
        Plugin.Log.Info("Reverse mod activated");
    }
}