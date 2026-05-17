using System.Runtime.CompilerServices;
using BaseMod.Core.Extensions;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using MelonLoader.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using YC.GameTrainerMod;

[assembly: HarmonyDontPatchAll]
[assembly: MelonInfo(typeof(Core), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace YC.GameTrainerMod;
public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonEvents.OnUpdate.Subscribe(TrainerOnUpdate, 100);
        MelonEvents.OnGUI.Subscribe(TrainerOnGUI, 100);

        HarmonyInstance.PatchAll();

        base.OnInitializeMelon();

        LogInfo($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    private void WriteConsole(string message)
    {
        Il2Cpp.Console console = Zessentials.Instance.gameObject.GetComponentWithCast<Il2Cpp.Console>();
        console?.ConsoleWrite(message);
    }

    private void TrainerOnUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex < 2 || Zessentials.Instance.battleManager.isInBattle)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _guiCtrlVisible = !_guiCtrlVisible;
                _guiShiftVisible = false;
            }

            if (_guiCtrlVisible)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    AddCredits();
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    AddCombatPoints();
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    AddEroPoints();
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    OpenAllWeapons();
                }

                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    OpenAllTrinkets();
                }

                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    AddScrollsAndStones();
                }

                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    AddTonics();
                }

                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    AddManuals();
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _guiShiftVisible = !_guiShiftVisible;
                _guiCtrlVisible = false;
            }

            if (_guiShiftVisible)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    UpLevelToMax();
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SwitchGodMod();
                }
            }
        }
    }

    private void TrainerOnGUI()
    {
        if (SceneManager.GetActiveScene().buildIndex < 2 || Zessentials.Instance.battleManager.isInBattle)
        {
            return;
        }
        if (!_guiCtrlVisible && !_guiShiftVisible)
        {
            GUI.Label(new Rect(Screen.width - 260f, 10f, 200f, 20f), "Ctrl + F1: Show Trainer menu for Control key");
            GUI.Label(new Rect(Screen.width - 260f, 30f, 200f, 20f), "Shift + F1: Show Trainer menu for Shift key");
            return;
        }

        float x = Screen.width - 260f;

        if (_guiCtrlVisible)
        {
            GUI.Label(new Rect(x, 10f, 250f, 20f), "Ctrl + F1: Hide");
            GUI.Label(new Rect(x, 30f, 250f, 20f), "Ctrl + 1: Add 100.000 credits");
            GUI.Label(new Rect(x, 50f, 250f, 20f), "Ctrl + 2: Add 100 Combat talent points");
            GUI.Label(new Rect(x, 70f, 250f, 20f), "Ctrl + 3: Add 100 Ero talent points");
            GUI.Label(new Rect(x, 90f, 250f, 20f), "Ctrl + 4: Open all weapons");
            GUI.Label(new Rect(x, 110f, 250f, 20f), "Ctrl + 5: Open all trinkets");
            GUI.Label(new Rect(x, 130f, 250f, 20f), "Ctrl + 6: Add all scrolls & stones (x100)");
            GUI.Label(new Rect(x, 150f, 250f, 20f), "Ctrl + 7: Add all tonics (x100)");
            GUI.Label(new Rect(x, 170f, 250f, 20f), "Ctrl + 0: Add all manuals");
        }

        if (_guiShiftVisible)
        {
            GUI.Label(new Rect(x, 10f, 250f, 20f), "Shift + F1: Hide");
            GUI.Label(new Rect(x, 30f, 250f, 20f), "Shift + Num 1: Up character level to max");
            GUI.Label(new Rect(x, 50f, 250f, 20f), $"Shift + Num 2: God Mode {(IsGodMode ? "Enabled" : "Disabled")}");
        }
    }

    #region Control mathods
    private bool _guiCtrlVisible = false;

    private void AddCredits()
    {
        CharacterDataa.Instance.credits += 100000;
        WriteConsole("Add 100 000 credits");
    }

    private void AddCombatPoints()
    {
        CharacterDataa.Instance.statusDATA.freeCombatTalentPoints += 100;
        WriteConsole( "Add 100 combat talent points" );
    }

    private void AddEroPoints()
    {
        CharacterDataa.Instance.statusDATA.freeEroTalentPoints += 100;
        WriteConsole( "Add 100 ero talent points" );
    }

    private void OpenAllWeapons()
    {
        try
        {
            CombatHolder combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();
            if (combatHolder is null)
            {
                return;
            }

            foreach (InventoryItem item in CharacterDataa.Instance.inventory.items)
            {
                if (item.itemType is 1 && !item.itemName.Equals("Nothing") && !item.itemName.Equals("Unarmed"))
                {
                    if (item.quality < 4)
                    {
                        for (int i = item.quality; i <= 4; i++)
                        {
                            CharacterDataa.Instance.inventory.UpgradeItemQualityById(item.id);
                        }
                    }
                }
            }

            List<CombatWeapon> newWeapons = [.. combatHolder.weapons];
            newWeapons = [.. newWeapons.OrderBy(w => w.itemName)];

            foreach (CombatWeapon newWeapon in newWeapons)
            {
                if (!CharacterDataa.Instance.inventory.HasItem(newWeapon.itemName))
                {
                    LogInfo($"Add weapon: {newWeapon.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(newWeapon.itemName, newWeapon.itemType, 1, 4, newWeapon.itemPrice);
                }
            }

            WriteConsole("All weapons was opened and upgraded");
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }

    private void OpenAllTrinkets()
    {
        try
        {
            CombatHolder combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();
            if (combatHolder is null)
            {
                return;
            }

            foreach (InventoryItem item in CharacterDataa.Instance.inventory.items)
            {
                if (item.itemType is 7 && !item.itemName.Equals("Nothing") && !item.itemName.Equals("Unarmed"))
                {
                    if (item.quality < 4)
                    {
                        for (int i = item.quality; i <= 4; i++)
                        {
                            CharacterDataa.Instance.inventory.UpgradeItemQualityById(item.id);
                        }
                    }
                }
            }

            List<CombatTrinket> newTrinkets = [.. combatHolder.trinkets];
            newTrinkets = [.. newTrinkets.OrderBy(t => t.itemName)];

            foreach (CombatTrinket newTrinket in newTrinkets)
            {
                if (!CharacterDataa.Instance.inventory.HasItem(newTrinket.itemName))
                {
                    LogInfo($"Add trinket: {newTrinket.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(newTrinket.itemName, newTrinket.itemType, 1, 4, newTrinket.itemPrice);
                }
            }

            WriteConsole("All trinkets was opened and upgraded");
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }

    private void AddScrollsAndStones()
    {
        try
        {
            CombatHolder combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();
            if (combatHolder is null)
            {
                return;
            }

            List<CombatConsumable> consumables = [.. combatHolder.consumables];
            consumables = [.. consumables.Where(c => c.itemType == 6).OrderBy(c => c.itemName)];

            foreach (CombatConsumable consumable in consumables)
            {
                LogInfo($"Add consumable: {consumable.itemName} x100");
                CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 100, consumable.itemQuality, consumable.itemPrice);
            }

            List<CombatItem> questItems = [.. combatHolder.questItems];
            foreach (CombatItem questItem in questItems)
            {
                if (!questItem.itemName.Equals("Credits"))
                {
                    LogInfo($"Add consumable: {questItem.itemName} x100");
                    CharacterDataa.Instance.inventory.AddItem(questItem.itemName, questItem.itemType, 100, questItem.itemQuality, questItem.itemPrice);
                }
            }

            WriteConsole("All scrolls and stones was added in your Inventory");
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }

    private void AddTonics()
    {
        try
        {
            CombatHolder combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();
            if (combatHolder is null)
            {
                return;
            }

            List<CombatConsumable> consumables = [.. combatHolder.consumables];
            consumables = [.. consumables.Where(c => c.itemType == 3).OrderBy(c => c.itemName)];

            foreach (CombatConsumable consumable in consumables)
            {
                LogInfo($"Add consumable: {consumable.itemName} x100");
                CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 100, consumable.itemQuality, consumable.itemPrice);
            }

            WriteConsole("All tonics was added in your Inventory");
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }

    private void AddManuals()
    {
        try
        {
            CombatHolder combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();
            if (combatHolder is null)
            {
                return;
            }

            List<string> actionsList = [];
            Il2CppReferenceArray<UnityEngine.Object> actionsObj = Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(CombatAction)));
            foreach (UnityEngine.Object actionObj in actionsObj)
            {
                CombatAction action = actionObj.TryCast<CombatAction>();
                if (action is not null)
                {
                    actionsList.Add(action.actionName);
                }
            }

            List<CombatConsumable> consumables = [.. combatHolder.consumables];
            consumables = [.. consumables.Where(c => c.itemType == 4 && actionsList.Contains(c.consumableEffectAlternative)).OrderBy(c => c.itemName)];

            foreach (CombatConsumable consumable in consumables)
            {
                if (!consumable.itemName.Equals("Sex databook") && !CharacterDataa.Instance.inventory.HasItem(consumable.itemName) && !IsKnown(consumable.consumableEffectAlternative))
                {
                    LogInfo($"Add consumable: {consumable.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 1, consumable.itemQuality, consumable.itemPrice);
                }
            }

            WriteConsole("All manuals was added in your Inventory");
        }
        catch (Exception e)
        {
            LogError(e);
        }

        static bool IsKnown(string manualName)
        {
            CharacterDataa.StatusDATA statusData = CharacterDataa.Instance.statusDATA;

            if (string.IsNullOrWhiteSpace(manualName))
            {
                return false;
            }

            if (statusData.unlockedStandingAbilities.Contains(manualName) || statusData.unlockedGrappledAbilities.Contains(manualName))
            {
                return true;
            }

            return false;
        }
    }
    #endregion Control mathods

    #region Shift mathods
    private bool _guiShiftVisible = false;

    private void UpLevelToMax()
    {
        try
        {
            Zessentials zessentials = Zessentials.Instance;

            if (CharacterDataa.Instance.characterLevel >= Zessentials.currentMaxLevel)
            {
                return;
            }

            while (CharacterDataa.Instance.characterLevel < Zessentials.currentMaxLevel)
            {
                int req = CombatExperienceManager.CalculateXPRequiredForLevel(CharacterDataa.Instance.characterLevel + 1);
                if (CombatExperienceManager.AddExperience(req + 10, false))
                {
                    Zessentials.Instance.battleManager.characterPlayer.LevelUp();
                }
            }

            WriteConsole($"Your level was increased till: {CharacterDataa.Instance.characterLevel}");
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }

    internal static bool IsGodMode = false;
    private void SwitchGodMod()
    {
        IsGodMode = !IsGodMode;
        WriteConsole($"God Mode: {(IsGodMode ? "Activated" : "Deactivated")}");
    }
    #endregion Shift mathods

    public static void LogTrace(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.DarkGray, Combine(message, methodName));

    public static void LogDebug(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Cyan, Combine(message, methodName));

    public static void LogInfo(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.White, Combine(message, methodName));
    public static void LogSuccess(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Green, Combine(message, methodName));
    public static void LogWarning(string message, [CallerMemberName] string methodName = null) => MelonLogger.Warning(Combine(message, methodName));
    public static void LogError(Exception excepion, [CallerMemberName] string methodName = null) => LogError(excepion.Message, excepion, methodName);
    public static void LogError(string message, Exception excepion, [CallerMemberName] string methodName = null) => MelonLogger.Error(Combine(message, methodName), excepion);

    private static string Combine(string message, string methodName) => !string.IsNullOrWhiteSpace(methodName) ? $"{methodName}:> {message}" : message;
}
