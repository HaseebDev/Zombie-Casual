using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using MEC;
using UnityEngine;

public class AudioSystem : BaseSystem<AudioSystem>, IAudioSystem
{
    public Dictionary<BGM_ENUM, BGMDef> _dictBGM { get; private set; }
    public Dictionary<SFX_ENUM, SFXDef> _dictSFX { get; private set; }

    public static AudioSystem instance;
    private SettingData _settingData;

    private Dictionary<SFX_ENUM, float> _dictDelayPlaySFX = new Dictionary<SFX_ENUM, float>();

    public BGM_ENUM currentBGM { get; private set; }

    private CoroutineHandle bgmPlayByDuration;
    
    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        currentBGM = BGM_ENUM.NONE;
        _dictSFX = new Dictionary<SFX_ENUM, SFXDef>();
        _dictBGM = new Dictionary<BGM_ENUM, BGMDef>();

        if (ResourceManager.instance._soundBank.ListBGM != null)
        {
            foreach (var bgm in ResourceManager.instance._soundBank.ListBGM)
            {
                if (!_dictBGM.ContainsKey(bgm._bgm))
                    _dictBGM.Add(bgm._bgm, bgm);
            }
        }


        if (ResourceManager.instance._soundBank.ListSFX != null)
        {
            foreach (var sfx in ResourceManager.instance._soundBank.ListSFX)
            {
                if (!_dictSFX.ContainsKey(sfx._sfx))
                    _dictSFX.Add(sfx._sfx, sfx);
            }
        }
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void InitState()
    {
        UpdSoundState();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            SaveManager.Instance.Data != null && SaveManager.Instance.Data.SettingData != null);
        _settingData = SaveManager.Instance.Data.SettingData;
        InitState();
    }

    public void Mute()
    {
        _settingData.IsSfxOn = false;
        _settingData.IsBgmOn = false;
        this.UpdSoundState();
    }

    public void Unmute()
    {
        this.UpdSoundState();
    }

    private void UpdSoundState()
    {
        this.bgMusicAudioSource.volume = _settingData.IsBgmOn ? 1 : 0;
        this.soundAudioSource.volume = _settingData.IsSfxOn ? 1 : 0;
    }

    public void PlayBgMusic(BGM_ENUM musicType)
    {
        if (bgmPlayByDuration != null)
            Timing.KillCoroutines(bgmPlayByDuration);
        
        if (musicType == BGM_ENUM.NONE)
            return;

        var bgm = this.GetBGM(musicType);
        if (bgm != null)
        {
            this.bgMusicAudioSource.volume = bgMusicAudioSource.volume != 0 ? bgm._volume : 0; // > 0 ? bgm._volume : 1;
            this.bgMusicAudioSource.clip = bgm._audio;
            bgMusicAudioSource.Play();
        }
        currentBGM = musicType;
    }

    public void PlayBgMusicWithDuration(BGM_ENUM musicType)
    {
        var bgm = this.GetBGM(musicType);
        var lastBGM = currentBGM;
        PlayBgMusic(musicType);
        bgmPlayByDuration = Timing.CallDelayed(bgm._playDuration, () =>
        {
            PlayBgMusic(lastBGM);
        });

    }

    public void PlaySFX(SFX_ENUM sfxType)
    {
        if (sfxType == SFX_ENUM.NONE)
            return;


        var sfx = this.GetSFX(sfxType);

        if (sfx._threshold > 0)
        {
            float timeNextPlay = -1;
            _dictDelayPlaySFX.TryGetValue(sfxType, out timeNextPlay);

            if (Time.time < timeNextPlay)
                return;

            if (!_dictDelayPlaySFX.ContainsKey(sfxType))
                _dictDelayPlaySFX.Add(sfxType, Time.time + sfx._threshold);
            else
                _dictDelayPlaySFX[sfxType] = Time.time + sfx._threshold;
        }

        if (sfx != null && sfx._audio != null)
        {
            float volume = soundAudioSource.volume != 0 ? sfx._volume : 0; // sfx._volume; // > 0 ? sfx._volume : 1;
            this.soundAudioSource.PlayOneShot(sfx._audio, volume);
        }
    }

    [SerializeField] private AudioSource bgMusicAudioSource;

    [SerializeField] private AudioSource soundAudioSource;

    //[SerializeField] private AudioClip bgMusic;
    //[SerializeField] private AudioClip bgMusicHome;

    //[SerializeField] private AudioClip sniperShoot;

    //[SerializeField] private AudioClip upgradge;

    //[SerializeField] private AudioClip aviaudar;

    //[SerializeField] private AudioClip aviaudar2;

    //[SerializeField] private AudioClip laser;

    //[SerializeField] private AudioClip shoot0;

    //[SerializeField] private AudioClip shoot1;

    //[SerializeField] private AudioClip shoot3;

    //[SerializeField] private AudioClip shoot4;

    //[SerializeField] private AudioClip shoot5;

    //[SerializeField] private AudioClip shoot6;

    public void SwitchSoundState(bool isOn)
    {
        _settingData.IsSfxOn = isOn;
        UpdSoundState();
    }

    public void SwitchBGMState(bool isOn)
    {
        _settingData.IsBgmOn = isOn;
        UpdSoundState();
    }

    #region Getter

    public BGMDef GetBGM(BGM_ENUM type)
    {
        BGMDef result = null;
        _dictBGM.TryGetValue(type, out result);
        return result;
    }

    public SFXDef GetSFX(SFX_ENUM sfx)
    {
        SFXDef result = null;
        _dictSFX.TryGetValue(sfx, out result);
        return result;
    }

    #endregion
}