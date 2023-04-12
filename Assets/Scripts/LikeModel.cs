using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;

public class LikeModel : MonoBehaviour, IDisposable, ITickLate, ISave, ILoad
{
    public int LikeMultiplier {
        get {
            return this.likeMultiplier;
        }
    }

    public long LikesPerTap {
        get {
            return this.likesPerTap * (long)this.likeMultiplier * (long)this.bonusLikeMultiplier;
        }
    }

    public long LikesPerSec {
        get {
            return this.likesPerSec * (long)this.likeMultiplier * (long)this.bonusLikeMultiplier;
        }
    }

    public void IncraseLikesPerTap(int value)
    {
        this.likesPerTap += (long)value;
    }

    public void IncraseLikesPerSec(int value)
    {
        this.likesPerSec += (long)value;
    }

    public void IncraseMultipler()
    {
        this.likeMultiplier++;
    }

    public void Init()
    {
        this.Load();
    }

    public void Load()
    {
        this.likeMultiplier = 0;
        this.likesPerTap = 0L;
        this.likesPerSec = 0L;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("likemult", this.likeMultiplier);
        PlayerPrefs.SetString("likespertap", this.likesPerTap.ToString());
        PlayerPrefs.SetString("likespersec", this.likesPerSec.ToString());
    }

    public void TickLate()
    {
        this.OnLikesPerSecChanged = false;
        this.OnLikesPerTapChanged = false;
        this.OnStarsChanged = false;
        this.OnCharsHpChanged = false;
        this.OnCharsDeades = false;
    }

    public void Dispose()
    {
    }


    public bool OnLikesPerSecChanged;

    public bool OnLikesPerTapChanged;

    public bool OnStarsChanged;

    public bool OnCharsHpChanged;

    public bool OnCharsDeades;

    public bool OnExpChanged;

    public int bonusLikeMultiplier = 1;

    [SerializeField]
    private int likeMultiplier;

    [SerializeField]
    private long likesPerTap = 3L;

    [SerializeField]
    private long likesPerSec;
}
