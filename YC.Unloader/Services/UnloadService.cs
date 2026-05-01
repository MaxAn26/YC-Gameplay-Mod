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
            UnloaderMod.Log.Msg("Try find clothes in Resources");
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
            UnloaderMod.Log.Msg($"Prepared {clothingList.Count}/{clothesObj.Count} clothes items");
            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "Clothing.json", clothingList, false))
            {
                UnloaderMod.Log.Msg($"Clothing.json was created in {UnloaderMod.PluginResources}");
                File.WriteAllText($"{UnloaderMod.PluginResources}/Female_ClothingIds.txt", string.Join(", ", clothingList.Where(c => !c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
                File.WriteAllText($"{UnloaderMod.PluginResources}/Male_ClothingIds.txt", string.Join(", ", clothingList.Where(c => c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
            }
            else
            {
                UnloaderMod.Log.Msg("Clothing.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatBuffs()
    {
        try
        {
            List<CombatBuffItem> buffList = [];
            UnloaderMod.Log.Msg("Try find CombatBuff in Resources");
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

            UnloaderMod.Log.Msg($"Prepared {buffList.Count}/{buffsObj.Count} CombatBuff items");
            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatBuffs.json", buffList, false))
            {
                UnloaderMod.Log.Msg($"CombatBuffs.json was created in {UnloaderMod.PluginResources}");
            }
            else
            {
                UnloaderMod.Log.Msg("CombatBuffs.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatTalents()
    {
        try
        {
            List<CombatTalentItem> talentsList = [];
            UnloaderMod.Log.Msg("Try find CombatTalents in Resources");
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

            UnloaderMod.Log.Msg($"Prepared {talentsList.Count}/{talentsObj.Count} CombatTalents items");
            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatTalents.json", talentsList, false))
            {
                UnloaderMod.Log.Msg($"CombatTalents.json was created in {UnloaderMod.PluginResources}");
            }
            else
            {
                UnloaderMod.Log.Msg("CombatTalents.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatActions()
    {
        try
        {
            List<CombatActionItem> actionsList = [];
            UnloaderMod.Log.Msg("Try find CombatAction in Resources");
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

            UnloaderMod.Log.Msg($"Prepared {actionsList.Count}/{actionsObj.Count} CombatAction items");
            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatActions.json", actionsList, false))
            {
                UnloaderMod.Log.Msg($"CombatActions.json was created in {UnloaderMod.PluginResources}");
            }
            else
            {
                UnloaderMod.Log.Msg("CombatActions.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
        }
    }

    internal static void UnloadCombatEnemyPassives()
    {
        try
        {
            List<CombatEnemyPassiveItem> passivesList = [];
            UnloaderMod.Log.Msg("Try find CombatEnemyPassive in Resources");
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

            UnloaderMod.Log.Msg($"Prepared {passivesList.Count}/{passivesObj.Count} CombatEnemyPassive items");
            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatEnemyPassive.json", passivesList, false))
            {
                UnloaderMod.Log.Msg($"CombatEnemyPassive.json was created in {UnloaderMod.PluginResources}");
            }
            else
            {
                UnloaderMod.Log.Msg("CombatEnemyPassive.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
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
                UnloaderMod.Log.Msg("Get CombatConsumables from CombatHolder");
                foreach (CombatConsumable consumableItem in holder.consumables)
                {
                    items.Add(Models.InventoryItem.FromCombatConsumable(consumableItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatConsumables.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        UnloaderMod.Log.Msg($"CombatConsumables.json was created in {UnloaderMod.PluginResources}");
                    }
                    else
                    {
                        UnloaderMod.Log.Msg("CombatConsumables.json was NOT created");
                    }
                }

                items.Clear();
                UnloaderMod.Log.Msg("Get CombatTrinkets from CombatHolder");
                foreach (CombatTrinket trinketItem in holder.trinkets)
                {
                    items.Add(Models.InventoryItem.FromCombatTrinket(trinketItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatTrinkets.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        UnloaderMod.Log.Msg($"CombatTrinkets.json was created in {UnloaderMod.PluginResources}");
                    }
                    else
                    {
                        UnloaderMod.Log.Msg("CombatTrinkets.json was NOT created");
                    }
                }

                items.Clear();
                UnloaderMod.Log.Msg("Get CombatWeapons from CombatHolder");
                foreach (CombatWeapon weaponItem in holder.weapons)
                {
                    items.Add(Models.InventoryItem.FromCombatWeapon(weaponItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "CombatWeapons.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        UnloaderMod.Log.Msg($"CombatWeapons.json was created in {UnloaderMod.PluginResources}");
                    }
                    else
                    {
                        UnloaderMod.Log.Msg("CombatWeapons.json was NOT created");
                    }
                }

                items.Clear();
                UnloaderMod.Log.Msg("Get QuestItems from CombatHolder");
                foreach (CombatItem questItem in holder.questItems)
                {
                    items.Add(Models.InventoryItem.FromCombatItem(questItem));
                }

                if (items.Count > 0)
                {
                    if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "QuestItems.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                    {
                        UnloaderMod.Log.Msg($"QuestItems.json was created in {UnloaderMod.PluginResources}");
                    }
                    else
                    {
                        UnloaderMod.Log.Msg("QuestItems.json was NOT created");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
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

            if (JsonUtils.TrySerialize(UnloaderMod.PluginResources, "ColorsList.json", colors, false))
            {
                UnloaderMod.Log.Msg($"ColorsList.json was created in {UnloaderMod.PluginResources}");
            }
            else
            {
                UnloaderMod.Log.Msg("ColorsList.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            UnloaderMod.Log.Error(ex.Message);
        }
    }
}
