using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;

public class StarsGroup : MonoBehaviour
{
    public StarView startViewPrefab;
    public int TotalStars;

    private List<StarView> _listStars;

    public void Initialize(int _totalStars)
    {
        TotalStars = _totalStars;
        if (_listStars != null && _listStars.Count > 0)
        {
            foreach (var s in _listStars)
            {
                Pooly.Despawn(s.transform);
            }
        }
        _listStars = new List<StarView>();

        for (int i = 0; i < TotalStars; i++)
        {
            var view = Pooly.Spawn<StarView>(startViewPrefab.transform, Vector3.zero, Quaternion.identity, transform);
            if (view != null)
            {
                view.transform.localScale = Vector3.one;
                view.EnableStar(false, false);
                _listStars.Add(view);
            }
        }
    }

    public void EnableStars(int numStar)
    {
        if (numStar >= _listStars.Count)
        {
            // Debug.LogError("numStar > totalStars!!!!");
            foreach (var s in _listStars)
            {
                s.EnableStar(true, false);
            }
        }
        else
        {
            for (int i = 0; i < _listStars.Count; i++)
            {
                _listStars[i].EnableStar(i < numStar);
            }
        }

    }
}
