using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
	[AddComponentMenu("UI/Effects/Extensions/Cylinder Text")]
	public class CylinderText : BaseMeshEffect
	{
		protected override void Awake()
		{
			base.Awake();
			this.rectTrans = base.GetComponent<RectTransform>();
			this.OnRectTransformDimensionsChange();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.rectTrans = base.GetComponent<RectTransform>();
			this.OnRectTransformDimensionsChange();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (!this.IsActive())
			{
				return;
			}
			int currentVertCount = vh.currentVertCount;
			if (!this.IsActive() || currentVertCount == 0)
			{
				return;
			}
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				UIVertex uivertex = default(UIVertex);
				vh.PopulateUIVertex(ref uivertex, i);
				float x = uivertex.position.x;
				uivertex.position.z = -this.radius * Mathf.Cos(x / this.radius);
				uivertex.position.x = this.radius * Mathf.Sin(x / this.radius);
				vh.SetUIVertex(uivertex, i);
			}
		}

		public float radius;

		private RectTransform rectTrans;
	}
}
