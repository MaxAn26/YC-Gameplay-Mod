using System;

using BaseMod.Core;
using BaseMod.Core.Utils;

using Il2Cpp;

using MelonLoader;

using UnityEngine.SceneManagement;

namespace YC.GameplayMod.Mods;
internal class RandomReverseMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> IgnoreAtSameRoles;
    internal static MelonPreferences_Entry<bool> AlwaysAtSameRoles;
    internal static MelonPreferences_Entry<bool> AlwaysInCommandPose;
    internal static MelonPreferences_Entry<bool> AlwaysWhenCharmed;
    internal static MelonPreferences_Entry<int> Chance;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(PluginConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(RandomReverseMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            IgnoreAtSameRoles = config.Entry(nameof(RandomReverseMod), nameof(IgnoreAtSameRoles), false,
                "Do not activate when the enemy's role matches the player's role", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AlwaysAtSameRoles = config.Entry(nameof(RandomReverseMod), nameof(AlwaysAtSameRoles), false,
                "Always activate when the enemy's role matches the player's role", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AlwaysInCommandPose = config.Entry(nameof(RandomReverseMod), nameof(AlwaysInCommandPose), false,
                "Always activate when positions has tag Command", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AlwaysWhenCharmed = config.Entry(nameof(RandomReverseMod), nameof(AlwaysWhenCharmed), false,
                "Always activate when Caster OR Target is charmed. When both characters charmed - ignore", new PluginConfig.AcceptableValueList<bool>([true, false]));
            Chance = config.Entry(nameof(RandomReverseMod), nameof(Chance), 20,
                "Chance for animation reversal", new PluginConfig.AcceptableValueRange<int>(0, 100));

        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Apply(SexEncounter sexEncounter)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex < 2)
            {
                return;
            }

            if (sexEncounter.TargetSex.IsBoundHeavyRestraint > 0)
            {
                return;
            }

            if (sexEncounter.CasterSex.IsCharmed || sexEncounter.CasterAttributes.CheckForStatus("Charmed"))
            {
                return;
            }

            if (IgnoreAtSameRoles.Value && sexEncounter.CasterActive == sexEncounter.TargetActive)
            {
                return;
            }

            if (AlwaysAtSameRoles.Value && sexEncounter.CasterActive == sexEncounter.TargetActive)
            {
                ActivateReverse(sexEncounter);
                return;
            }

            if (AlwaysInCommandPose.Value)
            {
                if (sexEncounter.CurrentMove?.isCommand == true)
                {
                    ActivateReverse(sexEncounter);
                    return;
                }
                else if (SexChoiceRealismMod.LastMove?.ID == sexEncounter.SexID && SexChoiceRealismMod.LastMove?.IsCommand == true)
                {
                    ActivateReverse(sexEncounter);
                    return;
                }
            }

            if (AlwaysWhenCharmed.Value
                && (sexEncounter.TargetSex.IsCharmed && !sexEncounter.CasterSex.IsCharmed
                    || sexEncounter.CasterSex.IsCharmed && !sexEncounter.TargetSex.IsCharmed))
            {
                ActivateReverse(sexEncounter);
                return;
            }

            if (RandomUtils.Chance(Chance.Value))
            {
                ActivateReverse(sexEncounter);
                return;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    static void ActivateReverse(SexEncounter sexEncounter)
    {
        sexEncounter.ReverseMode = !sexEncounter.ReverseMode;
        Plugin.Log.Info("Reverse mod activated");
    }
}
