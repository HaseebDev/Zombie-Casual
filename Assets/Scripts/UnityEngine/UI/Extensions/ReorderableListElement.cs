using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	public class ReorderableListElement : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this._reorderableList == null)
			{
				return;
			}
			if (!this._reorderableList.IsDraggable)
			{
				this._draggingObject = null;
				return;
			}
			if (!this._reorderableList.CloneDraggedObject)
			{
				this._draggingObject = this._rect;
				this._fromIndex = this._rect.GetSiblingIndex();
				if (this._reorderableList.OnElementRemoved != null)
				{
					UnityEvent<ReorderableList.ReorderableListEventStruct> onElementRemoved = this._reorderableList.OnElementRemoved;
					ReorderableList.ReorderableListEventStruct arg = new ReorderableList.ReorderableListEventStruct
					{
						DroppedObject = this._draggingObject.gameObject,
						IsAClone = this._reorderableList.CloneDraggedObject,
						SourceObject = (this._reorderableList.CloneDraggedObject ? base.gameObject : this._draggingObject.gameObject),
						FromList = this._reorderableList,
						FromIndex = this._fromIndex
					};
					onElementRemoved.Invoke(arg);
				}
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
				this._draggingObject = gameObject.GetComponent<RectTransform>();
			}
			this._draggingObjectOriginalSize = base.gameObject.GetComponent<RectTransform>().rect.size;
			this._draggingObjectLE = this._draggingObject.GetComponent<LayoutElement>();
			this._draggingObject.SetParent(this._reorderableList.DraggableArea, false);
			this._draggingObject.SetAsLastSibling();
			this._fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
			this._fakeElementLE = this._fakeElement.gameObject.AddComponent<LayoutElement>();
			this.RefreshSizes();
			if (this._reorderableList.OnElementGrabbed != null)
			{
				UnityEvent<ReorderableList.ReorderableListEventStruct> onElementGrabbed = this._reorderableList.OnElementGrabbed;
				ReorderableList.ReorderableListEventStruct arg = new ReorderableList.ReorderableListEventStruct
				{
					DroppedObject = this._draggingObject.gameObject,
					IsAClone = this._reorderableList.CloneDraggedObject,
					SourceObject = (this._reorderableList.CloneDraggedObject ? base.gameObject : this._draggingObject.gameObject),
					FromList = this._reorderableList,
					FromIndex = this._fromIndex
				};
				onElementGrabbed.Invoke(arg);
			}
			this._isDragging = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!this._isDragging)
			{
				return;
			}
			this._draggingObject.position = eventData.position;
			EventSystem.current.RaycastAll(eventData, this._raycastResults);
			for (int i = 0; i < this._raycastResults.Count; i++)
			{
				this._currentReorderableListRaycasted = this._raycastResults[i].gameObject.GetComponent<ReorderableList>();
				if (this._currentReorderableListRaycasted != null)
				{
					break;
				}
			}
			if (this._currentReorderableListRaycasted == null || !this._currentReorderableListRaycasted.IsDropable)
			{
				this.RefreshSizes();
				this._fakeElement.transform.SetParent(this._reorderableList.DraggableArea, false);
				return;
			}
			if (this._fakeElement.parent != this._currentReorderableListRaycasted)
			{
				this._fakeElement.SetParent(this._currentReorderableListRaycasted.Content, false);
			}
			float num = float.PositiveInfinity;
			int siblingIndex = 0;
			float num2 = 0f;
			for (int j = 0; j < this._currentReorderableListRaycasted.Content.childCount; j++)
			{
				RectTransform component = this._currentReorderableListRaycasted.Content.GetChild(j).GetComponent<RectTransform>();
				if (this._currentReorderableListRaycasted.ContentLayout is VerticalLayoutGroup)
				{
					num2 = Mathf.Abs(component.position.y - eventData.position.y);
				}
				else if (this._currentReorderableListRaycasted.ContentLayout is HorizontalLayoutGroup)
				{
					num2 = Mathf.Abs(component.position.x - eventData.position.x);
				}
				else if (this._currentReorderableListRaycasted.ContentLayout is GridLayoutGroup)
				{
					num2 = Mathf.Abs(component.position.x - eventData.position.x) + Mathf.Abs(component.position.y - eventData.position.y);
				}
				if (num2 < num)
				{
					num = num2;
					siblingIndex = j;
				}
			}
			this.RefreshSizes();
			this._fakeElement.SetSiblingIndex(siblingIndex);
			this._fakeElement.gameObject.SetActive(true);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			this._isDragging = false;
			if (this._draggingObject != null)
			{
				if (this._currentReorderableListRaycasted != null && this._currentReorderableListRaycasted.IsDropable)
				{
					this.RefreshSizes();
					this._draggingObject.SetParent(this._currentReorderableListRaycasted.Content, false);
					this._draggingObject.SetSiblingIndex(this._fakeElement.GetSiblingIndex());
					if (this._reorderableList.OnElementDropped != null)
					{
						this._reorderableList.OnElementDropped.Invoke(new ReorderableList.ReorderableListEventStruct
						{
							DroppedObject = this._draggingObject.gameObject,
							IsAClone = this._reorderableList.CloneDraggedObject,
							SourceObject = (this._reorderableList.CloneDraggedObject ? base.gameObject : this._draggingObject.gameObject),
							FromList = this._reorderableList,
							FromIndex = this._fromIndex,
							ToList = this._currentReorderableListRaycasted,
							ToIndex = this._fakeElement.GetSiblingIndex() - 1
						});
					}
				}
				else if (this._reorderableList.CloneDraggedObject)
				{
					UnityEngine.Object.Destroy(this._draggingObject.gameObject);
				}
				else
				{
					this.RefreshSizes();
					this._draggingObject.SetParent(this._reorderableList.Content, false);
					this._draggingObject.SetSiblingIndex(this._fromIndex);
				}
			}
			if (this._fakeElement != null)
			{
				UnityEngine.Object.Destroy(this._fakeElement.gameObject);
			}
		}

		private void RefreshSizes()
		{
			Vector2 vector = this._draggingObjectOriginalSize;
			if (this._currentReorderableListRaycasted != null && this._currentReorderableListRaycasted.IsDropable && this._currentReorderableListRaycasted.Content.childCount > 0)
			{
				Transform child = this._currentReorderableListRaycasted.Content.GetChild(0);
				if (child != null)
				{
					vector = child.GetComponent<RectTransform>().rect.size;
				}
			}
			this._draggingObject.sizeDelta = vector;
			this._fakeElementLE.preferredHeight = (this._draggingObjectLE.preferredHeight = vector.y);
			this._fakeElementLE.preferredWidth = (this._draggingObjectLE.preferredWidth = vector.x);
		}

		public void Init(ReorderableList reorderableList)
		{
			this._reorderableList = reorderableList;
			this._rect = base.GetComponent<RectTransform>();
		}

		private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

		private ReorderableList _currentReorderableListRaycasted;

		private RectTransform _draggingObject;

		private LayoutElement _draggingObjectLE;

		private Vector2 _draggingObjectOriginalSize;

		private RectTransform _fakeElement;

		private LayoutElement _fakeElementLE;

		private int _fromIndex;

		private bool _isDragging;

		private RectTransform _rect;

		private ReorderableList _reorderableList;
	}
}
