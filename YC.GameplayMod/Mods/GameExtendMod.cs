using System;
using System.Collections.Generic;
using System.Linq;

using BaseMod.Core;
using BaseMod.Core.Utils;

using Il2Cpp;

using MelonLoader;

using UnityEngine;

using YC.GameplayMod.Extensions;

namespace YC.GameplayMod.Mods;
internal class GameExtendMod {
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> JoinThreesome;
    internal static MelonPreferences_Entry<bool> AssistAlly;
    internal static MelonPreferences_Entry<bool> AttackTarget;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    internal static void Load(PluginConfig config) {
        try {
            Enabled = config.Entry(nameof(GameExtendMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AssistAlly = config.Entry(nameof(GameExtendMod), nameof(AssistAlly), true,
                "Companion will help not only to player; they will help to all companion according with situation", new PluginConfig.AcceptableValueList<bool>([true, false]));
            AttackTarget = config.Entry(nameof(GameExtendMod), nameof(AttackTarget), true,
                "When character(Ally or enemy character) attack opponent they will focus on character with lower HP", new PluginConfig.AcceptableValueList<bool>([true, false]));
            JoinThreesome = config.Entry(nameof(GameExtendMod), nameof(JoinThreesome), true,
                "When character join to threesome it may change their positions: player or Elite enemies will always get a Caster role", new PluginConfig.AcceptableValueList<bool>([true, false]));


        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static bool SexEncounterJoinThreesome(SexEncounter sexEncounter, CharacterAttributes joinedCharacter, int oldSexID) {
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
                Plugin.Log.Debug("Player join to Threesome");
                SexEncounter.print("Player join to Threesome");
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
                Plugin.Log.Debug("Ally join to Threesome");
                Zessentials.Instance.battleManager.console.ConsoleWrite("Ally join to Threesome");
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
                Plugin.Log.Debug("Elite enemy join to Threesome");
                Zessentials.Instance.battleManager.console.ConsoleWrite("Elite enemy join to Threesome");
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
                Plugin.Log.Debug("Enemy join to Threesome");
                Zessentials.Instance.battleManager.console.ConsoleWrite("Enemy join to Threesome");
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

    internal static void BattleManagerAssistAlly( ref CombatAction combatAction ) {
        if (!Enabled.Value || !AssistAlly.Value)
            return;

        if (combatAction.caster.isPlayer || !combatAction.target.isPlayer)
            return;

        var battleManager = combatAction.caster.battleManager;
        var combatBuffsManager = battleManager.combatBuffsManager;
        if (battleManager is null || combatBuffsManager is null)
            return;

        CharacterAttributes player = combatAction.target;
        CharacterAttributes companion = null;
        if (battleManager.Ally1Exists && combatAction.caster != battleManager.characterAlly1)
            companion = battleManager.characterAlly1;
        else if (battleManager.Ally2Exists && combatAction.caster != battleManager.characterAlly2)
            companion = battleManager.characterAlly2;

        if (companion is null)
            return;

        int playerWeight = 0;
        int companionWeight = 0;

        playerWeight    += BoundsCount(player);
        companionWeight += BoundsCount(companion);

        playerWeight    += player.isGrappled ? 1 : 0;
        companionWeight += companion.isGrappled ? 1 : 0;

        playerWeight    -= combatBuffsManager.GetNumberOfStacksOnCharacteR(player, "Resolute cheer");
        companionWeight -= combatBuffsManager.GetNumberOfStacksOnCharacteR(companion, "Resolute cheer");

        playerWeight    -= combatBuffsManager.GetNumberOfStacksOnCharacteR(player, "Battle cheer");
        companionWeight -= combatBuffsManager.GetNumberOfStacksOnCharacteR(companion, "Battle cheer");

        playerWeight    = Math.Max(playerWeight, 0);
        companionWeight = Math.Max(companionWeight, 0);

        playerWeight    += (int)((1f - (float)player.currentHealth / player.maxHealth) * 10);
        companionWeight += (int)((1f - (float)companion.currentHealth / companion.maxHealth) * 10);

        if (player.currentPleasure > 0)
            playerWeight += Math.Max((int)((player.currentPleasure / 10000f) * 10), 1);

        if (companion.currentPleasure > 0)
            companionWeight += Math.Max((int)((companion.currentPleasure / 10000f) * 10), 1);

        Plugin.Log.Debug( $"BattleManagerAssistAlly weights: Player: {playerWeight}, Companion: {companionWeight}" );

        if (companionWeight >= playerWeight) {
            combatAction.target = companion;
            Plugin.Log.Debug($"'{combatAction.caster.characterSex.characterName}': Assist to '{combatAction.target.characterSex.characterName}'");
            Zessentials.Instance.battleManager.console.ConsoleWrite($"{combatAction.caster.characterSex.characterName}: will assist to {combatAction.target.characterSex.characterName}");
        }

        static int BoundsCount(CharacterAttributes characterAttributes) {
            if (characterAttributes.characterSex.restraintsWearing == 0)
                return 0;

            int res = 0;
            if (characterAttributes.characterSex.IsBoundHeavyRestraint > 0)
                res += 1;

            if (characterAttributes.characterSex.IsBoundHandRestraint > 0)
                res += 1;

            if (characterAttributes.characterSex.IsBoundLegRestraint > 0)
                res += 1;

            if (characterAttributes.characterSex.IsBoundBlindfold > 0)
                res += 1;

            if (characterAttributes.characterSex.IsBoundGag > 0)
                res += 1;

            return res;
        }
    }

    internal static void BattleManagerExecuteAction( ref CombatAction combatAction ) {
        if (!Enabled.Value || !AttackTarget.Value)
            return;

        if (combatAction.actionType is not 1 and not 2)
            return;

        if (combatAction.actionTarget is not 1 and not 3 and not 4)
            return;

        if (combatAction.caster.isPlayer)
            return;

        if (combatAction.caster.combatAI is null || combatAction.target?.combatAI is null)
            return;

        List<CharacterAttributes> enemies = [];
        CharacterAttributes target = null;
        if (combatAction.caster.IsCompanion(false) && !combatAction.target.IsCompanion()) {
            target = CompanionAttackTarget( combatAction, [.. Zessentials.Instance.GetAllFreeEnemyTargets()]);
            SetNewTarget(ref combatAction, target);
        } else if (combatAction.caster.IsEnemy() && combatAction.target.IsCompanion()) {
            target = EnemyAttackTarget( combatAction, [.. Zessentials.Instance.GetAllFreePlayerTargets()]);
            
        }

        if (target is null) {
            combatAction.ToDefence();
            return;
        }

        SetNewTarget(ref combatAction, target);

        static CharacterAttributes CompanionAttackTarget( CombatAction action, List<CharacterAttributes> freeTargets) {
            freeTargets = [.. freeTargets.Where(CheckCharacter)];

            if (freeTargets.Count <= 0) {
                return null;
            }

            CharacterAttributes newTarget = freeTargets.OrderBy(GetTargetScore).First();

            return newTarget;

            static float GetTargetScore(CharacterAttributes characterAttributes) {
                float score = 0f;

                score += ((float)characterAttributes.currentHealth / characterAttributes.maxHealth);

                score += RandomUtils.Float(0f, 0.2f);

                Plugin.Log.Debug($"{characterAttributes.characterName}: score: {score}");
                return score;
            }
        }

        static CharacterAttributes EnemyAttackTarget(CombatAction action, List<CharacterAttributes> freeTargets) {
            freeTargets = [.. freeTargets.Where(CheckCharacter)];

            if (freeTargets.Count <= 0) {
                return null;
            }

            float casterDamage = 0f;
            casterDamage += action.caster.equippedWeapon.weaponMinDamage;
            if (action.isAttack)
                casterDamage += action.actionEffect1Value * (action.caster.attackPower * action.actionEffect1Scaling / 100f);

            if (action.isSpell)
                casterDamage += action.actionEffect1Value * (action.caster.spellPower * action.actionEffect1Scaling / 100f);

            int damageBonuses = 100;
            if (action.isBasicAction) { damageBonuses += action.caster.basicActionBonus; }
            if (action.isAbility) { damageBonuses += action.caster.abilityPower; }
            if (action.isWeaponAttack) { damageBonuses += action.caster.weaponDamage; }
            if (action.isOneHanded) { damageBonuses += action.caster.onehandedDamage; }
            if (action.isTwoHanded) { damageBonuses += action.caster.twohandedDamage; }
            if (action.isUnarmed || action.caster.equippedWeapon.weaponType == 0) { damageBonuses += action.caster.unarmedDamage; }
            if (action.isKick) { damageBonuses += action.caster.kickDamage; }
            if (action.isSpell) { damageBonuses += action.caster.spellDamage; }
            if (action.isGrapple) { damageBonuses += action.caster.grappleDamage; }
            if (action.isHex) { damageBonuses += action.caster.hexDamage; }
            if (action.isRubyflame) { damageBonuses += action.caster.rubyflameDamage; }
            if (action.isNature) { damageBonuses += action.caster.natureDamage; }
            if (action.isArcane) { damageBonuses += action.caster.arcaneDamage; }
            if (action.elementPhysical) { damageBonuses += action.caster.physicalDamage; }
            if (action.elementFire) { damageBonuses += action.caster.fireDamage; }
            if (action.elementLightning) { damageBonuses += action.caster.lightningDamage; }
            if (action.elementCorrosive) { damageBonuses += action.caster.corrosiveDamage; }
            if (action.elementShadow) { damageBonuses += action.caster.shadowDamage; }

            casterDamage *= damageBonuses / 100;
            casterDamage *= action.caster.damageDone / 100;

            CharacterAttributes newTarget = freeTargets.OrderBy(GetTargetScore).First();

            return newTarget;

            float GetTargetScore(CharacterAttributes character) {
                float defenseReduction = Mathf.Min(character.defense * 0.15f, 75f);
                float defenseFactor = 1f - (defenseReduction / 100f);
                float resistFactor = 1f;

                if (action.elementPhysical)
                    resistFactor *= GetResistance(character.physicalResistance);
                if (action.elementFire)
                    resistFactor *= GetResistance(character.fireResistance);
                if (action.elementLightning)
                    resistFactor *= GetResistance(character.lightningResistance);
                if (action.elementCorrosive)
                    resistFactor *= GetResistance(character.corrosiveResistance);
                if (action.elementShadow)
                    resistFactor *= GetResistance(character.shadowResistance);

                float damageMultiplier = defenseFactor * resistFactor * (character.damageTaken / 100f);

                float score = 0f;
                score += ((float)character.currentHealth / character.maxHealth) * 0.5f;
                score += (character.currentHealth / damageMultiplier) * 0.001f;
                score += RandomUtils.Float(0f, 0.2f);

                Plugin.Log.Debug($"{character.characterName}: score: {score}");
                return score;

                float GetResistance( int resistValue ) {
                    return resistValue / 100f;
                }
            }
        }

        static bool CheckCharacter(CharacterAttributes characterAttributes) {
            if (characterAttributes.currentHealth < 1)
                return false;
               
            if (characterAttributes.characterSex.IsGrappled) 
                return false;

            if (characterAttributes.CheckForStatus("Defenseless") || characterAttributes.CheckForStatus("Weakened"))
                return false;

            if (characterAttributes.characterSex.IsBoundHeavyRestraint > 0)
                return false;
            
            return true;
        }

        static void SetNewTarget( ref CombatAction action, CharacterAttributes newTarget) {
            if (newTarget != action.target) {
                Plugin.Log.Debug($"Change CombatAction target from {action.target.characterName} to {newTarget.characterName}");
                action.target = newTarget;
                Zessentials.Instance.battleManager.console.ConsoleWrite($"{action.caster.characterName}: will attack {action.target.characterName}");
            }
        }
    }
}
