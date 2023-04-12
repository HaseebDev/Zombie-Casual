using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Ez.Pooly;

public class AirDrop : MonoBehaviour, IClickableObject
{
    public Transform parachuteTrf;

    public virtual void Initialize(float TimeTravel = 0f)
    {
        transform.DOKill();
        if (parachuteTrf)
        {
            parachuteTrf.DOKill();
            parachuteTrf.transform.localScale = Vector3.one;
            parachuteTrf.gameObject.SetActiveIfNot(true);
        }

    }

    public void OnPointerDown()
    {

    }

    public virtual void OnPointerUp()
    {
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_EQUIP_ARMOUR);
        //handle show dialog airdrop here!!
        var progress = SaveManager.Instance.Data.GetPlayProgress(GameMaster.instance.currentMode);
        int maxLevel = progress.MaxLevel;

        var _airDropRewards = DesignHelper.GetListAirDropRewards(GamePlayController.instance.gameMode, maxLevel);
        if (_airDropRewards != null)
        {
            HUDAirDropData data = new HUDAirDropData();
            data._listRewardNormal = _airDropRewards.Item1;
            data._listRewardExtra = _airDropRewards.Item2;
            InGameCanvas.instance.ShowHUD(EnumHUD.HUD_AIR_DROP, false, null, data, false);
        }

        GamePlayController.instance.DestroyAirdrop(this.transform);


    }

    public virtual void SpawnAirDrop(Vector3 targetPos, float timeDrop = 2.0f)
    {
        parachuteTrf.DOKill();
        transform.DOKill();
        transform.position = targetPos + Vector3.up * 5f;
        transform.DOMove(targetPos, timeDrop).SetEase(Ease.InOutSine);
        parachuteTrf.localScale = Vector3.one;
        //parachuteTrf.DOScale(Vector3.zero, timeDrop * 0.2f).SetEase(Ease.Linear).SetDelay(timeDrop * 0.8f);
    }

}
