using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/BoxSlider")]
	public class BoxSlider : Selectable, IDragHandler, IEventSystemHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		public RectTransform HandleRect
		{
			get
			{
				return this.m_HandleRect;
			}
			set
			{
				if (BoxSlider.SetClass<RectTransform>(ref this.m_HandleRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		public float MinValue
		{
			get
			{
				return this.m_MinValue;
			}
			set
			{
				if (BoxSlider.SetStruct<float>(ref this.m_MinValue, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		public float MaxValue
		{
			get
			{
				return this.m_MaxValue;
			}
			set
			{
				if (BoxSlider.SetStruct<float>(ref this.m_MaxValue, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		public bool WholeNumbers
		{
			get
			{
				return this.m_WholeNumbers;
			}
			set
			{
				if (BoxSlider.SetStruct<bool>(ref this.m_WholeNumbers, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		public float ValueX
		{
			get
			{
				if (this.WholeNumbers)
				{
					return Mathf.Round(this.m_ValueX);
				}
				return this.m_ValueX;
			}
			set
			{
				this.SetX(value);
			}
		}

		public float NormalizedValueX
		{
			get
			{
				if (Mathf.Approximately(this.MinValue, this.MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(this.MinValue, this.MaxValue, this.ValueX);
			}
			set
			{
				this.ValueX = Mathf.Lerp(this.MinValue, this.MaxValue, value);
			}
		}

		public float ValueY
		{
			get
			{
				if (this.WholeNumbers)
				{
					return Mathf.Round(this.m_ValueY);
				}
				return this.m_ValueY;
			}
			set
			{
				this.SetY(value);
			}
		}

		public float NormalizedValueY
		{
			get
			{
				if (Mathf.Approximately(this.MinValue, this.MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(this.MinValue, this.MaxValue, this.ValueY);
			}
			set
			{
				this.ValueY = Mathf.Lerp(this.MinValue, this.MaxValue, value);
			}
		}

		public BoxSlider.BoxSliderEvent OnValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		private float StepSize
		{
			get
			{
				if (!this.WholeNumbers)
				{
					return (this.MaxValue - this.MinValue) * 0.1f;
				}
				return 1f;
			}
		}

		protected BoxSlider()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public void LayoutComplete()
		{
		}

		public void GraphicUpdateComplete()
		{
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateCachedReferences();
			this.SetX(this.m_ValueX, false);
			this.SetY(this.m_ValueY, false);
			this.UpdateVisuals();
		}

		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			base.OnDisable();
		}

		private void UpdateCachedReferences()
		{
			if (this.m_HandleRect)
			{
				this.m_HandleTransform = this.m_HandleRect.transform;
				if (this.m_HandleTransform.parent != null)
				{
					this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
					return;
				}
			}
			else
			{
				this.m_HandleContainerRect = null;
			}
		}

		private void SetX(float input)
		{
			this.SetX(input, true);
		}

		private void SetX(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, this.MinValue, this.MaxValue);
			if (this.WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (this.m_ValueX == num)
			{
				return;
			}
			this.m_ValueX = num;
			this.UpdateVisuals();
			if (sendCallback)
			{
				this.m_OnValueChanged.Invoke(num, this.ValueY);
			}
		}

		private void SetY(float input)
		{
			this.SetY(input, true);
		}

		private void SetY(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, this.MinValue, this.MaxValue);
			if (this.WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (this.m_ValueY == num)
			{
				return;
			}
			this.m_ValueY = num;
			this.UpdateVisuals();
			if (sendCallback)
			{
				this.m_OnValueChanged.Invoke(this.ValueX, num);
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			this.UpdateVisuals();
		}

		private void UpdateVisuals()
		{
			this.m_Tracker.Clear();
			if (this.m_HandleContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 zero = Vector2.zero;
				Vector2 one = Vector2.one;
				zero[0] = (one[0] = this.NormalizedValueX);
				zero[1] = (one[1] = this.NormalizedValueY);
				this.m_HandleRect.anchorMin = zero;
				this.m_HandleRect.anchorMax = one;
			}
		}

		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform handleContainerRect = this.m_HandleContainerRect;
			if (handleContainerRect != null && handleContainerRect.rect.size[0] > 0f)
			{
				Vector2 a;
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(handleContainerRect, eventData.position, cam, out a))
				{
					return;
				}
				a -= handleContainerRect.rect.position;
				float normalizedValueX = Mathf.Clamp01((a - this.m_Offset)[0] / handleContainerRect.rect.size[0]);
				this.NormalizedValueX = normalizedValueX;
				float normalizedValueY = Mathf.Clamp01((a - this.m_Offset)[1] / handleContainerRect.rect.size[1]);
				this.NormalizedValueY = normalizedValueY;
			}
		}

		private bool CanDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.CanDrag(eventData))
			{
				return;
			}
			base.OnPointerDown(eventData);
			this.m_Offset = Vector2.zero;
			if (this.m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera))
			{
				Vector2 offset;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out offset))
				{
					this.m_Offset = offset;
				}
				this.m_Offset.y = -this.m_Offset.y;
				return;
			}
			this.UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.CanDrag(eventData))
			{
				return;
			}
			this.UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

	

		[SerializeField]
		private RectTransform m_HandleRect;

		[Space(6f)]
		[SerializeField]
		private float m_MinValue;

		[SerializeField]
		private float m_MaxValue = 1f;

		[SerializeField]
		private bool m_WholeNumbers;

		[SerializeField]
		private float m_ValueX = 1f;

		[SerializeField]
		private float m_ValueY = 1f;

		[Space(6f)]
		[SerializeField]
		private BoxSlider.BoxSliderEvent m_OnValueChanged = new BoxSlider.BoxSliderEvent();

		private Transform m_HandleTransform;

		private RectTransform m_HandleContainerRect;

		private Vector2 m_Offset = Vector2.zero;

		private DrivenRectTransformTracker m_Tracker;

		public enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}

		[Serializable]
		public class BoxSliderEvent : UnityEvent<float, float>
		{
		}

		private enum Axis
		{
			Horizontal,
			Vertical
		}
	}
}
