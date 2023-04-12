using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		private void Update()
		{
			this.thickness = Mathf.Clamp(this.thickness, 0f, base.rectTransform.rect.width / 2f);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float outer = -base.rectTransform.pivot.x * base.rectTransform.rect.width;
			float inner = -base.rectTransform.pivot.x * base.rectTransform.rect.width + this.thickness;
			vh.Clear();
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(0f, 1f);
			Vector2 vector3 = new Vector2(1f, 1f);
			Vector2 vector4 = new Vector2(1f, 0f);
			if (this.FixedToSegments)
			{
				float num = (float)this.fillPercent / 100f;
				float num2 = 360f / (float)this.segments;
				int num3 = (int)((float)(this.segments + 1) * num);
				for (int i = 0; i < num3; i++)
				{
					float f = 0.0174532924f * ((float)i * num2);
					float c = Mathf.Cos(f);
					float s = Mathf.Sin(f);
					vector = new Vector2(0f, 1f);
					vector2 = new Vector2(1f, 1f);
					vector3 = new Vector2(1f, 0f);
					vector4 = new Vector2(0f, 0f);
					Vector2 vector5;
					Vector2 vector6;
					Vector2 vector7;
					Vector2 vector8;
					this.StepThroughPointsAndFill(outer, inner, ref zero, ref zero2, out vector5, out vector6, out vector7, out vector8, c, s);
					vh.AddUIVertexQuad(base.SetVbo(new Vector2[]
					{
						vector5,
						vector6,
						vector7,
						vector8
					}, new Vector2[]
					{
						vector,
						vector2,
						vector3,
						vector4
					}));
				}
				return;
			}
			float width = base.rectTransform.rect.width;
			float height = base.rectTransform.rect.height;
			float num4 = (float)this.fillPercent / 100f * 6.28318548f / (float)this.segments;
			float num5 = 0f;
			for (int j = 0; j < this.segments + 1; j++)
			{
				float c2 = Mathf.Cos(num5);
				float s2 = Mathf.Sin(num5);
				Vector2 vector5;
				Vector2 vector6;
				Vector2 vector7;
				Vector2 vector8;
				this.StepThroughPointsAndFill(outer, inner, ref zero, ref zero2, out vector5, out vector6, out vector7, out vector8, c2, s2);
				vector = new Vector2(vector5.x / width + 0.5f, vector5.y / height + 0.5f);
				vector2 = new Vector2(vector6.x / width + 0.5f, vector6.y / height + 0.5f);
				vector3 = new Vector2(vector7.x / width + 0.5f, vector7.y / height + 0.5f);
				vector4 = new Vector2(vector8.x / width + 0.5f, vector8.y / height + 0.5f);
				vh.AddUIVertexQuad(base.SetVbo(new Vector2[]
				{
					vector5,
					vector6,
					vector7,
					vector8
				}, new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}));
				num5 += num4;
			}
		}

		private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY, out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
		{
			pos0 = prevX;
			pos1 = new Vector2(outer * c, outer * s);
			if (this.fill)
			{
				pos2 = Vector2.zero;
				pos3 = Vector2.zero;
			}
			else
			{
				pos2 = new Vector2(inner * c, inner * s);
				pos3 = prevY;
			}
			prevX = pos1;
			prevY = pos2;
		}

		[Tooltip("The circular fill percentage of the primitive, affected by FixedToSegments")]
		[Range(0f, 100f)]
		public int fillPercent = 100;

		[Tooltip("Should the primitive fill draw by segments or absolute percentage")]
		public bool FixedToSegments;

		[Tooltip("Draw the primitive filled or as a line")]
		public bool fill = true;

		[Tooltip("If not filled, the thickness of the primitive line")]
		public float thickness = 5f;

		[Tooltip("The number of segments to draw the primitive, more segments = smoother primitive")]
		[Range(0f, 360f)]
		public int segments = 360;
	}
}
