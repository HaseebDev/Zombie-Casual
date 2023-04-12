using System;
using System.Collections.Generic;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateManager : MonoBehaviour
{
	private void Start()
	{
		SceneManager.activeSceneChanged += this.SceneManager_ActiveSceneChanged;
	}

	private void SceneManager_ActiveSceneChanged(Scene arg0, Scene arg1)
	{
		this.OnSceneChanged = true;
		SceneManager.activeSceneChanged -= this.SceneManager_ActiveSceneChanged;
	}

	public void AddTo(object updateble)
	{
		if (updateble is ITick)
		{
			this.ticks.Add(updateble as ITick);
		}
		if (updateble is ITickFixed)
		{
			this.ticksFixes.Add(updateble as ITickFixed);
		}
		if (updateble is ITickLate)
		{
			this.ticksLate.Add(updateble as ITickLate);
		}
		if (updateble is ITickSec)
		{
			this.tickSecs.Add(updateble as ITickSec);
		}
	}

	public void RemoveFrom(object updateble)
	{
		if (updateble is ITick)
		{
			this.ticks.Remove(updateble as ITick);
		}
		if (updateble is ITickFixed)
		{
			this.ticksFixes.Remove(updateble as ITickFixed);
		}
		if (updateble is ITickLate)
		{
			this.ticksLate.Remove(updateble as ITickLate);
		}
		if (updateble is ITickSec)
		{
			this.tickSecs.Remove(updateble as ITickSec);
		}
	}

	private void Tick()
	{
		for (int i = 0; i < this.ticks.Count; i++)
		{
			this.ticks[i].Tick();
		}
		this.TickLate();
	}

	private void TickFixed()
	{
		for (int i = 0; i < this.ticksFixes.Count; i++)
		{
			this.ticksFixes[i].TickFixed();
		}
	}

	private void TickLate()
	{
		for (int i = 0; i < this.ticksLate.Count; i++)
		{
			this.ticksLate[i].TickLate();
		}
	}

	private void TickSec()
	{
		for (int i = 0; i < this.tickSecs.Count; i++)
		{
			this.tickSecs[i].TickSec();
		}
	}

	private void Update()
	{
		if (!this.OnSceneChanged)
		{
			if (this.sec > 0f)
			{
				this.sec -= Time.deltaTime;
			}
			else
			{
				this.sec = 1f;
				this.TickSec();
			}
			this.Tick();
		}
	}

	private void FixedUpdate()
	{
		if (!this.OnSceneChanged)
		{
			this.TickFixed();
		}
	}

	private void OnDestroy()
	{
		this.ticks.Clear();
		this.ticksLate.Clear();
		this.ticksFixes.Clear();
		this.tickSecs.Clear();
	}

	private List<ITick> ticks = new List<ITick>();

	private List<ITickFixed> ticksFixes = new List<ITickFixed>();

	private List<ITickLate> ticksLate = new List<ITickLate>();

	private List<ITickSec> tickSecs = new List<ITickSec>();

	private bool OnSceneChanged;

	private float sec = 1f;
}
