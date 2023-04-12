using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using Doozy.Engine.Extensions;
using MEC;
using QuickType;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HUDFusionResult : BaseHUD
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private ParticleSystem chargePs;
    [SerializeField] private ParticleSystem explodePs;
    [SerializeField] private ParticleSystem completePs;

    [SerializeField] private EquipmentUI resource1;
    [SerializeField] private EquipmentUI resource2;

    [SerializeField] private Image highlight;
    [SerializeField] private List<Transform> scaleAfterFusion;
    [SerializeField] private GameObject afterFusion;

    private Vector3 resource1OriginPos = Vector3.zero;
    private Vector3 resource2OriginPos = Vector3.zero;
    private WeaponData finalWeaponData;
    private WeaponData fakeFinal;
    private bool doingAnimation = false;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        _camera.SetActive(false);
        DOVirtual.DelayedCall(1, () => { _camera.SetActive(true); });

        if (args != null && args.Length > 0)

        {
            finalWeaponData = (WeaponData) args[0];
            fakeFinal = SaveGameHelper.defaultWeaponData(finalWeaponData.WeaponID, finalWeaponData.Rank - 1);
            fakeFinal.Level = finalWeaponData.GetWeaponLevel();
            
            MissionManager.Instance.TriggerMission(MissionType.FUSION_EQUIP);
            
            LoadPreFusion(fakeFinal, (WeaponData) args[1], (WeaponData) args[2]);
            //Load((WeaponData)args[0]);
        }

        if (GamePlayController.instance != null)
            GamePlayController.instance.SetPauseGameplay(true);
    }

    private void LoadPreFusion(WeaponData weaponData, WeaponData resource1Wp, WeaponData resource2Wp)
    {
        doingAnimation = true;
        afterFusion.SetActive(false);
        highlight.gameObject.SetActive(false);

        if (resource1OriginPos == Vector3.zero)
            resource1OriginPos = resource1.transform.position;
        if (resource2OriginPos == Vector3.zero)
            resource2OriginPos = resource2.transform.position;

        var wpDesign = DesignHelper.GetWeaponDesign(weaponData);

        resource1.Load(resource1Wp, DesignHelper.GetWeaponDesign(resource1Wp));
        resource2.Load(resource2Wp, DesignHelper.GetWeaponDesign(resource2Wp));
        _equipmentUi.Load(weaponData, wpDesign);

        Timing.RunCoroutine(AnimCoroutine());
    }

    private IEnumerator<float> AnimCoroutine()
    {
        var resource1Transform = resource1.transform;
        var resource2Transform = resource2.transform;

        resource1Transform.position = resource1OriginPos;
        resource2Transform.position = resource2OriginPos;
        resource1Transform.localScale = Vector3.one;
        resource2Transform.localScale = Vector3.one;

        yield return Timing.WaitForSeconds(0.5f);


        Shake(resource1Transform, 5, 7);
        Shake(resource2Transform, 5, 7);

        yield return Timing.WaitForSeconds(0.15f * 3);
        MoveToFinal(resource1Transform);
        MoveToFinal(resource2Transform);

        Shake(_equipmentUi.transform, 7, 7);
        _equipmentUi.transform.DOScale(0.8f, 7 * 0.15f).OnComplete(() =>
        {
            fakeFinal.Rank++;
            highlight.color = ResourceManager.instance.GetRankDefine(fakeFinal.Rank).color;
            highlight.gameObject.SetActive(true);

            var wpDesign = DesignHelper.GetWeaponDesign(fakeFinal);
            _equipmentUi.Load(fakeFinal, wpDesign);
        });

        chargePs.Play();

        yield return Timing.WaitForSeconds(1f);

        chargePs.Stop();
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(_equipmentUi.transform.DOScale(1.2f, 0.05f));
        scaleSequence.Append(_equipmentUi.transform.DOScale(1f, 0.05f));

        // yield return Timing.WaitForSeconds(0.05f);

        explodePs.Play();
        completePs.Play();

        yield return Timing.WaitForSeconds(0.5f);
        Load(finalWeaponData);

        afterFusion.SetActive(true);
        foreach (var VARIABLE in scaleAfterFusion)
        {
            VARIABLE.transform.localScale = Vector3.zero;
        }

        doingAnimation = false;

        foreach (var VARIABLE in scaleAfterFusion)
        {
            VARIABLE.transform.DOScale(1, 0.5f);
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    private void Shake(Transform target, int loop = 2, float shakePower = 3)
    {
        var _shakeSequence = DOTween.Sequence();
        Vector3 currentRotation = target.rotation.eulerAngles;

        _shakeSequence.Append(target.DORotate(new Vector3(currentRotation.x, currentRotation.y, -shakePower), 0.05f));
        _shakeSequence.Append(target.DORotate(new Vector3(currentRotation.x, currentRotation.y, shakePower), 0.05f));
        _shakeSequence.Append(target.DORotate(new Vector3(currentRotation.x, currentRotation.y, 0), 0.05f));
        _shakeSequence.SetLoops(loop);
    }

    private void MoveToFinal(Transform target)
    {
        target.transform.DOMove(_equipmentUi.transform.position, 0.5f);
        target.transform.DOScale(0.3f, 0.5f);
    }

    public override void OnButtonBack()
    {
        if (doingAnimation)
        {
            BackButtonManager.Instance.ShowCanGoBackText();
            return;
        }

        _camera.SetActive(false);
        base.OnButtonBack();

        if (GamePlayController.instance != null)
            GamePlayController.instance.SetPauseGameplay(false);
    }

    [SerializeField] private FusionResultAttributeUI _baseAttribute;
    [SerializeField] private FusionResultAttributeUI _maxLevel;
    [SerializeField] private FusionResultAttributeUI _newAttribute;

    [SerializeField] private AttributeUI _newAttributeAttributeUI;

    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private LocalizedTMPTextUI _nameText;

    private void Load(WeaponData weaponData)
    {
        var oldWpDesign = DesignHelper.GetWeaponDesign(weaponData.WeaponID, weaponData.Rank - 1, weaponData.GetWeaponLevel());
        var currentWpDesign = DesignHelper.GetWeaponDesign(weaponData);

        //_equipmentUi.Load(weaponData, currentWpDesign);
        var rankDefine = ResourceManager.instance.GetRankDefine(weaponData.Rank);
        _nameText.text = currentWpDesign.Name.AsLocalizeString();
        _nameText.targetTMPText.color = rankDefine.color;

        _maxLevel.Load(LOCALIZE_ID_PREF.MAX_LV.AsLocalizeString(), oldWpDesign.MaxLevel, currentWpDesign.MaxLevel);

        if (currentWpDesign.EquipType == 0) // Weapon
        {
            var currentPower = SaveGameHelper.AddPowerData(weaponData.DefaultPower,weaponData.UpgradedPower);
            var lastPower = weaponData.CalculatePowerDataFromRank(weaponData.Rank - 1);

            _baseAttribute.Load(
                DesignManager.instance._dictSkillDesign[EffectType.PASSIVE_INCREASE_PERCENT_DMG.ToString()],
                lastPower.PercentDmg,
                currentPower.PercentDmg);
        }
        else // Armour
        {
            _baseAttribute.Load(
                DesignManager.instance._dictSkillDesign[EffectType.PASSIVE_INCREASE_HP.ToString()],
                oldWpDesign.Hp,
                currentWpDesign.Hp);
        }

        _newAttribute.Load(LOCALIZE_ID_PREF.NEW_ATTRIBUTE.AsLocalizeString(), 0, 0);
        var lockedAttribute = oldWpDesign.GetNextLockedAttribute();
        if (lockedAttribute != null && currentWpDesign.Rarity == lockedAttribute.Item3)
        {
            _newAttribute.gameObject.SetActive(true);
            _newAttributeAttributeUI.Load(lockedAttribute.Item1, lockedAttribute.Item2);
        }
        else
        {
            _newAttribute.gameObject.SetActive(false);
        }
    }
}