using System;

using Il2Cpp;

using YC.GameplayMod.Configs;

namespace YC.GameplayMod.Mods;
internal class DickStraponVisibilityMod {
    #region Configuration
    internal static bool Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled;
    #endregion

    internal static void Load(ModConfig config) {
        try {
            Enabled = config.DickStraponVisibility.Enabled;

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static bool SetDicks(SexEncounter sexEncounter) {
        try {
            if (!Enabled)
                return false;

            if (sexEncounter.CasterMale || sexEncounter.CasterFuta || sexEncounter.CasterDickRequired) {
                sexEncounter.CasterSex.SetDick(true);
            } else {
                sexEncounter.CasterSex.SetDick(false);
            }

            if (sexEncounter.TargetMale || sexEncounter.TargetFuta || sexEncounter.TargetDickRequired) {
                sexEncounter.TargetSex.SetDick(true);
            } else {
                sexEncounter.TargetSex.SetDick(false);
            }

            if (sexEncounter.IsThreesome) {
                if (sexEncounter.AssistMale || sexEncounter.AssistFuta || sexEncounter.AssistDickRequired) {
                    sexEncounter.AssistSex.SetDick(true);
                } else {
                    sexEncounter.AssistSex.SetDick(false);
                }
            }
            return true;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return false;
        }
    }

    internal static void SetDick( CharacterSex characterSex, bool showDick = false) {
        try {
            if (!Enabled)
                return;

            if (showDick) {
                if (characterSex.IsMale) {
                    characterSex.ChangeFacialExpression(0, 0);
                } else {
                    characterSex.Dick.gameObject.SetActive(true);
                }
            } else {
                if (characterSex.IsMale) {
                    characterSex.ChangeFacialExpression(0, 100);
                } else  {
                    characterSex.Dick.gameObject.SetActive(false);
                }
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }
}