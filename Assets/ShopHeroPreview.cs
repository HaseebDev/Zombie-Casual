using com.datld.data;
using System;
using System.Collections;
using System.Collections.Generic;
using QuickEngine.Extensions;
using UnityEngine;
using MEC;

public class ShopHeroPreview : MonoBehaviour
{
    public Transform markerHero;
    private Dictionary<string, HeroAnimMachine> _dictHeroAnim = new Dictionary<string, HeroAnimMachine>();

    private HeroAnimMachine curAnim = null;

    private List<string> _listTaskSpawnHeroAnim = new List<string>();
    public RenderTexture _renderTexture;
    public Camera _walkingCamera;
    
    public void ResetAnimPreview(HeroData data)
    {
        Timing.RunCoroutine(ResetAnimPreviewCoroutine(data));
    }

    public IEnumerator<float> ResetAnimPreviewCoroutine(HeroData data)
    {
        markerHero.gameObject.SetActive(false);
        HeroAnimMachine anim = null;
        bool waitTask = false;
        if (curAnim != null)
            curAnim.gameObject.SetActiveIfNot(false);

        _dictHeroAnim.TryGetValue(data.UniqueID, out anim);
        if (anim == null && !_listTaskSpawnHeroAnim.Contains(data.UniqueID))
        {
            waitTask = true;
            _listTaskSpawnHeroAnim.Add(data.UniqueID);
            ResourceManager.instance._heroResources.SpawnHeroAnim(data.UniqueID, (animCallback) =>
            {
                if (animCallback != null)
                {
                    anim = animCallback;
                    anim.transform.SetParent(markerHero);
                    anim.gameObject.SetActiveIfNot(false);
                    anim.transform.localPosition = Vector3.zero;
                    anim.transform.localScale = Vector3.one;
                    anim.transform.localRotation = Quaternion.identity;
                    if (!_dictHeroAnim.ContainsKey(data.UniqueID))
                        _dictHeroAnim.Add(data.UniqueID, anim);
                    _listTaskSpawnHeroAnim.Remove(data.UniqueID);

                    waitTask = false;
                }

            });

        }
        // else
            // yield break;

        while (waitTask)
        {
            yield return Timing.WaitForOneFrame;
        }
        

        if (anim == null)
            yield break;

        curAnim = anim;
        curAnim.gameObject.SetActiveIfNot(true);
        markerHero.gameObject.SetActive(true);

        if (!data.EquippedWeapon.IsNullOrEmpty())
        {
            var WeaponData = SaveManager.Instance.Data.GetWeaponData(data.EquippedWeapon);
            anim.gameObject.SetActive(true);
            anim.transform.localScale = Vector3.one;
            anim.Initialize(WeaponData.Type, WeaponData.WeaponID);
        }
    }

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_HERO_PREVIEW,
            new Action<HeroData>(ResetAnimPreview));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_HERO_PREVIEW,
            new Action<HeroData>(ResetAnimPreview));
    }

    public void ResetRenderTexture()
    {
        if (_renderTexture != null)
        {
            _walkingCamera.targetTexture = _renderTexture;
        }
        else
        {
            Debug.LogError("_renderTexture is null!!!");
        }
    }
}