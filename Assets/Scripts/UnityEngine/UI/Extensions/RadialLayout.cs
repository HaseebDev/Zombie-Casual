using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Radial Layout")]
	public class RadialLayout : LayoutGroup
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
			float num = (this.MaxAngle - this.MinAngle) / (float)base.transform.childCount;
			float num2 = this.StartAngle;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = (RectTransform)base.transform.GetChild(i);
				if (rectTransform != null)
				{
					this.m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotX | DrivenTransformProperties.PivotY);
					Vector3 a = new Vector3(Mathf.Cos(num2 * 0.0174532924f), Mathf.Sin(num2 * 0.0174532924f), 0f);
					rectTransform.localPosition = a * this.fDistance;
					RectTransform rectTransform2 = rectTransform;
					RectTransform rectTransform3 = rectTransform;
					RectTransform rectTransform4 = rectTransform;
					Vector2 vector = new Vector2(0.5f, 0.5f);
					rectTransform4.pivot = vector;
					rectTransform2.anchorMin = (rectTransform3.anchorMax = vector);
					num2 += num;
				}
			}
		}

		public float fDistance;

		[Range(0f, 360f)]
		public float MinAngle;

		[Range(0f, 360f)]
		public float MaxAngle;

		[Range(0f, 360f)]
		public float StartAngle;
	}
}
