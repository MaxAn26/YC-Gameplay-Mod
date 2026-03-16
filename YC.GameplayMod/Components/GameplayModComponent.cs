using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;

using YC.GameplayMod.Models;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Components;
public class GameplayModComponent : MonoBehaviour {
    private bool _componentInitialized = false;
    private bool _sexInteractionSet = false;
    private CharacterRole _characterRole = CharacterRole.None;

    internal CharacterSex Sex { get; private set; }
    internal CharacterAttributes Attributes => Sex.characterAttributes;

    internal int CumsCount { get; set; } = 0;
    internal int SexCount { get; set; } = 0;
    internal int PersonalityId { get; set; }
    internal bool IsActiveRole => _characterRole switch { 
        CharacterRole.Active => true,
        CharacterRole.Passive => false,
        CharacterRole.Any => RandomUtils.Chance( Sex.IsFuta || ( Sex.IsMale && !Sex.IsFemboy) ? 75 : 50 ),
        _ => false,
    };

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

            if (_componentInitialized)
                return;

            if (gameObject.TryGetComponentWithCast(out CharacterSex characterSex)) {
                Plugin.Log.Info($"Register class for character {characterSex.characterName}");
                Sex = characterSex;

                LateInitialize();
            } else {
                Destroy(this);
            }

        } catch (Exception e) {
            Plugin.Log.Error(e);
            Destroy(this);
        }
    }

    public void LateUpdate() {
        LateInitialize();

        if (Sex.IsCumming) {
            if (!_sexInteractionSet) {
                _sexInteractionSet = true;

                SexCount++;
                if (Sex.ThisCharacterCumming) {
                    CumsCount++;
                }

                if (CharacterDataa.Instance.adultSettingsDATA.messyMakeup && (CumsCount >= 2 || SexCount >= 5)) {
                    Sex.SetMessyMakeup();
                }
            }
        } else {
            _sexInteractionSet = false;
        }
    }

    /*public void FixedUpdate() {
        if (Attributes is null)
            return;

        if (SexCount > 0) {
            if (Sex.IsGrappled) {
                _timer = 30f;
            } else if (Attributes.currentPleasure > 0) {
                _timer = 30f;
            } else if (Attributes.currentPleasure == 0) {
                if (_timer > 0) {
                    _timer -= Time.deltaTime;
                } else {
                    SexCount--;
                    Plugin.Log.Debug($"{Sex.characterName}: reduce SexCount");
                    _timer = 30f;
                }
            }
        } else {
            _timer = 0f;
        }
    }*/

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) {
        monoBehaviour.gameObject.AddComponentWithAction<GameplayModComponent>(component => component.Initialize());
    }

    private void LateInitialize() {
        if (Attributes is null || _componentInitialized)
            return;

        _characterRole = DefineCharacterRole(Sex.CharacterRole);

        if (Attributes.isPlayer) {
            PersonalityId = CharacterDataa.Instance.adultSettingsDATA.SexGameplayAI;
        } else {
            PersonalityId = Attributes.enemyData?.statsDATA.EnemyPersonality ?? 0;

            if ( Attributes.isAreaBoss || Attributes.combatAI.isElite == true ) {
                if ( SexChoiceRealismMod.RandomPersonalityElite.Value )
                    PersonalityId = RandomUtils.Int32(1, 6);
            } else if (Attributes.combatAI.isAlly) {
                if( SexChoiceRealismMod.RandomPersonalityAlly.Value )
                    PersonalityId = RandomUtils.Int32(1, 6);
            } else if (SexChoiceRealismMod.RandomPersonalityEnemy.Value) {
                PersonalityId = RandomUtils.Int32(1, 6);
            }
        }

        Plugin.Log.Info( $"{Sex.characterName}: Role: {_characterRole}, PersonalityId: {PersonalityId}" );

        _componentInitialized = true;
    }

    private static CharacterRole DefineCharacterRole(int roleId) {
        return roleId switch {
            0 => CharacterRole.Passive,
            1 => CharacterRole.Active,
            2 => CharacterRole.Any,
            3 => RandomUtils.Chance(50) ? CharacterRole.Active : CharacterRole.Passive,
            _ => CharacterRole.Any,
        };
    }
}
