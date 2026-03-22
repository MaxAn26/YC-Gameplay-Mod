using System;
using System.Collections.Generic;
using System.Linq;

using BaseMod.Core.Extensions;

using Il2Cpp;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace YC.GameTrainerMod.Components;
public class GameTrainerComponent : MonoBehaviour {
    private static bool _guiCtrlVisible = false;
    private static bool _guiShiftVisible = false;

    private bool _isInitialized;
    private Il2Cpp.Console _console;
    private CombatHolder _combatHolder;

    #region ctor
    static GameTrainerComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<GameTrainerComponent>();
    }

    public GameTrainerComponent() : base(ClassInjector.DerivedConstructorPointer<GameTrainerComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public GameTrainerComponent(IntPtr pointer) : base(pointer) {

    }

    public void Initialize() {
        try {
            if (_isInitialized)
                return;
            
            _console = Zessentials.Instance.gameObject.GetComponentWithCast<Il2Cpp.Console>();
            _combatHolder = Zessentials.Instance.gameObject.GetComponentWithCast<CombatHolder>();

            Plugin.Log.Info("Register class in Zessentials game object");
            _isInitialized = true;
        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }
    #endregion

    #region Unity
    public void Update() {
        if (SceneManager.GetActiveScene().buildIndex < 2 || Zessentials.Instance.battleManager.isInBattle)
            return;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKeyDown(KeyCode.F1)) {
                _guiCtrlVisible = !_guiCtrlVisible;
                _guiShiftVisible = false;
            }

            if (_guiCtrlVisible) {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    AddCredits();

                if (Input.GetKeyDown(KeyCode.Alpha2))
                    AddCombatPoints();

                if (Input.GetKeyDown(KeyCode.Alpha3))
                    AddEroPoints();

                if (Input.GetKeyDown(KeyCode.Alpha4))
                    OpenAllWeapons();

                if (Input.GetKeyDown(KeyCode.Alpha5))
                    OpenAllTrinkets();

                if (Input.GetKeyDown(KeyCode.Alpha6))
                    AddScrollsAndStones();

                if (Input.GetKeyDown(KeyCode.Alpha7))
                    AddTonics();

                if (Input.GetKeyDown(KeyCode.Alpha0))
                    AddManuals();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (Input.GetKeyDown(KeyCode.F1)) {
                _guiShiftVisible = !_guiShiftVisible;
                _guiCtrlVisible = false;
            }

            if (_guiShiftVisible) {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    UpLevelToMax();

                if (Input.GetKeyDown(KeyCode.Alpha2))
                    SwitchGodMod();
            }
        }
    }

    public void OnGUI() {
        if (SceneManager.GetActiveScene().buildIndex < 2 || Zessentials.Instance.battleManager.isInBattle)
            return;

        if (!_guiCtrlVisible && !_guiShiftVisible) {
            GUI.Label(new Rect(Screen.width - 260f, 10f, 200f, 20f), "Ctrl + F1: Show Trainer menu for Control key");
            GUI.Label(new Rect(Screen.width - 260f, 30f, 200f, 20f), "Shift + F1: Show Trainer menu for Alt key");
            return;
        }

        float x = Screen.width - 260f;

        if (_guiCtrlVisible) {
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

        if (_guiShiftVisible) {
            GUI.Label(new Rect(x, 10f, 250f, 20f), "Shift + F1: Hide");
            GUI.Label(new Rect(x, 30f, 250f, 20f), "Shift + Num 1: Up character level to max");
            GUI.Label(new Rect(x, 50f, 250f, 20f), $"Shift + Num 2: God Mode {(IsGodMode ? "Enabled" : "Disabled")}");
        }
    }
    #endregion

    #region Control mathods
    private void AddCredits() {
        CharacterDataa.Instance.credits += 100000;
        _console?.ConsoleWrite( "Add 100 000 credits" );
    }

    private void AddCombatPoints() {
        CharacterDataa.Instance.statusDATA.freeCombatTalentPoints += 100;
        _console?.ConsoleWrite("Add 100 combat talent points");
    }

    private void AddEroPoints() {
        CharacterDataa.Instance.statusDATA.freeEroTalentPoints += 100;
        _console?.ConsoleWrite("Add 100 ero talent points");
    }

    private void OpenAllWeapons() {
        try {
            List<InventoryItem> weapons = [];
            foreach (var item in CharacterDataa.Instance.inventory.items) {
                if (item.itemType is 1 && !item.itemName.Equals("Nothing") && !item.itemName.Equals("Unarmed"))
                    weapons.Add(item);
            }

            foreach (var weapon in weapons) {
                if (CharacterDataa.Instance.equippedWeapon != weapon.id) {
                    Plugin.Log.Debug($"Remove weapon: {weapon.itemName}");
                    CharacterDataa.Instance.inventory.RemoveItemById(weapon.id, 1);
                }
            }

            List<CombatWeapon> newWeapons = [.. _combatHolder.weapons];
            newWeapons = [.. newWeapons.OrderBy(w => w.itemName)];

            foreach (var newWeapon in newWeapons) {
                if (!CharacterDataa.Instance.inventory.HasItem(newWeapon.itemName)) {
                    Plugin.Log.Debug($"Add weapon: {newWeapon.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(newWeapon.itemName, newWeapon.itemType, 1, newWeapon.itemQuality, newWeapon.itemPrice);
                }
            }

            _console.ConsoleWrite( "All weapons was opened" );
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }

    private void OpenAllTrinkets() {
        try {
            List<InventoryItem> trinkets = [];
            foreach (var item in CharacterDataa.Instance.inventory.items) {
                if (item.itemType is 7 && !item.itemName.Equals("Nothing") && !item.itemName.Equals("Unarmed"))
                    trinkets.Add(item);
            }

            foreach (var trinket in trinkets) {
                if (CharacterDataa.Instance.equippedTrinket != trinket.id && CharacterDataa.Instance.equippedTrinket2 != trinket.id) {
                    Plugin.Log.Debug($"Remove trinket: {trinket.itemName}");
                    CharacterDataa.Instance.inventory.RemoveItemById(trinket.id, 1);
                }
            }

            List<CombatTrinket> newTrinkets = [.. _combatHolder.trinkets];
            newTrinkets = [..newTrinkets.OrderBy(t => t.itemName)];

            foreach (var newTrinket in newTrinkets) {
                if (!CharacterDataa.Instance.inventory.HasItem(newTrinket.itemName)) {
                    Plugin.Log.Debug($"Add trinket: {newTrinket.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(newTrinket.itemName, newTrinket.itemType, 1, newTrinket.itemQuality, newTrinket.itemPrice);
                }
            }

            _console.ConsoleWrite("All trinkets was opened");
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }

    private void AddScrollsAndStones() {
        try {
            List<CombatConsumable> consumables = [.. _combatHolder.consumables];
            consumables = [.. consumables.Where(c => c.itemType == 6).OrderBy(c => c.itemName)];

            foreach (var consumable in consumables) {
                Plugin.Log.Debug($"Add consumable: {consumable.itemName} x100");
                CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 100, consumable.itemQuality, consumable.itemPrice);
            }

            _console.ConsoleWrite("All scrolls and stones was added in your Inventory");
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }

    private void AddTonics() {
        try {
            List<CombatConsumable> consumables = [.. _combatHolder.consumables];
            consumables = [.. consumables.Where(c => c.itemType == 3).OrderBy(c => c.itemName)];

            foreach (var consumable in consumables) {
                Plugin.Log.Debug($"Add consumable: {consumable.itemName} x100");
                CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 100, consumable.itemQuality, consumable.itemPrice);
            }

            _console.ConsoleWrite("All tonics was added in your Inventory");
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }

    private void AddManuals() {
        try {
            List<string> actionsList = [];
            var actionsObj = Resources.FindObjectsOfTypeAll( Il2CppType.From( typeof(CombatAction) ) );
            foreach (var actionObj in actionsObj) {
                var action = actionObj.TryCast<CombatAction>();
                if (action is not null) {
                    actionsList.Add(action.actionName);
                }
            }

            List<CombatConsumable> consumables = [.. _combatHolder.consumables];
            consumables = [.. consumables.Where( c => c.itemType == 4 && actionsList.Contains(c.consumableEffectAlternative)).OrderBy(c => c.itemName)];

            foreach (var consumable in consumables) {
                if (!consumable.itemName.Equals("Sex databook") && !CharacterDataa.Instance.inventory.HasItem(consumable.itemName) && !IsKnown(consumable.consumableEffectAlternative)) {
                    Plugin.Log.Debug($"Add consumable: {consumable.itemName}");
                    CharacterDataa.Instance.inventory.AddItem(consumable.itemName, consumable.itemType, 1, consumable.itemQuality, consumable.itemPrice);
                }
            }

            _console.ConsoleWrite("All manuals was added in your Inventory");
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }

        static bool IsKnown( string manualName ) {
            var statusData = CharacterDataa.Instance.statusDATA;

            if (string.IsNullOrWhiteSpace(manualName))
                return false;

            if (statusData.unlockedStandingAbilities.Contains(manualName) || statusData.unlockedGrappledAbilities.Contains(manualName))
                return true;

            return false;
        }
    }
    #endregion Control mathods

    #region Shift mathods
    private void UpLevelToMax() {
        try {
            Zessentials zessentials = Zessentials.Instance;

            if (CharacterDataa.Instance.characterLevel >= Zessentials.currentMaxLevel)
                return;

            while(CharacterDataa.Instance.characterLevel < Zessentials.currentMaxLevel) {
                int req = CombatExperienceManager.CalculateXPRequiredForLevel( CharacterDataa.Instance.characterLevel + 1 );
                if (CombatExperienceManager.AddExperience(req + 10, false))
                    Zessentials.Instance.battleManager.characterPlayer.LevelUp();
            }
                        
            _console?.ConsoleWrite($"Your level was increased till: {CharacterDataa.Instance.characterLevel}");
        } catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }

    internal static bool IsGodMode = false;
    private void SwitchGodMod() {
        IsGodMode = !IsGodMode;
        _console?.ConsoleWrite($"God Mode: {(IsGodMode ? "Activated" : "Deactivated")}");
    }
    #endregion Shift mathods

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) => RegisterClass(monoBehaviour.gameObject);

    [HideFromIl2Cpp]
    public static void RegisterClass(GameObject gameObject) {
        gameObject.AddComponentWithAction<GameTrainerComponent>(component => component.Initialize());
    }
}
