using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using com.datld.data;
using Ez.Pooly;
using MEC;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class MiniGameController : BaseSystem<MiniGameController>
{
    public static MiniGameController instance;

    private void Awake()
    {
        instance = this;
    }

    public static float TIMER_CREATE_DURATION = 4.0f;
    public static string ZOM_DEMO = "ZOMBIE_DEMO";

    public Camera camera;

    [Header("Marker")]
    public List<Transform> _listHeroMarkers;
    public BoxCollider _zombieSpawnPoint;
    //public Transform _zombieTargetPoint;
    public Transform _rotatorTrf;

    [Header("Prefab")]
    public DemoZombie _zombieDemoPrefab;

    private float _timerZombie;
    private List<DemoZombie> _listZombies;
    private List<Character> _listCharacters;

    private DemoZombieData _zomData;
    private HUDHomeMenu _hudHomeMenu;
    private Transform _iconIdleChest;
    private bool isInited = false;

    private Coin3D coinPrefab;

    public bool PauseMiniGame { get; private set; }

    public void SetPauseMiniGame(bool pause)
    {
        PauseMiniGame = pause;
    }

    private bool EnableIdleChest = false;


    public override IEnumerator<float> InitializeCoroutineHandler()
    {
        if (isInited)
            yield break;

        _listZombies = new List<DemoZombie>();
        _listCharacters = new List<Character>();

        _zomData = new DemoZombieData()
        {
            HP = 1000,
            moveSpeed = 0.35f
        };

        AddressableManager.instance.LoadObjectAsync<Coin3D>(POOLY_PREF.COIN_3D, (coin) =>
        {
            coinPrefab = coin;
        });

        foreach (var point in _listHeroMarkers)
        {
            foreach (Transform child in point.transform)
            {
                Pooly.Despawn(child.transform);
            }

            bool waitTask = true;
            Character heroPref = null;
            Timing.RunCoroutine(SpawnCharacter("HERO_DEMO", false, point.transform, (hero) =>
            {
                waitTask = false;
                heroPref = hero;
            }));

            while (waitTask)
                yield return Timing.WaitForOneFrame;

            if (heroPref != null)
            {
                heroPref.parashuteFallBehavior.SetTargetY(point.position.y);
                heroPref.transform.localRotation = Quaternion.Euler(Vector3.zero);
                _listCharacters.Add(heroPref);

            }
        }

        _hudHomeMenu = (HUDHomeMenu)MainMenuCanvas.instance.GetHUD(EnumHUD.HUD_HOME);
        _iconIdleChest = _hudHomeMenu.btnIdleGold.transform;

        PauseMiniGame = false;

        // camera.targetTexture = _hudHomeMenu._minigameTexture;
        _hudHomeMenu.ResetMiniGame();

        camera.gameObject.SetActiveIfNot(true);
      
        isInited = true;

        OnInitializeComplete?.Invoke(true);
    }

    private IEnumerator<float> SpawnCharacter(string heroID, bool spawnHpBar, Transform parent, Action<Character> callback)
    {
        Character character = null;
        //Character heroPref = ResourceManager.instance.GetHeroPrefab(heroID);

        var op = Addressables.LoadAssetAsync<GameObject>(heroID);
        while (!op.IsDone)
        {
            yield return Timing.WaitForOneFrame;
        }
        if (op.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"cant loaded hero {heroID}");
            callback?.Invoke(null);
            yield break;
        }
        Character heroPref = op.Result.GetComponent<Character>();
        if (heroPref != null)
        {
            character = GameObject.Instantiate(heroPref);
            HeroData heroData = SaveManager.Instance.Data.GetHeroData(heroID);
            character.transform.SetParent(parent);
            character.transform.localPosition = Vector3.zero;
            character.transform.localRotation = Quaternion.Euler(Vector3.zero);
            character.gameObject.SetActiveIfNot(true);
            character.Initialize(heroData, true);

            if (spawnHpBar)
            {
                //PlayerHealth heroHealth = Pooly.Spawn<PlayerHealth>(POOLY_PREF.PLAYER_HEALTH, Vector3.zero,
                //    Quaternion.identity, character.transform);
                //if (heroHealth != null)
                //{
                //    var Power = heroData.GetPowerDataByGameMode();

                //    heroHealth.Initialize(Power.Hp);
                //    heroHealth.transform.localPosition = Vector3.zero;

                //    character.ApplyHealth(heroHealth);
                //}

                AddressableManager.instance.SpawnObjectAsync<PlayerHealth>(POOLY_PREF.PLAYER_HEALTH, (hp) =>
                {
                    PlayerHealth heroHealth = hp;
                    heroHealth.transform.SetParent(character.transform);
                    heroHealth.transform.localPosition = Vector3.zero;
                    heroHealth.transform.localRotation = Quaternion.identity;

                    if (heroHealth != null)
                    {
                        var Power = heroData.GetPowerDataByGameMode();
                        heroHealth.Initialize(Power.Hp);
                        heroHealth.transform.localPosition = Vector3.zero;

                        character.ApplyHealth(heroHealth);
                    }

                });

            }
        }

        callback?.Invoke(character);
    }

    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        UpdateSystem(_deltaTime);
    }

    public override void UpdateSystem(float _deltaTime)
    {
        if (!isInited || PauseMiniGame)
            return;

        base.UpdateSystem(_deltaTime);
        UpdateAutoCollectCoins(_deltaTime);
        _timerZombie += _deltaTime;
        if (_timerZombie >= TIMER_CREATE_DURATION)
        {
            var zom = CreateZombie();
            _listZombies.Add(zom);
            _timerZombie = 0f;
        }

        if (_listCharacters != null && _listCharacters.Count > 0)
        {
            for (int i = _listCharacters.Count - 1; i >= 0; i--)
            {
                _listCharacters[i].UpdateCharacter(_deltaTime);
            }
        }

        if (_listZombies != null && _listZombies.Count > 0)
        {
            for (int i = _listZombies.Count - 1; i >= 0; i--)
            {
                _listZombies[i].UpdateZombie(_deltaTime);
            }
        }
    }

    private DemoZombie CreateZombie()
    {
        var pos = _zombieSpawnPoint.bounds.RandomPointInBounds();
        pos.y = _zombieSpawnPoint.transform.position.y;

        DemoZombie component = Pooly.Spawn<DemoZombie>(_zombieDemoPrefab.transform, pos, Quaternion.identity, transform);

        var targetpos = _zombieSpawnPoint.transform.position;
        targetpos += transform.forward * UnityEngine.Random.Range(-2, 0);
        targetpos += transform.right * UnityEngine.Random.Range(-2, 2);
        component.Initialize(_zomData, targetpos);

        component.transform.localRotation = Quaternion.identity;
        component.transform.localScale = Vector3.one;
        component.ActionZombieDie = OnZombieDie;
        return component;

    }

    private void OnZombieDie(DemoZombie zom)
    {
        _listZombies.Remove(zom);
        Spawn3DCoins(GameMode.CAMPAIGN_MODE, zom.transform.position);
    }

    public void CleanUp()
    {
        for (int i = _listCharacters.Count - 1; i >= 0; i--)
        {
            _listCharacters[i].DestroyHero();
        }

        for (int i = _listZombies.Count - 1; i >= 0; i--)
        {
            _listZombies[i].DestroyZombie();
        }

        _listCharacters.Clear();
        _listZombies.Clear();
        _timerZombie = 0f;
        isInited = false;
    }


    #region 3D CoinSpawn

    public static float AUTO_COLLECT_INTERVAL = 5f;
    private List<Coin3D> _list3DCoins = new List<Coin3D>();
    private float timerCollectCoin = 0f;


    public void Spawn3DCoins(GameMode mode, Vector3 worldPos)
    {
        var coin3d = Pooly.Spawn<Coin3D>(coinPrefab.transform, worldPos + Vector3.up * 0.2f, Quaternion.identity, transform);
        coin3d.transform.localScale = Vector3.one;
        _list3DCoins.Add(coin3d);
    }

    private void UpdateAutoCollectCoins(float _deltaTime)
    {
        if (_list3DCoins.Count <= 0)
            return;

        timerCollectCoin += _deltaTime;
        if (timerCollectCoin >= AUTO_COLLECT_INTERVAL)
        {
            timerCollectCoin = 0f;
            for (int i = _list3DCoins.Count - 1; i >= 0; i--)
            {
                _list3DCoins[i].PlayAnimCollectCustom(camera, _iconIdleChest.position, () =>
                 {
                     _iconIdleChest.transform.DOKill();
                     _iconIdleChest.transform.localScale = Vector3.one;
                     _iconIdleChest.transform.DOScale(1.2f, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
                 }, 0.3f, Ease.OutSine, _hudHomeMenu.transform);
                Pooly.Despawn(_list3DCoins[i].transform);
                _list3DCoins.RemoveAt(i);

                if (i == 0)
                    AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_COLLECT_COIN);
            }
        }
    }

    public void CollectAllCoins()
    {
        timerCollectCoin = 0f;
        for (int i = _list3DCoins.Count - 1; i >= 0; i--)
        {
            _list3DCoins[i].PlayAnimCollectInGame(() =>
            {

            }, null);
            Pooly.Despawn(_list3DCoins[i].transform);
            _list3DCoins.RemoveAt(i);
        }

        _list3DCoins.Clear();
    }

    #endregion

}
