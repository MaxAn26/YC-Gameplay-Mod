using System;

using BaseMod.Core.Extensions;

using Il2Cpp;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Components;
public class GameplayModComponent : MonoBehaviour {
    internal CharacterSex Sex { get; private set; }
    internal CharacterAttributes Attributes { get; private set; }

    internal int SexInteractions { get; set; } = 0;
    internal bool IsActiveRole { get; private set; } = false;
    private bool _sexInteractionSet = false;

    static GameplayModComponent() {
        ClassInjector.RegisterTypeInIl2Cpp<GameplayModComponent>();
    }

    public GameplayModComponent() : base(ClassInjector.DerivedConstructorPointer<GameplayModComponent>()) {
        ClassInjector.DerivedConstructorBody(this);
    }

    public GameplayModComponent(IntPtr pointer) : base(pointer) {

    }

    public void Initialize() {
        try {
            if (!SexChoiceRealismMod.IsModActive)
                Destroy(this);

            if (gameObject.TryGetComponentWithCast(out CharacterSex characterSex)) {
                Plugin.Log.Info($"Register class for character {characterSex.characterName}");
                Sex = characterSex;
                Attributes = characterSex.characterAttributes;
                IsActiveRole = Sex.IsActive;
                Plugin.Log.Debug($"Saved Role: {(IsActiveRole ? "Active" : "Passive")}");
            } else {
                Destroy(this);
            }

        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public void LateUpdate() {
        if (Sex.currentSexEncounter is not null && Sex.currentSexEncounter.IsCumming) {
            if (!_sexInteractionSet) {
                SexInteractions++;
                _sexInteractionSet = true;
            }
        } else {
            _sexInteractionSet = false;
        }
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) {
        monoBehaviour.gameObject.AddComponentWithAction<GameplayModComponent>(component => component.Initialize());
    }
}