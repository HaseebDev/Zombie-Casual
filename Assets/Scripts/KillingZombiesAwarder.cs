using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;
using Ez.Pooly;
using System.Collections.Generic;
using com.datld.talent;
using DG.Tweening;
using MEC;

public class KillingZombiesAwarder : BaseSystem<KillingZombiesAwarder>
{
    private Coin3D coinPrefab;
    public void PreInit(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CAMPAIGN_MODE:
                AddressableManager.instance.LoadObjectAsync<Coin3D>(POOLY_PREF.COIN_3D, (coin3d) =>
                {
                    coinPrefab = coin3d;
                });
                break;
            case GameMode.IDLE_MODE:

                AddressableManager.instance.LoadObjectAsync<Coin3D>(POOLY_PREF.TOKEN_3D, (token) =>
                {
                    coinPrefab = token;
                });
                break;
            default:
                break;
        }

    }

    private void Start()
    {
        this.Inject();
        this.zmKillRewardKUData = UnityEngine.Object.FindObjectOfType<ZMKillRewardKUData>();
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ZOMBIE_KILLED,
            new Action<KilledZombieInfoStruct>(OnZombieKilled));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ZOMBIE_KILLED,
            new Action<KilledZombieInfoStruct>(OnZombieKilled));
    }

    private void OnZombieKilled(KilledZombieInfoStruct rwdData)
    {
        int num = Mathf.CeilToInt((float)rwdData.killReward);

        if (rwdData.gameMode == GameMode.CAMPAIGN_MODE)
            CurrencyModels.instance.Golds += (long)(num * (1 + ModelTalent.bonusGoldPercent));
        else
        {
            SaveManager.Instance.Data.GameData.IdleProgress.CollectedValue += (long)num;
            CurrencyModels.instance.Tokens += (long)num;
        }

        Spawn3DCoins(rwdData.gameMode, rwdData.worldPosition);

        GamePlayController.instance.currentLevelEarn?.AddValue((long)num);

        bool isAttacked = rwdData.isAttacked;
        bool isBoss = rwdData.isBoss;
        if (!isAttacked)
        {

            LevelModel.instance.AddExp(1L);
            return;
        }


        LevelModel.instance.DecExp(1L);
    }

    private ZMKillRewardKUData zmKillRewardKUData;

    public override void UpdateSystem(float _deltaTime)
    {
        base.UpdateSystem(_deltaTime);
        UpdateAutoCollectCoins(_deltaTime);
    }

    #region 3D CoinSpawn
    public static float MAX_COIN = 30;
    public static float AUTO_COLLECT_INTERVAL = 5f;
    private List<Coin3D> _list3DCoins = new List<Coin3D>();
    private float timerCollectCoin = 0f;
    private bool isCollectingCoin = false;

    public void Spawn3DCoins(GameMode mode, Vector3 worldPos)
    {
        if (_list3DCoins.Count >= MAX_COIN)
            return;
        //var objToSpawn = mode == GameMode.CAMPAIGN_MODE ? POOLY_PREF.COIN_3D : POOLY_PREF.TOKEN_3D;
        var coin3d = Pooly.Spawn<Coin3D>(coinPrefab.transform, worldPos + Vector3.up * 0.2f, Quaternion.identity, transform);
        coin3d.transform.localScale = Vector3.one;
        _list3DCoins.Add(coin3d);
    }

    private void UpdateAutoCollectCoins(float _deltaTime)
    {
        if (_list3DCoins == null || _list3DCoins.Count <= 0)
            return;

        timerCollectCoin += _deltaTime;
        if (timerCollectCoin >= AUTO_COLLECT_INTERVAL && !isCollectingCoin)
        {
            timerCollectCoin = 0f;
            Timing.RunCoroutine(CollectCoinsCoroutine());
        }
    }

    private bool preventSpamUpdate = false;

    IEnumerator<float> CollectCoinsCoroutine()
    {
        isCollectingCoin = true;
        preventSpamUpdate = false;

        for (int i = _list3DCoins.Count - 1; i >= 0; i--)
        {
            if (i < 0 || i >= _list3DCoins.Count)
                continue;

            _list3DCoins[i]?.PlayAnimCollectInGame(() =>
            {
                if (!preventSpamUpdate)
                {
                    preventSpamUpdate = true;
                    EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_GAME_HUD);
                    Timing.CallDelayed(0.5f, () =>
                    {
                        preventSpamUpdate = false;
                    });
                }


            }, () =>
            {
                AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_COLLECT_COIN);
            });

            if (_list3DCoins[i] != null)
            {
                Pooly.Despawn(_list3DCoins[i].transform);
            }
            _list3DCoins.RemoveAt(i);
            yield return Timing.WaitForOneFrame;
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_GAME_HUD);
        isCollectingCoin = false;
        yield break;


    }


    public void CollectAllCoins()
    {
        if (_list3DCoins == null || _list3DCoins.Count <= 0)
            return;
        timerCollectCoin = 0f;
        Timing.RunCoroutine(CollectCoinsCoroutine());
    }

    #endregion
}