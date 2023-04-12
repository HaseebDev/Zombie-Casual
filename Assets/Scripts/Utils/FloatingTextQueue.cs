using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct FloatingTextData
{
    public Vector3 ScreenPos;
    public string Content;
    public int Size;
    public int MoveHeight;
    public Color Color;
    public float Duration;
}


public class FloatingTextQueue
{
    public static int MAX_FLOATING_TEXT_PER_FRAME = 8;

    private Queue<FloatingTextData> _queueFloatingText;
    private short limitCounter = 0;
    private CoroutineHandle processHandler;

    public void Initialize()
    {
        _queueFloatingText = new Queue<FloatingTextData>();
        _dictFloatingDmgLookup = new Dictionary<float, string>();
        limitCounter = 0;
    }

    public void EnqueueProcess(FloatingTextData data)
    {
        _queueFloatingText.Enqueue(data);
    }

    public void StartLoop()
    {
        limitCounter = 0;
        processHandler = Timing.RunCoroutine(ProcessLoop());
    }

    public void StopLoop()
    {
        Timing.KillCoroutines(processHandler);
        _queueFloatingText.Clear();
        _dictFloatingDmgLookup.Clear();
    }

    IEnumerator<float> ProcessLoop()
    {
        while (true)
        {
            if (_queueFloatingText.Count > 0)
            {
                ++limitCounter;
                InGameCanvas.instance.ShowFloatingText(_queueFloatingText.Dequeue());
                if (limitCounter >= MAX_FLOATING_TEXT_PER_FRAME)
                {
                    yield return Timing.WaitForOneFrame;
                    if (GameMaster.IsSpeedUp)
                        yield return Timing.WaitForOneFrame;

                    limitCounter = 0;
                }
            }
            else
            {
                yield return Timing.WaitForOneFrame;
                if (GameMaster.IsSpeedUp)
                    yield return Timing.WaitForOneFrame;
                limitCounter = 0;
            }

        }
    }

    #region Lookup Dict
    private Dictionary<float, string> _dictFloatingDmgLookup;

    public string GetDmgText(float dmg)
    {
        string result = null;
        _dictFloatingDmgLookup.TryGetValue(dmg, out result);
        if (result == null)
        {
            result = $"{Mathf.Round(dmg * 1.0f)}";
            _dictFloatingDmgLookup.Add(dmg, result);
        }

        return result;
    }

    #endregion

    public void CleanUp()
    {
        _queueFloatingText.Clear();
    }
}
