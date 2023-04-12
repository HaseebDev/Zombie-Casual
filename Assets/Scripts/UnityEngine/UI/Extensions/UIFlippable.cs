using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform), typeof(Graphic))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Effects/Extensions/Flippable")]
	public class UIFlippable : MonoBehaviour, IMeshModifier
	{
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		public bool vertical
		{
			get
			{
				return this.m_Veritical;
			}
			set
			{
				this.m_Veritical = value;
			}
		}

		protected void OnValidate()
		{
			base.GetComponent<Graphic>().SetVerticesDirty();
		}

		public void ModifyMesh(VertexHelper verts)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			for (int i = 0; i < verts.currentVertCount; i++)
			{
				UIVertex uivertex = default(UIVertex);
				verts.PopulateUIVertex(ref uivertex, i);
				uivertex.position = new Vector3(this.m_Horizontal ? (uivertex.position.x + (rectTransform.rect.center.x - uivertex.position.x) * 2f) : uivertex.position.x, this.m_Veritical ? (uivertex.position.y + (rectTransform.rect.center.y - uivertex.position.y) * 2f) : uivertex.position.y, uivertex.position.z);
				verts.SetUIVertex(uivertex, i);
			}
		}

		public void ModifyMesh(Mesh mesh)
		{
		}

		[SerializeField]
		private bool m_Horizontal;

		[SerializeField]
		private bool m_Veritical;
	}
}
