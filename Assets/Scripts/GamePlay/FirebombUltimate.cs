using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebombUltimate : AddOnActiveFlameBox
{
    public override void ThrowBomb(Vector3 worldPos)
    {
        var grenade = Pooly.Spawn<BaseBomb>(POOLY_PREF.SINGLE_GRENADE, transform.position, Quaternion.identity, null);
        if (grenade != null)
        {
            grenade.transform.localScale = Vector3.one;
            grenade.PreInit(this.GetUltimateDmg(), _design.Radius, this._OwnerID);
            grenade.Launch(worldPos, () =>
            {
                grenade.ActiveBomb();
                SpawnFireZone(worldPos + Vector3.up * 0.2f, _design.Radius);
            });
        }

        base.PostSkill();

    }
}
