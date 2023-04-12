using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Curved Layout")]
	public class CurvedLayout : LayoutGroup
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.CalculateRadial();
		}

		public override void SetLayoutHorizontal()
		{
		}

		public override void SetLayoutVertical()
		{
		}

		public override void CalculateLayoutInputVertical()
		{
			this.CalculateRadial();
		}

		public override void CalculateLayoutInputHorizontal()
		{
			this.CalculateRadial();
		}

		private void CalculateRadial()
		{
			this.m_Tracker.Clear();
			if (base.transform.childCount == 0)
			{
				return;
			}
			Vector2 pivot = new Vector2((float)((int)base.childAlignment % (int)TextAnchor.MiddleLeft) * 0.5f, (float)((int)base.childAlignment /(int) TextAnchor.MiddleLeft) * 0.5f);
			Vector3 a = new Vector3(base.GetStartOffset(0, base.GetTotalPreferredSize(0)), base.GetStartOffset(1, base.GetTotalPreferredSize(1)), 0f);
			float num = 0f;
			float num2 = 1f / (float)base.transform.childCount;
			Vector3 b = this.itemAxis.normalized * this.itemSize;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = (RectTransform)base.transform.GetChild(i);
				if (rectTransform != null)
				{
					this.m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotX | DrivenTransformProperties.PivotY);
					Vector3 a2 = a + b;
					a = (rectTransform.localPosition = a2 + (num - this.centerpoint) * this.CurveOffset);
					rectTransform.pivot = pivot;
					RectTransform rectTransform2 = rectTransform;
					RectTransform rectTransform3 = rectTransform;
					Vector2 vector = new Vector2(0.5f, 0.5f);
					rectTransform3.anchorMax = vector;
					rectTransform2.anchorMin = vector;
					num += num2;
				}
			}
		}

		public Vector3 CurveOffset;

		[Tooltip("axis along which to place the items, Normalized before use")]
		public Vector3 itemAxis;

		[Tooltip("size of each item along the Normalized axis")]
		public float itemSize;

		public float centerpoint = 0.5f;
	}
}
