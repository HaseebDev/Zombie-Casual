using System;
using System.Linq;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDetector2D : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (this.ignoreUI && EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			this.mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			if (this.lastMousePos != this.mousePos)
			{
				RaycastHit2D[] array = (from h in Physics2D.RaycastAll(new Vector2(this.mousePos.x, this.mousePos.y), Vector2.zero, 0f, this.collisionLayerMask)
				orderby h.distance
				select h).ToArray<RaycastHit2D>();
				if (array.Length != 0 && array[array.Length - 1].collider != null)
				{
					IRayCastListiner component = array[array.Length - 1].collider.GetComponent<IRayCastListiner>();
					if (component != null)
					{
						component.RayCastClick();
						this.lastMousePos = this.mousePos;
					}
				}
			}
		}
	}

	private Vector2 mousePos;

	private Vector2 lastMousePos;

	[SerializeField]
	private LayerMask collisionLayerMask;

	[SerializeField]
	private bool ignoreUI;
}
