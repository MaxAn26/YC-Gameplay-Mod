using System;
using System.Xml;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;

using UnityEngine;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Components;
public class GameplayModComponent : MonoBehaviour {
    internal CharacterSex Sex { get; private set; }
    internal CharacterAttributes Attributes { get; private set; }

    internal int CumsCount { get; set; } = 0;
    internal int SexCount { get; set; } = 0;
    internal int PersonalityId { get; private set; } = 0;
    internal bool IsActiveRole { get; private set; } = false;

    private bool _componentInitialized = false;
    private bool _sexInteractionSet = false;
    private float _timer = 0f;

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
                IsActiveRole = Sex.IsActive;

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

    public void FixedUpdate() {
        LateInitialize();

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
    }

    [HideFromIl2Cpp]
    public static void RegisterClass(MonoBehaviour monoBehaviour) {
        monoBehaviour.gameObject.AddComponentWithAction<GameplayModComponent>(component => component.Initialize());
    }

    private void LateInitialize() {
        if (Attributes is not null)
            return;

        if (Sex.characterAttributes is not null) {
            Attributes = Sex.characterAttributes;

            if (Attributes.isPlayer) {
                PersonalityId = CharacterDataa.Instance.adultSettingsDATA.SexGameplayAI;
            } else {
                PersonalityId = Attributes.enemyData?.statsDATA.EnemyPersonality ?? 0;

                if (SexChoiceRealismMod.Enabled.Value) {
                    if (SexChoiceRealismMod.RandomPersonalityElite.Value && (Attributes.isAreaBoss || Attributes.combatAI?.isElite == true))
                        PersonalityId = RandomUtils.Int32(1, 6);
                    else if (SexChoiceRealismMod.RandomPersonalityAlly.Value && Attributes.combatAI?.isAlly == true)
                        PersonalityId = RandomUtils.Int32(1, 6);
                    else if (SexChoiceRealismMod.RandomPersonalityEnemy.Value)
                        PersonalityId = RandomUtils.Int32(1, 6);
                }
            }

            _componentInitialized = true;
        }
    }
}
