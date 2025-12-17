using System;

using BaseMod.Core;

using Il2Cpp;

using MelonLoader;

using UnityEngine;

namespace YC.GameplayMod.Mods;
internal class GameFixMod {
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> JoinThreesome;
    internal static MelonPreferences_Entry<bool> AssistAlly;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(PluginConfig config) {
        try {
            Enabled = config.Entry(nameof(GameFixMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AssistAlly = config.Entry(nameof(GameFixMod), nameof(AssistAlly), false,
                "Extend Assist Ally", new PluginConfig.AcceptableValueList<bool>([true, false]));
            JoinThreesome = config.Entry(nameof(GameFixMod), nameof(JoinThreesome), false,
                "Extend Join threesome", new PluginConfig.AcceptableValueList<bool>([true, false]));

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static bool JoinThreesomeFix(SexEncounter sexEncounter, CharacterAttributes joinedCharacter, int oldSexID) {
        try {
            if (!Enabled.Value || !JoinThreesome.Value)
                return false;

            if (joinedCharacter == null) { return false; }

            CharacterSex casterSex          = sexEncounter.CasterSex;
            CharacterSex currentCasterSex   = sexEncounter.CasterSex;
            CharacterSex targetSex          = sexEncounter.TargetSex;
            CharacterSex currentTargetSex   = sexEncounter.TargetSex;
            CharacterSex assistSex          = joinedCharacter.characterSex;
            CharacterSex currentAssistSex   = joinedCharacter.characterSex;

            if (assistSex.IsPlayer) {
                Plugin.Log.Debug($"Player join to Threesome");
                SexEncounter.print($"Player join to Threesome");
                if (casterSex.characterAttributes.combatAI.isAlly && !casterSex.characterAttributes.CheckForStatus("Charmed")) {
                    casterSex = currentAssistSex;
                    targetSex = currentTargetSex;
                    assistSex = currentCasterSex;
                } else {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                }

                if (CharacterDataa.Instance.currentState == CharacterDataa.BattleState.Fucking) {
                    sexEncounter.battleManager.combatUIManager.EnableSexfightUI();
                } else {
                    sexEncounter.battleManager.combatUIManager.DisableSexfightUI();
                }
            } else if (assistSex.characterAttributes.combatAI.isAlly) {
                Plugin.Log.Debug($"Ally join to Threesome");
                SexEncounter.print($"Ally join to Threesome");
                if (targetSex.IsPlayer) {
                    casterSex = currentTargetSex;
                    targetSex = currentCasterSex;
                    assistSex = currentAssistSex;
                } else if (targetSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                } else {
                    casterSex = currentCasterSex;
                    targetSex = currentTargetSex;
                    assistSex = currentAssistSex;
                }
            } else if (assistSex.characterAttributes.combatAI.isElite) {
                Plugin.Log.Debug($"Elite enemy join to Threesome");
                SexEncounter.print($"Elite enemy join to Threesome");
                if (casterSex.IsPlayer || casterSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                } else {
                    casterSex = currentAssistSex;
                    targetSex = currentTargetSex;
                    assistSex = currentCasterSex;
                }
            } else {
                Plugin.Log.Debug($"Enemy join to Threesome");
                SexEncounter.print($"Enemy join to Threesome");
                if (casterSex.IsPlayer || casterSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentTargetSex;
                    targetSex = currentCasterSex;
                    assistSex = currentAssistSex;
                } else {
                    casterSex = currentCasterSex;
                    targetSex = currentTargetSex;
                    assistSex = currentAssistSex;
                }
            }

            assistSex.currentSexEncounter = sexEncounter;
            casterSex.currentSexEncounter = sexEncounter;
            targetSex.currentSexEncounter = sexEncounter;

            sexEncounter.Assist = assistSex.gameObject;
            sexEncounter.AssistSex = assistSex;
            sexEncounter.AssistAnim = assistSex.GetComponent<Animator>();
            sexEncounter.AssistAttributes = assistSex.characterAttributes;
            sexEncounter.AssistMale = assistSex.IsMale;
            sexEncounter.AssistActive = assistSex.IsActive;
            sexEncounter.AssistFuta = assistSex.IsFuta;

            sexEncounter.Caster = casterSex.gameObject;
            sexEncounter.CasterSex = casterSex;
            sexEncounter.CasterAnim = casterSex.GetComponent<Animator>();
            sexEncounter.CasterAttributes = casterSex.characterAttributes;
            sexEncounter.CasterMale = casterSex.IsMale;
            sexEncounter.CasterActive = casterSex.IsActive;
            sexEncounter.CasterFuta = casterSex.IsFuta;

            sexEncounter.Target = targetSex.gameObject;
            sexEncounter.TargetSex = targetSex;
            sexEncounter.TargetAnim = targetSex.GetComponent<Animator>();
            sexEncounter.TargetAttributes = targetSex.characterAttributes;
            sexEncounter.TargetMale = targetSex.IsMale;
            sexEncounter.TargetActive = targetSex.IsActive;
            sexEncounter.TargetFuta = targetSex.IsFuta;

            sexEncounter.IsThreesome = true;
            sexEncounter.ThreesomeIsCasterFriend = true;
            sexEncounter.CasterIsAttacker = true;

            if (SexChoiceRealismMod.IsModActive) {
                int newSexId = SexChoiceRealismMod.GetSexId(sexEncounter.CasterSex, targetSex, assistSex);
                if (newSexId > 0)
                    oldSexID = newSexId;
            }

            sexEncounter.SexID = oldSexID;
            sexEncounter.StartThreesome();

            return true;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return false;
        }
    }

    public static void AssistAllyFix(CombatAction combatAction) {
        if (!Enabled.Value || !AssistAlly.Value)
            return;

        if (combatAction.caster.isPlayer || !combatAction.target.isPlayer)
            return;

        var battleManager = combatAction.caster.battleManager;
        var combatBuffsManager = battleManager.combatBuffsManager;
        if (battleManager is null || combatBuffsManager is null)
            return;

        CharacterAttributes player = combatAction.target;
        CharacterAttributes otherAlly = null;
        if (battleManager.Ally1Exists && combatAction.caster != battleManager.characterAlly1)
            otherAlly = battleManager.characterAlly1;
        else if (battleManager.Ally2Exists && combatAction.caster != battleManager.characterAlly2)
            otherAlly = battleManager.characterAlly1;

        if (otherAlly is null)
            return;

        if (!player.isGrappled && !otherAlly.isGrappled) {
            if (IsBound( player ))
                combatAction.target = player;
            else if (IsBound(otherAlly))
                combatAction.target = otherAlly;
            else if (player.currentHealth / player.maxHealth > otherAlly.currentHealth / otherAlly.maxHealth)
                combatAction.target = otherAlly;
            else if (combatBuffsManager.GetNumberOfStacksOnCharacteR( player, "Resolute cheer") > combatBuffsManager.GetNumberOfStacksOnCharacteR(otherAlly, "Resolute cheer"))
                combatAction.target = otherAlly;
        }
        else if (player.isGrappled && !otherAlly.isGrappled) {
            if (combatBuffsManager.GetNumberOfStacksOnCharacteR(player, "Battle cheer") >= 4)
                combatAction.target = otherAlly;
        } else if (!player.isGrappled && otherAlly.isGrappled) {
            if (player.currentHealth > player.maxHealth * 0.6 && combatBuffsManager.GetNumberOfStacksOnCharacteR(otherAlly, "Battle cheer") < 4)
                combatAction.target = otherAlly;
        }else {
            if (combatBuffsManager.GetNumberOfStacksOnCharacteR(otherAlly, "Battle cheer") < 4)
                combatAction.target = otherAlly;
        }

        static bool IsBound(CharacterAttributes characterAttributes) {
            if (characterAttributes.characterSex.restraintsWearing == 0)
                return false;

            if (characterAttributes.characterSex.IsBoundHeavyRestraint > 0)
                return true;

            if (characterAttributes.characterSex.IsBoundHandRestraint > 0)
                return true;

            if (characterAttributes.characterSex.IsBoundLegRestraint > 0)
                return true;

            if (characterAttributes.characterSex.IsBoundBlindfold > 0)
                return true;

            if (characterAttributes.characterSex.IsBoundGag > 0)
                return true;

            return false;
        }
    }
}