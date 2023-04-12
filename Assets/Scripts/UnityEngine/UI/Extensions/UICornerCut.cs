using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			Rect rect2 = rect;
			Color32 color = this.color;
			bool flag = this.cutUL | this.cutUR;
			bool flag2 = this.cutLL | this.cutLR;
			bool flag3 = this.cutLL | this.cutUL;
			bool flag4 = this.cutLR | this.cutUR;
			if ((flag || flag2) && this.cornerSize.sqrMagnitude > 0f)
			{
				vh.Clear();
				if (flag3)
				{
					rect2.xMin += this.cornerSize.x;
				}
				if (flag2)
				{
					rect2.yMin += this.cornerSize.y;
				}
				if (flag)
				{
					rect2.yMax -= this.cornerSize.y;
				}
				if (flag4)
				{
					rect2.xMax -= this.cornerSize.x;
				}
				if (this.makeColumns)
				{
					Vector2 vector = new Vector2(rect.xMin, this.cutUL ? rect2.yMax : rect.yMax);
					Vector2 vector2 = new Vector2(rect.xMax, this.cutUR ? rect2.yMax : rect.yMax);
					Vector2 vector3 = new Vector2(rect.xMin, this.cutLL ? rect2.yMin : rect.yMin);
					Vector2 vector4 = new Vector2(rect.xMax, this.cutLR ? rect2.yMin : rect.yMin);
					if (flag3)
					{
						UICornerCut.AddSquare(vector3, vector, new Vector2(rect2.xMin, rect.yMax), new Vector2(rect2.xMin, rect.yMin), rect, this.useColorUp ? this.colorUp : color, vh);
					}
					if (flag4)
					{
						UICornerCut.AddSquare(vector2, vector4, new Vector2(rect2.xMax, rect.yMin), new Vector2(rect2.xMax, rect.yMax), rect, this.useColorDown ? this.colorDown : color, vh);
					}
				}
				else
				{
					Vector2 vector = new Vector2(this.cutUL ? rect2.xMin : rect.xMin, rect.yMax);
					Vector2 vector2 = new Vector2(this.cutUR ? rect2.xMax : rect.xMax, rect.yMax);
					Vector2 vector3 = new Vector2(this.cutLL ? rect2.xMin : rect.xMin, rect.yMin);
					Vector2 vector4 = new Vector2(this.cutLR ? rect2.xMax : rect.xMax, rect.yMin);
					if (flag2)
					{
						UICornerCut.AddSquare(vector4, vector3, new Vector2(rect.xMin, rect2.yMin), new Vector2(rect.xMax, rect2.yMin), rect, this.useColorDown ? this.colorDown : color, vh);
					}
					if (flag)
					{
						UICornerCut.AddSquare(vector, vector2, new Vector2(rect.xMax, rect2.yMax), new Vector2(rect.xMin, rect2.yMax), rect, this.useColorUp ? this.colorUp : color, vh);
					}
				}
				if (this.makeColumns)
				{
					UICornerCut.AddSquare(new Rect(rect2.xMin, rect.yMin, rect2.width, rect.height), rect, color, vh);
					return;
				}
				UICornerCut.AddSquare(new Rect(rect.xMin, rect2.yMin, rect.width, rect2.height), rect, color, vh);
			}
		}

		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(a.x, a.y, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(b.x, b.y, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(c.x, c.y, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			Vector2 uv = new Vector2(Mathf.InverseLerp(area.xMin, area.xMax, x), Mathf.InverseLerp(area.yMin, area.yMax, y));
			vh.AddVert(new Vector3(x, y), color32, uv);
			return vh.currentVertCount - 1;
		}

		public Vector2 cornerSize = new Vector2(16f, 16f);

		[Header("Corners to cut")]
		public bool cutUL = true;

		public bool cutUR;

		public bool cutLL;

		public bool cutLR;

		[Tooltip("Up-Down colors become Left-Right colors")]
		public bool makeColumns;

		[Header("Color the cut bars differently")]
		public bool useColorUp;

		public Color32 colorUp = Color.blue;

		public bool useColorDown;

		public Color32 colorDown = Color.green;
	}
}
