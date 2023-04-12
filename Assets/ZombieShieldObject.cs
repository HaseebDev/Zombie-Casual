using DG.Tweening;
using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieShieldObject : BaseZombieObject
{
    public void PlayAnimDeploy()
    {
        transform.DOKill();
        transform.gameObject.SetActiveIfNot(true);
        transform.SetParent(null);
        transform.DOLocalMoveY(1.2f, 0.2f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
            {
                _effectsDeploy[i].gameObject.SetActiveIfNot(true);
                _effectsDeploy[i].Play();
            }
        });
    }

    public override void OnDie(bool skipAnimDead)
    {
        var pos = this._health._hitMarker.transform.position;
        GameMaster.PlayEffect(COMMON_FX.FX_EXPLODE_METAL, pos, Quaternion.identity, null);
        base.OnDie(skipAnimDead);
    }
}
