using System;

using BaseMod.Core;

using Il2Cpp;

using MelonLoader;

namespace YC.GameplayMod.Mods;
internal class DickStraponVisibilityMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(PluginConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(DickStraponVisibilityMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));

        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static bool SetDicks(SexEncounter sexEncounter)
    {
        try
        {
            if (!Enabled.Value)
            {
                return false;
            }

            if (sexEncounter.CasterMale || sexEncounter.CasterFuta || sexEncounter.CasterDickRequired)
            {
                sexEncounter.CasterSex.SetDick(true);
            }
            else
            {
                sexEncounter.CasterSex.SetDick(false);
            }

            if (sexEncounter.TargetMale || sexEncounter.TargetFuta || sexEncounter.TargetDickRequired)
            {
                sexEncounter.TargetSex.SetDick(true);
            }
            else
            {
                sexEncounter.TargetSex.SetDick(false);
            }

            if (sexEncounter.IsThreesome)
            {
                if (sexEncounter.AssistMale || sexEncounter.AssistFuta || sexEncounter.AssistDickRequired)
                {
                    sexEncounter.AssistSex.SetDick(true);
                }
                else
                {
                    sexEncounter.AssistSex.SetDick(false);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
            return false;
        }
    }

    internal static void SetDick(CharacterSex characterSex, bool showDick = false)
    {
        try
        {
            if (!Enabled.Value)
            {
                return;
            }

            if (showDick)
            {
                if (characterSex.IsMale)
                {
                    characterSex.ChangeFacialExpression(0, 0);
                }
                else
                {
                    characterSex.Dick.gameObject.SetActive(true);
                }
            }
            else
            {
                if (characterSex.IsMale)
                {
                    characterSex.ChangeFacialExpression(0, 100);
                }
                else
                {
                    characterSex.Dick.gameObject.SetActive(false);
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }
}
