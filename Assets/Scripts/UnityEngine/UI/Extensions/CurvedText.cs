using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
	[AddComponentMenu("UI/Effects/Extensions/Curved Text")]
	public class CurvedText : BaseMeshEffect
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
			int currentVertCount = vh.currentVertCount;
			if (!this.IsActive() || currentVertCount == 0)
			{
				return;
			}
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				UIVertex uivertex = default(UIVertex);
				vh.PopulateUIVertex(ref uivertex, i);
				uivertex.position.y = uivertex.position.y + this.curveForText.Evaluate(this.rectTrans.rect.width * this.rectTrans.pivot.x + uivertex.position.x) * this.curveMultiplier;
				vh.SetUIVertex(uivertex, i);
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			Keyframe key = this.curveForText[this.curveForText.length - 1];
			key.time = this.rectTrans.rect.width;
			this.curveForText.MoveKey(this.curveForText.length - 1, key);
		}

		public AnimationCurve curveForText = AnimationCurve.Linear(0f, 0f, 1f, 10f);

		public float curveMultiplier = 1f;

		private RectTransform rectTrans;
	}
}
