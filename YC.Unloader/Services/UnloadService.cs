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
            if (File.Exists(Path.Combine(Core.PluginResources, "Clothing.json")))
            {
                return;
            }

            List<ClothesItem> clothingList = [];
            Core.LogInfo("Try find clothes in Resources");
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
            Core.LogInfo($"Prepared {clothingList.Count}/{clothesObj.Count} clothes items");
            if (JsonUtils.TrySerialize(Core.PluginResources, "Clothing.json", clothingList, false))
            {
                Core.LogSuccess($"Clothing.json was created in {Core.PluginResources}");
                File.WriteAllText($"{Core.PluginResources}/Female_ClothingIds.txt", string.Join(", ", clothingList.Where(c => !c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
                File.WriteAllText($"{Core.PluginResources}/Male_ClothingIds.txt", string.Join(", ", clothingList.Where(c => c.ForMale).OrderBy(c => c.ID).Select(c => c.ID)));
            }
            else
            {
                Core.LogInfo("Clothing.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void UnloadCombatBuffs()
    {
        try
        {
            if (File.Exists(Path.Combine(Core.PluginResources, "CombatBuffs.json")))
            {
                return;
            }

            List<CombatBuffItem> buffList = [];
            Core.LogInfo("Try find CombatBuff in Resources");
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

            Core.LogInfo($"Prepared {buffList.Count}/{buffsObj.Count} CombatBuff items");
            if (JsonUtils.TrySerialize(Core.PluginResources, "CombatBuffs.json", buffList, false))
            {
                Core.LogSuccess($"CombatBuffs.json was created in {Core.PluginResources}");
            }
            else
            {
                Core.LogInfo("CombatBuffs.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void UnloadCombatTalents()
    {
        try
        {
            if (File.Exists(Path.Combine(Core.PluginResources, "CombatTalents.json")))
            {
                return;
            }

            List<CombatTalentItem> talentsList = [];
            Core.LogInfo("Try find CombatTalents in Resources");
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

            Core.LogInfo($"Prepared {talentsList.Count}/{talentsObj.Count} CombatTalents items");
            if (JsonUtils.TrySerialize(Core.PluginResources, "CombatTalents.json", talentsList, false))
            {
                Core.LogSuccess($"CombatTalents.json was created in {Core.PluginResources}");
            }
            else
            {
                Core.LogInfo("CombatTalents.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void UnloadCombatActions()
    {
        try
        {
            if (File.Exists(Path.Combine(Core.PluginResources, "CombatActions.json")))
            {
                return;
            }

            List<CombatActionItem> actionsList = [];
            Core.LogInfo("Try find CombatAction in Resources");
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

            Core.LogInfo($"Prepared {actionsList.Count}/{actionsObj.Count} CombatAction items");
            if (JsonUtils.TrySerialize(Core.PluginResources, "CombatActions.json", actionsList, false))
            {
                Core.LogSuccess($"CombatActions.json was created in {Core.PluginResources}");
            }
            else
            {
                Core.LogInfo("CombatActions.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void UnloadCombatEnemyPassives()
    {
        try
        {
            if (File.Exists(Path.Combine(Core.PluginResources, "CombatEnemyPassive.json")))
            {
                return;
            }

            List<CombatEnemyPassiveItem> passivesList = [];
            Core.LogInfo("Try find CombatEnemyPassive in Resources");
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

            Core.LogInfo($"Prepared {passivesList.Count}/{passivesObj.Count} CombatEnemyPassive items");
            if (JsonUtils.TrySerialize(Core.PluginResources, "CombatEnemyPassive.json", passivesList, false))
            {
                Core.LogSuccess($"CombatEnemyPassive.json was created in {Core.PluginResources}");
            }
            else
            {
                Core.LogInfo("CombatEnemyPassive.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError( ex );
        }
    }

    internal static void UnloadInventoryItems()
    {
        try
        {
            if (Zessentials.Instance.gameObject.TryGetComponentWithCast(out CombatHolder holder))
            {
                List<Models.InventoryItem> items = [];

                if (!File.Exists(Path.Combine(Core.PluginResources, "CombatConsumables.json")))
                {
                    items.Clear();
                    Core.LogInfo("Get CombatConsumables from CombatHolder");
                    foreach (CombatConsumable consumableItem in holder.consumables)
                    {
                        items.Add(Models.InventoryItem.FromCombatConsumable(consumableItem));
                    }

                    if (items.Count > 0)
                    {
                        if (JsonUtils.TrySerialize(Core.PluginResources, "CombatConsumables.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                        {
                            Core.LogSuccess($"CombatConsumables.json was created in {Core.PluginResources}");
                        }
                        else
                        {
                            Core.LogInfo("CombatConsumables.json was NOT created");
                        }
                    }
                }

                if (!File.Exists(Path.Combine(Core.PluginResources, "CombatTrinkets.json")))
                {
                    items.Clear();
                    Core.LogInfo("Get CombatTrinkets from CombatHolder");
                    foreach (CombatTrinket trinketItem in holder.trinkets)
                    {
                        items.Add(Models.InventoryItem.FromCombatTrinket(trinketItem));
                    }

                    if (items.Count > 0)
                    {
                        if (JsonUtils.TrySerialize(Core.PluginResources, "CombatTrinkets.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                        {
                            Core.LogSuccess($"CombatTrinkets.json was created in {Core.PluginResources}");
                        }
                        else
                        {
                            Core.LogInfo("CombatTrinkets.json was NOT created");
                        }
                    }
                }

                if (!File.Exists(Path.Combine(Core.PluginResources, "CombatWeapons.json")))
                {
                    items.Clear();
                    Core.LogInfo("Get CombatWeapons from CombatHolder");
                    foreach (CombatWeapon weaponItem in holder.weapons)
                    {
                        items.Add(Models.InventoryItem.FromCombatWeapon(weaponItem));
                    }

                    if (items.Count > 0)
                    {
                        if (JsonUtils.TrySerialize(Core.PluginResources, "CombatWeapons.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                        {
                            Core.LogSuccess($"CombatWeapons.json was created in {Core.PluginResources}");
                        }
                        else
                        {
                            Core.LogInfo("CombatWeapons.json was NOT created");
                        }
                    }
                }

                if (!File.Exists(Path.Combine(Core.PluginResources, "QuestItems.json")))
                {
                    items.Clear();
                    Core.LogInfo("Get QuestItems from CombatHolder");
                    foreach (CombatItem questItem in holder.questItems)
                    {
                        items.Add(Models.InventoryItem.FromCombatItem(questItem));
                    }

                    if (items.Count > 0)
                    {
                        if (JsonUtils.TrySerialize(Core.PluginResources, "QuestItems.json", items.OrderBy(i => i.Type).ThenBy(i => i.Name), false))
                        {
                            Core.LogSuccess($"QuestItems.json was created in {Core.PluginResources}");
                        }
                        else
                        {
                            Core.LogInfo("QuestItems.json was NOT created");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
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

            if (JsonUtils.TrySerialize(Core.PluginResources, "ColorsList.json", colors, false))
            {
                Core.LogSuccess($"ColorsList.json was created in {Core.PluginResources}");
            }
            else
            {
                Core.LogInfo("ColorsList.json was NOT created");
            }
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }
}
