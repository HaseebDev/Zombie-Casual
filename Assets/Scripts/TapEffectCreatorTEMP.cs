using System;
using UnityEngine;

public class TapEffectCreatorTEMP : MonoBehaviour
{
	private void Start()
	{
	}

	public void OnStartDrag()
	{
		this.startDragPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
	}

	public void OnTap()
	{
		this.poolObjectManager.CreatePoolObjectOnPos(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
	}

	public void OnDrag()
	{
		this.currentDragPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		if (Vector3.Distance(this.startDragPos, this.currentDragPos) > this.tapEffectInterval)
		{
			this.poolObjectManager.CreatePoolObjectOnPos(this.currentDragPos);
			this.tutorialController.t1TSprite.OnDrag(default(Vector3));
		}
	}

	private void Update()
	{
	}

	[SerializeField]
	private PoolObjectManager poolObjectManager;

	[SerializeField]
	private TutorialController tutorialController;

	private Vector3 startDragPos;

	private Vector3 currentDragPos;

	[SerializeField]
	private float tapEffectInterval = 1f;
}
