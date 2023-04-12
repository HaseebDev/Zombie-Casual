using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient2")]
	public class Gradient2 : BaseMeshEffect
	{
		public Gradient2.Blend BlendMode
		{
			get
			{
				return this._blendMode;
			}
			set
			{
				this._blendMode = value;
			}
		}

		public Gradient EffectGradient
		{
			get
			{
				return this._effectGradient;
			}
			set
			{
				this._effectGradient = value;
			}
		}

		public Gradient2.Type GradientType
		{
			get
			{
				return this._gradientType;
			}
			set
			{
				this._gradientType = value;
			}
		}

		public float Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = value;
			}
		}

		public override void ModifyMesh(VertexHelper helper)
		{
			if (!this.IsActive() || helper.currentVertCount == 0)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			helper.GetUIVertexStream(list);
			int count = list.Count;
			Gradient2.Type gradientType = this.GradientType;
			if (gradientType == Gradient2.Type.Horizontal)
			{
				float num = list[0].position.x;
				float num2 = list[0].position.x;
				for (int i = count - 1; i >= 1; i--)
				{
					float x = list[i].position.x;
					if (x > num2)
					{
						num2 = x;
					}
					else if (x < num)
					{
						num = x;
					}
				}
				float num3 = 1f / (num2 - num);
				UIVertex uivertex = default(UIVertex);
				for (int j = 0; j < helper.currentVertCount; j++)
				{
					helper.PopulateUIVertex(ref uivertex, j);
					// uivertex.color = this.BlendColor(uivertex.color, this.EffectGradient.Evaluate((uivertex.position.x - num) * num3 - this.Offset));
					helper.SetUIVertex(uivertex, j);
				}
				return;
			}
			if (gradientType != Gradient2.Type.Vertical)
			{
				return;
			}
			float num4 = list[0].position.y;
			float num5 = list[0].position.y;
			for (int k = count - 1; k >= 1; k--)
			{
				float y = list[k].position.y;
				if (y > num5)
				{
					num5 = y;
				}
				else if (y < num4)
				{
					num4 = y;
				}
			}
			float num6 = 1f / (num5 - num4);
			UIVertex uivertex2 = default(UIVertex);
			for (int l = 0; l < helper.currentVertCount; l++)
			{
				helper.PopulateUIVertex(ref uivertex2, l);
				// uivertex2.color = this.BlendColor(uivertex2.color, this.EffectGradient.Evaluate((uivertex2.position.y - num4) * num6 - this.Offset));
				helper.SetUIVertex(uivertex2, l);
			}
		}

		private Color BlendColor(Color colorA, Color colorB)
		{
			Gradient2.Blend blendMode = this.BlendMode;
			if (blendMode == Gradient2.Blend.Add)
			{
				return colorA + colorB;
			}
			if (blendMode != Gradient2.Blend.Multiply)
			{
				return colorB;
			}
			return colorA * colorB;
		}

		[SerializeField]
		private Gradient2.Type _gradientType;

		[SerializeField]
		private Gradient2.Blend _blendMode = Gradient2.Blend.Multiply;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _offset;

		[SerializeField]
		private Gradient _effectGradient = new Gradient
		{
			/*colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.black, 0f),
				new GradientColorKey(Color.white, 1f)
			}*/
		};

		public enum Type
		{
			Horizontal,
			Vertical
		}

		public enum Blend
		{
			Override,
			Add,
			Multiply
		}
	}
}
