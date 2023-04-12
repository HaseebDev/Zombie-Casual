using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ez.Pooly;
using Spine;
using UnityEngine;

public class HomeLevelProgress : MonoBehaviour
{
    [SerializeField] private Color _passedColor;
    [SerializeField] private Color _currentColor;
    [SerializeField] private Color _notPassColor;

    private bool _isInit = false;
    private List<HomeLevelProgressItem> _listItem;

    public void Init()
    {
        _isInit = true;
    }

    public void CleanUp()
    {
        if (_listItem != null)
        {
            foreach (var VARIABLE in _listItem)
            {
               Pooly.Despawn(VARIABLE.transform); 
            }
        }
    }

    public void ResetLayer()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (!_isInit)
            {
                Init();
            }
            else
            {
                return;
            }

            transform.DestroyAllChild();
            int currentLevel = SaveGameHelper.GetMaxCampaignLevel();
            _listItem = new  List<HomeLevelProgressItem>();
        
            // normal 
            int startPhase = currentLevel / 10;

            if (currentLevel % 10 == 0)
                startPhase--;

            int startLevel = startPhase * 10 + 1;

            var nextCustomReward = DesignHelper.GetNearestNextCustomReward(currentLevel);
            if (startLevel <= 0)
                startLevel = 1;
            
            for (int i = startLevel; i < startLevel + 10; i++)
            {
                var item = Pooly.Spawn<HomeLevelProgressItem>(POOLY_PREF.LEVEL_PROGRESS_ITEM, Vector3.zero, Quaternion.identity, transform);// Instantiate(_prefab, transform);
                item.transform.localScale = Vector3.one;
                _listItem.Add(item);

                if (nextCustomReward != null)
                {
                    var hasCustomReward = nextCustomReward.LevelReward == i;
                    item.Load(i, GetStateColor(i, currentLevel), nextCustomReward, hasCustomReward);
                }
            }  
        });
    }

    private Color GetStateColor(int level, int currentLevel)
    {
        if (level < currentLevel)
            return _passedColor;
        if (level > currentLevel)
            return _notPassColor;
        return _currentColor;
    }
}