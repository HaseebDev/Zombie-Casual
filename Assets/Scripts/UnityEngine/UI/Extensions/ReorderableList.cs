using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Extensions/Re-orderable list")]
	public class ReorderableList : MonoBehaviour
	{
		public RectTransform Content
		{
			get
			{
				if (this._content == null)
				{
					this._content = this.ContentLayout.GetComponent<RectTransform>();
				}
				return this._content;
			}
		}

		private Canvas GetCanvas()
		{
			Transform transform = base.transform;
			Canvas canvas = null;
			int num = 100;
			int num2 = 0;
			while (canvas == null && num2 < num)
			{
				canvas = transform.gameObject.GetComponent<Canvas>();
				if (canvas == null)
				{
					transform = transform.parent;
				}
				num2++;
			}
			return canvas;
		}

		private void Awake()
		{
			if (this.ContentLayout == null)
			{
				UnityEngine.Debug.LogError("You need to have a child LayoutGroup content set for the list: " + base.name, base.gameObject);
				return;
			}
			if (this.DraggableArea == null)
			{
				this.DraggableArea = base.transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
			}
			if (this.IsDropable && !base.GetComponent<Graphic>())
			{
				UnityEngine.Debug.LogError("You need to have a Graphic control (such as an Image) for the list [" + base.name + "] to be droppable", base.gameObject);
				return;
			}
			if (this.GetCanvas().renderMode != RenderMode.ScreenSpaceOverlay)
			{
				UnityEngine.Debug.LogError("The ReOrderable List is only supported on a Screenspace-Overlay Canvas at the moment");
			}
			this._listContent = this.ContentLayout.gameObject.AddComponent<ReorderableListContent>();
			this._listContent.Init(this);
		}

		public void TestReOrderableListTarget(ReorderableList.ReorderableListEventStruct item)
		{
			UnityEngine.Debug.Log("Event Received");
			UnityEngine.Debug.Log("Hello World, is my item a clone? [" + item.IsAClone.ToString() + "]");
		}

		[Tooltip("Child container with re-orderable items in a layout group")]
		public LayoutGroup ContentLayout;

		[Tooltip("Parent area to draw the dragged element on top of containers. Defaults to the root Canvas")]
		public RectTransform DraggableArea;

		[Tooltip("Can items be dragged from the container?")]
		public bool IsDraggable = true;

		[Tooltip("Should the draggable components be removed or cloned?")]
		public bool CloneDraggedObject;

		[Tooltip("Can new draggable items be dropped in to the container?")]
		public bool IsDropable = true;

		[Header("UI Re-orderable Events")]
		public ReorderableList.ReorderableListHandler OnElementDropped = new ReorderableList.ReorderableListHandler();

		public ReorderableList.ReorderableListHandler OnElementGrabbed = new ReorderableList.ReorderableListHandler();

		public ReorderableList.ReorderableListHandler OnElementRemoved = new ReorderableList.ReorderableListHandler();

		private RectTransform _content;

		private ReorderableListContent _listContent;

		[Serializable]
		public struct ReorderableListEventStruct
		{
			public GameObject DroppedObject;

			public int FromIndex;

			public ReorderableList FromList;

			public bool IsAClone;

			public GameObject SourceObject;

			public int ToIndex;

			public ReorderableList ToList;
		}

		[Serializable]
		public class ReorderableListHandler : UnityEvent<ReorderableList.ReorderableListEventStruct>
		{
		}
	}
}
