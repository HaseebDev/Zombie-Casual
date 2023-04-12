using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Layout/Extensions/Tile Size Fitter")]
	public class TileSizeFitter : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		public Vector2 Border
		{
			get
			{
				return this.m_Border;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Vector2>(ref this.m_Border, value))
				{
					this.SetDirty();
				}
			}
		}

		public Vector2 TileSize
		{
			get
			{
				return this.m_TileSize;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Vector2>(ref this.m_TileSize, value))
				{
					this.SetDirty();
				}
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetDirty();
		}

		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			this.UpdateRect();
		}

		private void UpdateRect()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.m_Tracker.Clear();
			this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY);
			this.rectTransform.anchorMin = Vector2.zero;
			this.rectTransform.anchorMax = Vector2.one;
			this.rectTransform.anchoredPosition = Vector2.zero;
			this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDelta);
			Vector2 vector = this.GetParentSize() - this.Border;
			if (this.TileSize.x > 0.001f)
			{
				vector.x -= Mathf.Floor(vector.x / this.TileSize.x) * this.TileSize.x;
			}
			else
			{
				vector.x = 0f;
			}
			if (this.TileSize.y > 0.001f)
			{
				vector.y -= Mathf.Floor(vector.y / this.TileSize.y) * this.TileSize.y;
			}
			else
			{
				vector.y = 0f;
			}
			this.rectTransform.sizeDelta = -vector;
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = this.rectTransform.parent as RectTransform;
			if (!rectTransform)
			{
				return Vector2.zero;
			}
			return rectTransform.rect.size;
		}

		public virtual void SetLayoutHorizontal()
		{
		}

		public virtual void SetLayoutVertical()
		{
		}

		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateRect();
		}

		[SerializeField]
		private Vector2 m_Border = Vector2.zero;

		[SerializeField]
		private Vector2 m_TileSize = Vector2.zero;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;
	}
}
