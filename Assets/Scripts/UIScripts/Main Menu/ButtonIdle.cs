using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class ButtonIdle : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _earnPillText;
    [SerializeField] private LocalizedTMPTextParam _stageText;
    [SerializeField] private Slider _progressImg;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private GameObject _lock;
    
    private Button button;
    [HideInInspector] public bool IsUnlocked = false;

    private void Awake()
    {
        button = this.GetComponent<Button>();
    }

    public void Load()
    {
        var progressData = SaveManager.Instance.Data.GameData.IdleProgress;
        int currentLevel = progressData.CurrentLevel;
        _stageText.UpdateParams(progressData.CurrentLevel);
        _horizontalLayoutGroup.enabled = false;
        DOVirtual.DelayedCall(0.1f, () => { _horizontalLayoutGroup.enabled = true; });
    }

    public void SetUnlock(bool isUnlocked)
    {
        IsUnlocked = isUnlocked;
        _lock.SetActive(!isUnlocked);
    }
}