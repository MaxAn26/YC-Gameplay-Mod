using System;

using BaseMod.Core;

using Il2Cpp;

using MelonLoader;

namespace YC.GameplayMod.Mods;
internal class GameFixMod {
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(PluginConfig config) {
        try {
            Enabled = config.Entry(nameof(GameFixMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void SexEncounerSetSexAnimation(ref SexEncounter sexEncounter) {
        if (!Enabled.Value)
            return;

        switch (sexEncounter.SexID) {
            case 1103:
                ResetAnimation(ref sexEncounter);

                sexEncounter.CasterDickRequired = false;
                sexEncounter.TargetDickRequired = false;

                sexEncounter.SexIsLickingCaster = false;
                sexEncounter.SexIsOralCaster    = false;
                sexEncounter.SexIsLickingTarget = false;
                sexEncounter.SexIsOralTarget    = false;

                Plugin.Log.Info( $"Fix position ID {sexEncounter.SexID}" );

                if (sexEncounter.TargetSex.IsMale || sexEncounter.TargetSex.IsFuta) {
                    sexEncounter.TargetAnim.CrossFade("FFMWresOralV3", 0.3f);
                    sexEncounter.CasterAnim.CrossFade("FFMWresOralA3", 0.3f);
                    sexEncounter.AssistAnim.CrossFade("MMFWresMissionX3", 0.3f);

                    sexEncounter.TargetDickRequired = true;
                    sexEncounter.SexIsOralCaster = true;
                    sexEncounter.SexIsLickingTarget = true;
                } else {
                    sexEncounter.TargetAnim.CrossFade("FFMWresOralV3", 0.3f);
                    sexEncounter.CasterAnim.CrossFade("FFFWresOralA3", 0.3f);
                    sexEncounter.AssistAnim.CrossFade("MMFWresMissionX3", 0.3f);

                    sexEncounter.SexIsLickingTarget = true;
                    sexEncounter.SexIsLickingCaster = true;
                }

                break;
            default:
                break;
        }

        static void ResetAnimation(ref SexEncounter sexEncounter) {
            if (sexEncounter.CasterAnim.IsInTransition(0)) {
                sexEncounter.CasterAnim.StopPlayback();
            }
            if (sexEncounter.TargetAnim.IsInTransition(0)) {
                sexEncounter.TargetAnim.StopPlayback();
            }
            if (sexEncounter.AssistAnim.IsInTransition(0)) {
                sexEncounter.AssistAnim.StopPlayback();
            }
        }
    }
}
