using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/BestFit Outline")]
	public class BestFitOutline : Shadow
	{
		protected BestFitOutline()
		{
		}

		public override void ModifyMesh(Mesh mesh)
		{
			if (!this.IsActive())
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				vertexHelper.GetUIVertexStream(list);
			}
			Text component = base.GetComponent<Text>();
			float num = 1f;
			if (component && component.resizeTextForBestFit)
			{
				num = (float)component.cachedTextGenerator.fontSizeUsedForBestFit / (float)(component.resizeTextMaxSize - 1);
			}
			int start = 0;
			int count = list.Count;
			base.ApplyShadowZeroAlloc(list, base.effectColor, start, list.Count, base.effectDistance.x * num, base.effectDistance.y * num);
			start = count;
			int count2 = list.Count;
			base.ApplyShadowZeroAlloc(list, base.effectColor, start, list.Count, base.effectDistance.x * num, -base.effectDistance.y * num);
			start = count2;
			int count3 = list.Count;
			base.ApplyShadowZeroAlloc(list, base.effectColor, start, list.Count, -base.effectDistance.x * num, base.effectDistance.y * num);
			start = count3;
			int count4 = list.Count;
			base.ApplyShadowZeroAlloc(list, base.effectColor, start, list.Count, -base.effectDistance.x * num, -base.effectDistance.y * num);
			using (VertexHelper vertexHelper2 = new VertexHelper())
			{
				vertexHelper2.AddUIVertexTriangleStream(list);
				vertexHelper2.FillMesh(mesh);
			}
		}
	}
}
