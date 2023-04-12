using System;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("UI/Effects/Extensions/Shining Effect")]
	public class ShineEffector : MonoBehaviour
	{
		public float YOffset
		{
			get
			{
				return this.yOffset;
			}
			set
			{
				this.ChangeVal(value);
				this.yOffset = value;
			}
		}

		private void OnEnable()
		{
			if (this.effector == null)
			{
				GameObject gameObject = new GameObject("effector");
				this.effectRoot = new GameObject("ShineEffect");
				this.effectRoot.transform.SetParent(base.transform);
				this.effectRoot.AddComponent<Image>().sprite = base.gameObject.GetComponent<Image>().sprite;
				this.effectRoot.GetComponent<Image>().type = base.gameObject.GetComponent<Image>().type;
				this.effectRoot.AddComponent<Mask>().showMaskGraphic = false;
				this.effectRoot.transform.localScale = Vector3.one;
				this.effectRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
				this.effectRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
				this.effectRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				this.effectRoot.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				this.effectRoot.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				this.effectRoot.transform.SetAsFirstSibling();
				gameObject.AddComponent<RectTransform>();
				gameObject.transform.SetParent(this.effectRoot.transform);
				this.effectorRect = gameObject.GetComponent<RectTransform>();
				this.effectorRect.localScale = Vector3.one;
				this.effectorRect.anchoredPosition3D = Vector3.zero;
				this.effectorRect.gameObject.AddComponent<ShineEffect>();
				this.effectorRect.anchorMax = Vector2.one;
				this.effectorRect.anchorMin = Vector2.zero;
				this.effectorRect.Rotate(0f, 0f, -8f);
				this.effector = gameObject.GetComponent<ShineEffect>();
				this.effectorRect.offsetMax = Vector2.zero;
				this.effectorRect.offsetMin = Vector2.zero;
				this.OnValidate();
			}
		}

		private void OnValidate()
		{
			this.effector.Yoffset = this.yOffset;
			this.effector.Width = this.width;
			if (this.yOffset <= -1f || this.yOffset >= 1f)
			{
				this.effectRoot.SetActive(false);
				return;
			}
			if (!this.effectRoot.activeSelf)
			{
				this.effectRoot.SetActive(true);
			}
		}

		private void ChangeVal(float value)
		{
			this.effector.Yoffset = value;
			if (value <= -1f || value >= 1f)
			{
				this.effectRoot.SetActive(false);
				return;
			}
			if (!this.effectRoot.activeSelf)
			{
				this.effectRoot.SetActive(true);
			}
		}

		private void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this.effectRoot);
				return;
			}
			UnityEngine.Object.Destroy(this.effectRoot);
		}

		public ShineEffect effector;

		[SerializeField]
		[HideInInspector]
		private GameObject effectRoot;

		[Range(-1f, 1f)]
		public float yOffset = -1f;

		[Range(0.1f, 1f)]
		public float width = 0.5f;

		private RectTransform effectorRect;
	}
}
