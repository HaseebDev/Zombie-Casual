using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.talent;
using DG.Tweening;
using UnityEngine;
using MEC;
using UnityExtensions;

public class CastleHealth : Health
{
    public Transform _markerHPBar;

    public List<MeshRenderer> meshBarriers;

    private List<Color> _matInitColor;
    private List<Material> _matBarriers;

    public Color colorHit = Color.red;

    public override void Initialize(float MaxHp, bool _allowHeadshot = false, EffectController effectController = null, bool IsZombieBoss = false)
    {
        base.Initialize(MaxHp, _allowHeadshot, effectController);
        if (this.hpBar != null)
        {
            hpBar.EnableHPBar(false, false);
        }

        if (meshBarriers != null && meshBarriers.Count > 0)
        {
            _matInitColor = new List<Color>();
            _matBarriers = new List<Material>();
            foreach (var item in meshBarriers)
            {
                _matBarriers.Add(item.material);
                _matInitColor.Add(item.material.color);
            }
        }


        ResetMat();

        IsShowFloatingDmg = false;
    }

    public override void SetDamage(float _dmg, ShotType _type = ShotType.NORMAL,
        string casterId = null,
        List<EffectHit> effectHits = null, Action<bool, List<EffectArmour>> responseHit = null, float range = 0,
        Vector3 offsetScreen = default(Vector3), List<int> _listIgnoreEffectHits = null)
    {
        float reduceDamage = range > 5 ? ModelTalent.rangeBlock : ModelTalent.meleeBlock;
        if(reduceDamage > 0)
        {
            _dmg *= (1.0f - reduceDamage / 100f);
            Debug.Log($"Block Hit by talent! {reduceDamage}");
        }
     
        if (_dmg < 0)
            _dmg = 0;
        // Debug.LogError($"Range{range}, reduce damage{reduceDamage}");

        base.SetDamage(_dmg, _type, casterId, effectHits, responseHit, range, offsetScreen);
        GamePlayController.instance.campaignData.AttackBasement(_dmg);

        if (!hpBar.gameObject.activeInHierarchy)
            hpBar.EnableHPBar(true);

        hpBar.playAnimGetHit();
        if (meshBarriers != null && meshBarriers.Count > 0)
        {
            FlashColourWhenHit();
        }
    }

    public override void RefillHP(float amount, bool withAnim = true)
    {
        if (amount == 0)
            return;

        base.RefillHP(amount, withAnim);
        GamePlayController.instance.campaignData.UpdateData();
    }

    public override void ResetHealth()
    {
        base.ResetHealth();
        if (hpBar != null)
            hpBar.EnableHPBar(false, false);
        ResetMeshColor();
    }

    private bool flashAnimBusy = false;

    public void ResetMat()
    {
        for (int i = 0; i < this._matBarriers.Count; i++)
        {
            var mat = _matBarriers[i];
            mat.SetFloat("_Blend", 1f);
            mat.color = _matInitColor[i];
        }
    }

    public void FlashColourWhenHit()
    {
        if (flashAnimBusy || this._matBarriers == null || this._matBarriers.Count <= 0)
            return;

        flashAnimBusy = true;
        for (int i = 0; i < this._matBarriers.Count; i++)
        {
            Timing.RunCoroutine(FlashColourWhenHitCoroutine(i));
        }
    }

    IEnumerator<float> FlashColourWhenHitCoroutine(int index)
    {
        var mat = _matBarriers[index];
        mat.DOKill();
        mat.color = _matInitColor[index];
        mat.color = colorHit;
        //mat.SetFloat("_Blend", 0.2f);
        yield return Timing.WaitForSeconds(0.135f);
        mat.color = _matInitColor[index];
        //mat.SetFloat("_Blend", 1f);
        yield return Timing.WaitForSeconds(0.1f);
        flashAnimBusy = false;
    }

    public void ResetMeshColor()
    {
        if (this._matBarriers == null || this._matBarriers.Count <= 0)
            return;

        for (int i = 0; i < this._matBarriers.Count; i++)
        {
            var mat = _matBarriers[i];
            mat.DOKill();
            mat.color = _matInitColor[i];
        }
    }

    public void ResetHPBar()
    {
        if (this.hpBar != null && this.hpBar.GetType().Equals(typeof(HealthBarScreenUI)))
        {
            Vector3 newPosition = GamePlayController.instance.GetMainCamera().WorldToScreenPoint(_markerHPBar.position);
            newPosition.x = 0;
            this.hpBar.transform.position = newPosition;

            Vector3 rectPosition = this.hpBar.rectTransform().anchoredPosition;
            rectPosition.x = 0;
            this.hpBar.rectTransform().anchoredPosition = rectPosition - new Vector3(0,100f);
        }
    }
}