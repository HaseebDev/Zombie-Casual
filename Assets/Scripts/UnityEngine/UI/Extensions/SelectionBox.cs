using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Canvas))]
	[AddComponentMenu("UI/Extensions/Selection Box")]
	public class SelectionBox : MonoBehaviour
	{
		private void ValidateCanvas()
		{
			if (base.gameObject.GetComponent<Canvas>().renderMode != RenderMode.ScreenSpaceOverlay)
			{
				throw new Exception("SelectionBox component must be placed on a canvas in Screen Space Overlay mode.");
			}
			CanvasScaler component = base.gameObject.GetComponent<CanvasScaler>();
			if (component && component.enabled && (!Mathf.Approximately(component.scaleFactor, 1f) || component.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize))
			{
				UnityEngine.Object.Destroy(component);
				UnityEngine.Debug.LogWarning("SelectionBox component is on a gameObject with a Canvas Scaler component. As of now, Canvas Scalers without the default settings throw off the coordinates of the selection box. Canvas Scaler has been removed.");
			}
		}

		private void SetSelectableGroup(IEnumerable<MonoBehaviour> behaviourCollection)
		{
			if (behaviourCollection == null)
			{
				this.selectableGroup = null;
				return;
			}
			List<MonoBehaviour> list = new List<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in behaviourCollection)
			{
				if (monoBehaviour is IBoxSelectable)
				{
					list.Add(monoBehaviour);
				}
			}
			this.selectableGroup = list.ToArray();
		}

		private void CreateBoxRect()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Selection Box";
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<Image>();
			this.boxRect = (gameObject.transform as RectTransform);
		}

		private void ResetBoxRect()
		{
			Image component = this.boxRect.GetComponent<Image>();
			component.color = this.color;
			component.sprite = this.art;
			this.origin = Vector2.zero;
			this.boxRect.anchoredPosition = Vector2.zero;
			this.boxRect.sizeDelta = Vector2.zero;
			this.boxRect.anchorMax = Vector2.zero;
			this.boxRect.anchorMin = Vector2.zero;
			this.boxRect.pivot = Vector2.zero;
			this.boxRect.gameObject.SetActive(false);
		}

		private void BeginSelection()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			this.boxRect.gameObject.SetActive(true);
			this.origin = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
			if (!this.PointIsValidAgainstSelectionMask(this.origin))
			{
				this.ResetBoxRect();
				return;
			}
			this.boxRect.anchoredPosition = this.origin;
			MonoBehaviour[] array;
			if (this.selectableGroup == null)
			{
				array = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
			}
			else
			{
				array = this.selectableGroup;
			}
			List<IBoxSelectable> list = new List<IBoxSelectable>();
			MonoBehaviour[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IBoxSelectable boxSelectable = array2[i] as IBoxSelectable;
				if (boxSelectable != null)
				{
					list.Add(boxSelectable);
					if (!Input.GetKey(KeyCode.LeftShift))
					{
						boxSelectable.selected = false;
					}
				}
			}
			this.selectables = list.ToArray();
			this.clickedBeforeDrag = this.GetSelectableAtMousePosition();
		}

		private bool PointIsValidAgainstSelectionMask(Vector2 screenPoint)
		{
			if (!this.selectionMask)
			{
				return true;
			}
			Camera screenPointCamera = this.GetScreenPointCamera(this.selectionMask);
			return RectTransformUtility.RectangleContainsScreenPoint(this.selectionMask, screenPoint, screenPointCamera);
		}

		private IBoxSelectable GetSelectableAtMousePosition()
		{
			if (!this.PointIsValidAgainstSelectionMask(UnityEngine.Input.mousePosition))
			{
				return null;
			}
			foreach (IBoxSelectable boxSelectable in this.selectables)
			{
				RectTransform rectTransform = boxSelectable.transform as RectTransform;
				if (rectTransform)
				{
					Camera screenPointCamera = this.GetScreenPointCamera(rectTransform);
					if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, UnityEngine.Input.mousePosition, screenPointCamera))
					{
						return boxSelectable;
					}
				}
				else
				{
					float magnitude = boxSelectable.transform.GetComponent<Renderer>().bounds.extents.magnitude;
					if (Vector2.Distance(this.GetScreenPointOfSelectable(boxSelectable), UnityEngine.Input.mousePosition) <= magnitude)
					{
						return boxSelectable;
					}
				}
			}
			return null;
		}

		private void DragSelection()
		{
			if (!Input.GetMouseButton(0) || !this.boxRect.gameObject.activeSelf)
			{
				return;
			}
			Vector2 vector = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
			Vector2 vector2 = vector - this.origin;
			Vector2 anchoredPosition = this.origin;
			if (vector2.x < 0f)
			{
				anchoredPosition.x = vector.x;
				vector2.x = -vector2.x;
			}
			if (vector2.y < 0f)
			{
				anchoredPosition.y = vector.y;
				vector2.y = -vector2.y;
			}
			this.boxRect.anchoredPosition = anchoredPosition;
			this.boxRect.sizeDelta = vector2;
			foreach (IBoxSelectable boxSelectable in this.selectables)
			{
				Vector3 v = this.GetScreenPointOfSelectable(boxSelectable);
				boxSelectable.preSelected = (RectTransformUtility.RectangleContainsScreenPoint(this.boxRect, v, null) && this.PointIsValidAgainstSelectionMask(v));
			}
			if (this.clickedBeforeDrag != null)
			{
				this.clickedBeforeDrag.preSelected = true;
			}
		}

		private void ApplySingleClickDeselection()
		{
			if (this.clickedBeforeDrag == null)
			{
				return;
			}
			if (this.clickedAfterDrag != null && this.clickedBeforeDrag.selected && this.clickedBeforeDrag.transform == this.clickedAfterDrag.transform)
			{
				this.clickedBeforeDrag.selected = false;
				this.clickedBeforeDrag.preSelected = false;
			}
		}

		private void ApplyPreSelections()
		{
			foreach (IBoxSelectable boxSelectable in this.selectables)
			{
				if (boxSelectable.preSelected)
				{
					boxSelectable.selected = true;
					boxSelectable.preSelected = false;
				}
			}
		}

		private Vector2 GetScreenPointOfSelectable(IBoxSelectable selectable)
		{
			RectTransform rectTransform = selectable.transform as RectTransform;
			if (rectTransform)
			{
				return RectTransformUtility.WorldToScreenPoint(this.GetScreenPointCamera(rectTransform), selectable.transform.position);
			}
			return Camera.main.WorldToScreenPoint(selectable.transform.position);
		}

		private Camera GetScreenPointCamera(RectTransform rectTransform)
		{
			RectTransform rectTransform2 = rectTransform;
			Canvas canvas;
			do
			{
				canvas = rectTransform2.GetComponent<Canvas>();
				if (canvas && !canvas.isRootCanvas)
				{
					canvas = null;
				}
				rectTransform2 = (RectTransform)rectTransform2.parent;
			}
			while (canvas == null);
			switch (canvas.renderMode)
			{
			case RenderMode.ScreenSpaceOverlay:
				return null;
			case RenderMode.ScreenSpaceCamera:
				if (!canvas.worldCamera)
				{
					return Camera.main;
				}
				return canvas.worldCamera;
			}
			return Camera.main;
		}

		public IBoxSelectable[] GetAllSelected()
		{
			if (this.selectables == null)
			{
				return new IBoxSelectable[0];
			}
			List<IBoxSelectable> list = new List<IBoxSelectable>();
			foreach (IBoxSelectable boxSelectable in this.selectables)
			{
				if (boxSelectable.selected)
				{
					list.Add(boxSelectable);
				}
			}
			return list.ToArray();
		}

		private void EndSelection()
		{
			if (!Input.GetMouseButtonUp(0) || !this.boxRect.gameObject.activeSelf)
			{
				return;
			}
			this.clickedAfterDrag = this.GetSelectableAtMousePosition();
			this.ApplySingleClickDeselection();
			this.ApplyPreSelections();
			this.ResetBoxRect();
			this.onSelectionChange.Invoke(this.GetAllSelected());
		}

		private void Start()
		{
			this.ValidateCanvas();
			this.CreateBoxRect();
			this.ResetBoxRect();
		}

		private void Update()
		{
			this.BeginSelection();
			this.DragSelection();
			this.EndSelection();
		}

		public Color color;

		public Sprite art;

		private Vector2 origin;

		public RectTransform selectionMask;

		private RectTransform boxRect;

		private IBoxSelectable[] selectables;

		private MonoBehaviour[] selectableGroup;

		private IBoxSelectable clickedBeforeDrag;

		private IBoxSelectable clickedAfterDrag;

		public SelectionBox.SelectionEvent onSelectionChange = new SelectionBox.SelectionEvent();

		public class SelectionEvent : UnityEvent<IBoxSelectable[]>
		{
		}
	}
}
