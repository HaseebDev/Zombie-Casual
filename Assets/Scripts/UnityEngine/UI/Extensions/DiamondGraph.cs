using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
	public class DiamondGraph : UIPrimitiveBase
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			float num = base.rectTransform.rect.width / 2f;
			this.a = Math.Min(1f, Math.Max(0f, this.a));
			this.b = Math.Min(1f, Math.Max(0f, this.b));
			this.c = Math.Min(1f, Math.Max(0f, this.c));
			this.d = Math.Min(1f, Math.Max(0f, this.d));
			Color32 color = this.color;
			vh.AddVert(new Vector3(-num * this.a, 0f), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(0f, num * this.b), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(num * this.c, 0f), color, new Vector2(1f, 1f));
			vh.AddVert(new Vector3(0f, -num * this.d), color, new Vector2(1f, 0f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		public float a = 1f;

		public float b = 1f;

		public float c = 1f;

		public float d = 1f;
	}
}