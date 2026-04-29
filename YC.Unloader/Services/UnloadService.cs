using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime;

using UnityEngine;

using YC.Unloader.Models;

namespace YC.Unloader.Services;
internal static class UnloadService
{
    internal static void UnloadClothes()
    {
        try
        {
            List<ClothesItem> clothingList = [];
            Plugin.Log.Info("Try find clothes in Resources");
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> clothesObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(Clothing)));
            foreach (UnityEngine.Object clothingObj in clothesObj)
            {
                Clothing clothing = clothingObj.TryCast<Clothing>();
                if (clothing is not null)
                {
                    var item = ClothesItem.FromClothes(clothing);
                    clothingList.Add(item);
                }
            }

            if (clothingList.Count <= 0)
            {
                return;
            }

            clothingList.Sort();
            Plugin.Log.Info($"Prepared {clothingList.Count}/{clothesObj.Count} clothes items");
            if (JsonUtils.TrySerialize(Plugin.PluginResources, "Clothing.json", clothingList, false))
            {
                Plugin.Log.Info($"Clothing.json was created in {Plugin.PluginResources}");
                File.WriteAllText($"{Plugin.PluginResources}/Female_ClothingIds.txt", string.Join(", ", clothingList.Where(c => !c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
                File.WriteAllText($"{Plugin.PluginResources}/Male_ClothingIds.txt", string.Join(", ", clothingList.Where(c => c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
            }
            else
            {
                Plugin.Log.Info("Clothing.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatBuffs()
    {
        try
        {
            List<CombatBuffItem> buffList = [];
            Plugin.Log.Info("Try find CombatBuff in Resources");
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> buffsObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(CombatBuff)));
            foreach (UnityEngine.Object buffObj in buffsObj)
            {
                CombatBuff buff = buffObj.TryCast<CombatBuff>();
                if (buff is not null)
                {
                    var item = CombatBuffItem.FromCombatBuff(buff);
                    buffList.Add(item);
                }
            }

            if (buffList.Count <= 0)
            {
                return;
            }

            Plugin.Log.Info($"Prepared {buffList.Count}/{buffsObj.Count} CombatBuff items");
            if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatBuffs.json", buffList, false))
            {
                Plugin.Log.Info($"CombatBuffs.json was created in {Plugin.PluginResources}");
            }
            else
            {
                Plugin.Log.Info("CombatBuffs.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatTalents()
    {
        try
        {
            List<CombatTalentItem> talentsList = [];
            Plugin.Log.Info("Try find CombatTalents in Resources");
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> talentsObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(CombatTalent)));
            foreach (UnityEngine.Object talentObj in talentsObj)
            {
                CombatTalent talent = talentObj.TryCast<CombatTalent>();
                if (talent is not null)
                {
                    var item = CombatTalentItem.FromCombatTalent(talent);
                    talentsList.Add(item);
                }
            }

            if (talentsList.Count <= 0)
            {
                return;
            }

            Plugin.Log.Info($"Prepared {talentsList.Count}/{talentsObj.Count} CombatTalents items");
            if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatTalents.json", talentsList, false))
            {
                Plugin.Log.Info($"CombatTalents.json was created in {Plugin.PluginResources}");
            }
            else
            {
                Plugin.Log.Info("CombatTalents.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatActions()
    {
        try
        {
            List<CombatActionItem> actionsList = [];
            Plugin.Log.Info("Try find CombatAction in Resources");
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> actionsObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(CombatAction)));
            foreach (UnityEngine.Object actionObj in actionsObj)
            {
                CombatAction action = actionObj.TryCast<CombatAction>();
                if (action is not null)
                {
                    var item = CombatActionItem.FromCombatTalent(action);
                    actionsList.Add(item);
                }
            }

            if (actionsList.Count <= 0)
            {
                return;
            }

            Plugin.Log.Info($"Prepared {actionsList.Count}/{actionsObj.Count} CombatAction items");
            if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatActions.json", actionsList, false))
            {
                Plugin.Log.Info($"CombatActions.json was created in {Plugin.PluginResources}");
            }
            else
            {
                Plugin.Log.Info("CombatActions.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatEnemyPassives()
    {
        try
        {
            List<CombatEnemyPassiveItem> passivesList = [];
            Plugin.Log.Info("Try find CombatEnemyPassive in Resources");
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> passivesObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(CombatEnemyPassive)));
            foreach (UnityEngine.Object passiveObj in passivesObj)
            {
                CombatEnemyPassive action = passiveObj.TryCast<CombatEnemyPassive>();
                if (action is not null)
                {
                    var item = CombatEnemyPassiveItem.FromCombatTalent(action);
                    passivesList.Add(item);
                }
            }

            if (passivesList.Count <= 0)
            {
                return;
            }

            Plugin.Log.Info($"Prepared {passivesList.Count}/{passivesObj.Count} CombatEnemyPassive items");
            if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatEnemyPassive.json", passivesList, false))
            {
                Plugin.Log.Info($"CombatEnemyPassive.json was created in {Plugin.PluginResources}");
            }
            else
            {
                Plugin.Log.Info("CombatEnemyPassive.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadInventoryItems()
    {
        try
        {
            if (Zessentials.Instance.gameObject.TryGetComponentWithCast(out CombatHolder holder))
            {
                List<Models.InventoryItem> items = [];

                items.Clear();
                Plugin.Log.Info("Get CombatConsumables from CombatHolder");
                foreach (CombatConsumable consumableItem in holder.consumables)
                {
                    items.Add(Models.InventoryItem.FromCombatConsumable(consumableItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatConsumables.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        Plugin.Log.Info($"CombatConsumables.json was created in {Plugin.PluginResources}");
                    }
                    else
                    {
                        Plugin.Log.Info("CombatConsumables.json was NOT created");
                    }
                }

                items.Clear();
                Plugin.Log.Info("Get CombatTrinkets from CombatHolder");
                foreach (CombatTrinket trinketItem in holder.trinkets)
                {
                    items.Add(Models.InventoryItem.FromCombatTrinket(trinketItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatTrinkets.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        Plugin.Log.Info($"CombatTrinkets.json was created in {Plugin.PluginResources}");
                    }
                    else
                    {
                        Plugin.Log.Info("CombatTrinkets.json was NOT created");
                    }
                }

                items.Clear();
                Plugin.Log.Info("Get CombatWeapons from CombatHolder");
                foreach (CombatWeapon weaponItem in holder.weapons)
                {
                    items.Add(Models.InventoryItem.FromCombatWeapon(weaponItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(Plugin.PluginResources, "CombatWeapons.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        Plugin.Log.Info($"CombatWeapons.json was created in {Plugin.PluginResources}");
                    }
                    else
                    {
                        Plugin.Log.Info("CombatWeapons.json was NOT created");
                    }
                }

                items.Clear();
                Plugin.Log.Info("Get QuestItems from CombatHolder");
                foreach (CombatItem questItem in holder.questItems)
                {
                    items.Add(Models.InventoryItem.FromCombatItem(questItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(Plugin.PluginResources, "QuestItems.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        Plugin.Log.Info($"QuestItems.json was created in {Plugin.PluginResources}");
                    }
                    else
                    {
                        Plugin.Log.Info("QuestItems.json was NOT created");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void UnloadColors()
    {
        try
        {
            ColorsList colors = new();
            CombatEnemyManager enemyManager = Zessentials.Instance.battleManager.enemyManager;

            foreach (Color color in enemyManager.EyeColors)
            {
                colors.EyesColors.Add(ColorsList.Color.FromUnityColor(color));
            }

            foreach (Color color in enemyManager.HairColors)
            {
                colors.HairColors.Add(ColorsList.Color.FromUnityColor(color));
            }

            foreach (Color color in enemyManager.SkinTones)
            {
                colors.SkinTones.Add(ColorsList.Color.FromUnityColor(color));
            }

            if (JsonUtils.TrySerialize(Plugin.PluginResources, "ColorsList.json", colors, false))
            {
                Plugin.Log.Info($"ColorsList.json was created in {Plugin.PluginResources}");
            }
            else
            {
                Plugin.Log.Info("ColorsList.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex.Message);
        }
    }
}
