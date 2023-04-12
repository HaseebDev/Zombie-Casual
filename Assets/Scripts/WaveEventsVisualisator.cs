using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;

public class WaveEventsVisualisator : MonoBehaviour
{
    private void Start()
    {
        this.Inject();
        //this.eventManager.AddListener(EVENT_TYPE.WAVE_STARTED, this);
        //this.eventManager.AddListener(EVENT_TYPE.BOSS_WIN, this);
        //this.eventManager.AddListener(EVENT_TYPE.BOSS_LOOSE, this);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.WAVE_STARTED, new Action<WaveType>(OnWaveStarted));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_COMPLETE, new Action(OnLevelComplete));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_FAILED, new Action(OnLevelFailed));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.WAVE_STARTED, new Action<WaveType>(OnWaveStarted));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_COMPLETE, new Action(OnLevelComplete));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_FAILED, new Action(OnLevelFailed));
    }

    private void OnWaveStarted(WaveType waveType)
    {
        return;
        switch (waveType)
        {
            case WaveType.NORMAL_WAVE:
                this.normalWave.Show();
                break;
            case WaveType.BIG_WAVE:
                this.bigWave.Show();
                break;
            case WaveType.BOSS_WAVE:
                this.bossWave.Show();
                break;
        }
    }

    private void OnLevelComplete()
    {
        // this.levelComplete.Show();
    }

    private void OnLevelFailed()
    {
        // this.levelFailed.Show();
    }

    //   public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    //{
    //	if (eventType == EVENT_TYPE.WAVE_STARTED)
    //	{
    //		switch ((WaveType)Param)
    //		{
    //		case WaveType.NORMAL_WAVE:
    //			this.normalWave.Show();
    //			break;
    //		case WaveType.BIG_WAVE:
    //			this.bigWave.Show();
    //			break;
    //		case WaveType.BOSS_WAVE:
    //			this.bossWave.Show();
    //			break;
    //		}
    //	}
    //	if (eventType == EVENT_TYPE.BOSS_WIN)
    //	{
    //		this.bossWin.Show();
    //	}
    //	if (eventType == EVENT_TYPE.BOSS_LOOSE)
    //	{
    //		this.bossLoose.Show();
    //	}
    //}

    [Inject]
    private EventManager eventManager;

    [SerializeField]
    private WaveBeginUIVisualisator bigWave;

    [SerializeField]
    private WaveBeginUIVisualisator bossWave;

    [SerializeField]
    private WaveBeginUIVisualisator normalWave;

    [SerializeField]
    private WaveBeginUIVisualisator levelComplete;

    [SerializeField]
    private WaveBeginUIVisualisator levelFailed;
}
