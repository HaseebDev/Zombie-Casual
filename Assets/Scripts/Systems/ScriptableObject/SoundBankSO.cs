using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

[Serializable]
public enum BGM_ENUM
{
    NONE,
    BGM_MENU,
    BGM_BATTLE_01,
    BGM_BATTLE_02,
    BGM_BATTLE_03,
    BGM_DEFEAT
}


[Serializable]
public enum SFX_ENUM
{
    //MENU
    NONE = 0,
    SFX_BUTTON,
    SFX_TAB,
    SFX_EQUIP_WEAPON,
    SFX_EQUIP_ARMOUR,
    SFX_SELECT,
    SFX_UPGRADE,
    SFX_DENIED,
    SFX_1STAR,
    SFX_2STAR,
    SFX_3STAR,
    SFX_WIN,
    SFX_DEFEAT,

    //WEAPON
    SFX_TAP_BATTLE = 200,
    SFX_PISTOL,
    SFX_RIFLE,
    SFX_SNIPER,
    SFX_SHOTGUN,
    SFX_ROCKET,
    SFX_MANUAL_BULLET,

    //BOMB
    SFX_BOMB_EXPLODE_01 = 300,
    SFX_BOMB_EXPLODE_02,
    SFX_BOMB_EXPLODE_03,
    SFX_BOMB_EXPLODE_04,
    SFX_BOMB_EXPLODE_05,


    //MONSTER
    SFX_ZOMBIE_DEAD_01 = 500,
    SFX_ZOMBIE_DEAD_02,
    SFX_BOSS_DEAD_01,
    SFX_BOSS_DEAD_02,
    SFX_ZOMBIE_HIT_01,
    SFX_ZOMBIE_HIT_02,
    SFX_BOSS_HIT_01,
    SFX_BOSS_HIT_02,
    SFX_CRITICAL,
    SFX_HEADSHOT,
    SFX_ZOMBIE_ATTACK_01,
    SFX_ZOMBIE_ATTACK_02,
    SFX_BOSS_ATTACK_01,
    SFX_BOSS_ATTACK_02,
    SFX_ZOMBIE_ATTACK_RANGE_01,
    SFX_ZOMBIE_ATTACK_RANGE_02,


    //SKILL & ULTIMATES
    SFX_CAST_SKILL_ACTIVE_BOMB = 700,
    SFX_CAST_SKILL_ZONE_BOMB,
    SFX_CAST_SKILL_STUN_BOMB,
    SFX_CAST_SKILL_PIERCE_BULLET,
    SFX_CAST_SKILL_MANUAL_HERO,
    SFX_CAST_SKILL_DARK_WING,
    SFX_CAST_SKILL_POISON_BOMB,
    SFX_CAST_SKILL_SONIC_WAVE,
    SFX_CAST_SKILL_FORKED_LIGHTNING,


    //ADD-ON
    SFX_CAST_AO_RADIO = 900,
    SFX_CAST_AO_AIRDROP,
    SFX_CAST_AO_SPAWNGATE,
    SFX_CAST_AO_MEDIC,
    SFX_CAST_AO_SPEED_UP,
    SFX_CAST_AO_FLAME_BOX,
    SFX_CAST_AO_BUFF_DEADSHOT,
    SFX_CAST_AO_AUTO_MANUAL,

    //OTHERS
    SFX_COLLECT_COIN = 1000,
    SFX_PARACHUTE_LANDING,
    SFX_COUNTING_STAR,
    SFX_CHEST_APPEAR,
    SFX_CHEST_OPEN

}

[Serializable]
public class BGMDef
{
    public BGM_ENUM _bgm;
    public AudioClip _audio;
    public float _volume = 1;
    public bool _loop = true;
    public float _playDuration = -1;
}

[Serializable]
public class SFXDef
{
    public SFX_ENUM _sfx;
    public AudioClip _audio;
    public float _volume;
    public float _threshold = 0f;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundBankSO", order = 1)]
public class SoundBankSO : ScriptableObject
{
    [Header("BGM")]
    public List<BGMDef> ListBGM;

    [Header("SFX")]
    public List<SFXDef> ListSFX;


    [Button("Load All Missing")]
    public void ReloadAll()
    {
        foreach (BGM_ENUM val in Enum.GetValues(typeof(BGM_ENUM)))
        {
            if (ListBGM.FirstOrDefault(x => x._bgm == val) == null)
            {
                ListBGM.Add(new BGMDef()
                {
                    _bgm = val,
                    _audio = null,
                    _volume = 1,
                    _loop = true
                });
            }
        }

        foreach (SFX_ENUM val in Enum.GetValues(typeof(SFX_ENUM)))
        {
            if (val == SFX_ENUM.NONE)
                continue;
            if (ListSFX.FirstOrDefault(x => x._sfx == val) == null)
            {
                ListSFX.Add(new SFXDef()
                {
                    _sfx = val,
                    _audio = null,
                    _volume = 1
                });
            }
        }

        ListSFX = ListSFX.OrderBy(x => (int)(x._sfx)).ToList();

    }

    [Button("Save Data")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    [Button("Check volume = 0")]
    public void CheckZeroVolume()
    {
        foreach (var m in ListBGM)
        {
            if (m._volume <= 0)
                m._volume = 1.0f;
        }

        foreach (var s in ListSFX)
        {
            if (s._volume <= 0)
                s._volume = 1.0f;
        }
    }
}
