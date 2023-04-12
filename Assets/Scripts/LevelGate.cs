using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelGate : MonoBehaviour
{
    public void CheckAndUnlockLevelGate(int currentValue)
    {
        if (currentValue < this.minValue)
        {
            this.gateObj.SetActive(true);
            this.UpdateLevelGateMinLevelText(this.minValue);
            return;
        }
        if (this.gateObj != null)
            this.gateObj.SetActive(false);
    }

    private void UpdateLevelGateMinLevelText(int level)
    {
        this.minValueText.text = "LVL" + level.ToString();
    }

    public GameObject gateObj;

    [SerializeField]
    private int minValue;

    [SerializeField]
    private Text minValueText;
}
