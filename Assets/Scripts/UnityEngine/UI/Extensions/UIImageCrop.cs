using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/UIImageCrop")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class UIImageCrop : MonoBehaviour
	{
		private void Start()
		{
			this.SetMaterial();
		}

		public void SetMaterial()
		{
			this.mGraphic = base.GetComponent<MaskableGraphic>();
			this.XCropProperty = Shader.PropertyToID("_XCrop");
			this.YCropProperty = Shader.PropertyToID("_YCrop");
			if (this.mGraphic != null)
			{
				if (this.mGraphic.material == null || this.mGraphic.material.name == "Default UI Material")
				{
					this.mGraphic.material = new Material(Shader.Find("UI Extensions/UI Image Crop"));
				}
				this.mat = this.mGraphic.material;
				return;
			}
			UnityEngine.Debug.LogError("Please attach component to a Graphical UI component");
		}

		public void OnValidate()
		{
			this.SetMaterial();
			this.SetXCrop(this.XCrop);
			this.SetYCrop(this.YCrop);
		}

		public void SetXCrop(float xcrop)
		{
			this.XCrop = Mathf.Clamp01(xcrop);
			this.mat.SetFloat(this.XCropProperty, this.XCrop);
		}

		public void SetYCrop(float ycrop)
		{
			this.YCrop = Mathf.Clamp01(ycrop);
			this.mat.SetFloat(this.YCropProperty, this.YCrop);
		}

		private MaskableGraphic mGraphic;

		private Material mat;

		private int XCropProperty;

		private int YCropProperty;

		public float XCrop;

		public float YCrop;
	}
}
