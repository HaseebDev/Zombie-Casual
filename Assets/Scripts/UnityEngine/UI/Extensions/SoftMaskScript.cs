using System;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
	public class SoftMaskScript : MonoBehaviour
	{
		private void Start()
		{
			if (this.MaskArea == null)
			{
				this.MaskArea = base.GetComponent<RectTransform>();
			}
			Text component = base.GetComponent<Text>();
			if (component != null)
			{
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				component.material = this.mat;
				this.cachedCanvas = component.canvas;
				this.cachedCanvasTransform = this.cachedCanvas.transform;
				if (base.transform.parent.GetComponent<Mask>() == null)
				{
					base.transform.parent.gameObject.AddComponent<Mask>();
				}
				base.transform.parent.GetComponent<Mask>().enabled = false;
				return;
			}
			Graphic component2 = base.GetComponent<Graphic>();
			if (component2 != null)
			{
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				component2.material = this.mat;
				this.cachedCanvas = component2.canvas;
				this.cachedCanvasTransform = this.cachedCanvas.transform;
			}
		}

		private void Update()
		{
			if (this.cachedCanvas != null)
			{
				this.SetMask();
			}
		}

		private void SetMask()
		{
			Rect canvasRect = this.GetCanvasRect();
			Vector2 size = canvasRect.size;
			this.maskScale.Set(1f / size.x, 1f / size.y);
			this.maskOffset = -canvasRect.min;
			this.maskOffset.Scale(this.maskScale);
			this.mat.SetTextureOffset("_AlphaMask", this.maskOffset);
			this.mat.SetTextureScale("_AlphaMask", this.maskScale);
			this.mat.SetTexture("_AlphaMask", this.AlphaMask);
			this.mat.SetFloat("_HardBlend", (float)(this.HardBlend ? 1 : 0));
			this.mat.SetInt("_FlipAlphaMask", this.FlipAlphaMask ? 1 : 0);
			this.mat.SetInt("_NoOuterClip", this.DontClipMaskScalingRect ? 1 : 0);
			this.mat.SetFloat("_CutOff", this.CutOff);
		}

		public Rect GetCanvasRect()
		{
			if (this.cachedCanvas == null)
			{
				return default(Rect);
			}
			this.MaskArea.GetWorldCorners(this.m_WorldCorners);
			for (int i = 0; i < 4; i++)
			{
				this.m_CanvasCorners[i] = this.cachedCanvasTransform.InverseTransformPoint(this.m_WorldCorners[i]);
			}
			return new Rect(this.m_CanvasCorners[0].x, this.m_CanvasCorners[0].y, this.m_CanvasCorners[2].x - this.m_CanvasCorners[0].x, this.m_CanvasCorners[2].y - this.m_CanvasCorners[0].y);
		}

		private Material mat;

		private Canvas cachedCanvas;

		private Transform cachedCanvasTransform;

		private readonly Vector3[] m_WorldCorners = new Vector3[4];

		private readonly Vector3[] m_CanvasCorners = new Vector3[4];

		[Tooltip("The area that is to be used as the container.")]
		public RectTransform MaskArea;

		[Tooltip("Texture to be used to do the soft alpha")]
		public Texture AlphaMask;

		[Tooltip("At what point to apply the alpha min range 0-1")]
		[Range(0f, 1f)]
		public float CutOff;

		[Tooltip("Implement a hard blend based on the Cutoff")]
		public bool HardBlend;

		[Tooltip("Flip the masks alpha value")]
		public bool FlipAlphaMask;

		[Tooltip("If a different Mask Scaling Rect is given, and this value is true, the area around the mask will not be clipped")]
		public bool DontClipMaskScalingRect;

		private Vector2 maskOffset = Vector2.zero;

		private Vector2 maskScale = Vector2.one;
	}
}
