using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Extensions/UI_Knob")]
    public class UI_Knob : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            this._canDrag = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this._canDrag = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this._canDrag = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this._canDrag = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.SetInitPointerData(eventData);
        }

        private void SetInitPointerData(PointerEventData eventData)
        {
            this._initRotation = base.transform.rotation;
            this._currentVector = eventData.position - new Vector2(base.transform.position.x , transform.position.y);
            this._initAngle = Mathf.Atan2(this._currentVector.y, this._currentVector.x) * 57.29578f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!this._canDrag)
            {
                this.SetInitPointerData(eventData);
                return;
            }

            this._currentVector = eventData.position - new Vector2(base.transform.position.x, base.transform.position.y);
            this._currentAngle = Mathf.Atan2(this._currentVector.y, this._currentVector.x) * 57.29578f;
            Quaternion rhs = Quaternion.AngleAxis(this._currentAngle - this._initAngle, base.transform.forward);
            rhs.eulerAngles = new Vector3(0f, 0f, rhs.eulerAngles.z);
            Quaternion rotation = this._initRotation * rhs;
            if (this.direction == UI_Knob.Direction.CW)
            {
                this.knobValue = 1f - rotation.eulerAngles.z / 360f;
                if (this.snapToPosition)
                {
                    this.SnapToPosition(ref this.knobValue);
                    rotation.eulerAngles = new Vector3(0f, 0f, 360f - 360f * this.knobValue);
                }
            }
            else
            {
                this.knobValue = rotation.eulerAngles.z / 360f;
                if (this.snapToPosition)
                {
                    this.SnapToPosition(ref this.knobValue);
                    rotation.eulerAngles = new Vector3(0f, 0f, 360f * this.knobValue);
                }
            }

            if (Mathf.Abs(this.knobValue - this._previousValue) > 0.5f)
            {
                if (this.knobValue < 0.5f && this.loops > 1 && this._currentLoops < (float) (this.loops - 1))
                {
                    this._currentLoops += 1f;
                }
                else if (this.knobValue > 0.5f && this._currentLoops >= 1f)
                {
                    this._currentLoops -= 1f;
                }
                else
                {
                    if (this.knobValue > 0.5f && this._currentLoops == 0f)
                    {
                        this.knobValue = 0f;
                        base.transform.localEulerAngles = Vector3.zero;
                        this.SetInitPointerData(eventData);
                        this.InvokeEvents(this.knobValue + this._currentLoops);
                        return;
                    }

                    if (this.knobValue < 0.5f && this._currentLoops == (float) (this.loops - 1))
                    {
                        this.knobValue = 1f;
                        base.transform.localEulerAngles = Vector3.zero;
                        this.SetInitPointerData(eventData);
                        this.InvokeEvents(this.knobValue + this._currentLoops);
                        return;
                    }
                }
            }

            if (this.maxValue > 0f && this.knobValue + this._currentLoops > this.maxValue)
            {
                this.knobValue = this.maxValue;
                float z = (this.direction == UI_Knob.Direction.CW)
                    ? (360f - 360f * this.maxValue)
                    : (360f * this.maxValue);
                base.transform.localEulerAngles = new Vector3(0f, 0f, z);
                this.SetInitPointerData(eventData);
                this.InvokeEvents(this.knobValue);
                return;
            }

            base.transform.rotation = rotation;
            this.InvokeEvents(this.knobValue + this._currentLoops);
            this._previousValue = this.knobValue;
        }

        private void SnapToPosition(ref float knobValue)
        {
            float num = 1f / (float) this.snapStepsPerLoop;
            float num2 = Mathf.Round(knobValue / num) * num;
            knobValue = num2;
        }

        private void InvokeEvents(float value)
        {
            if (this.clampOutput01)
            {
                value /= (float) this.loops;
            }

            this.OnValueChanged.Invoke(value);
        }

        [Tooltip("Direction of rotation CW - clockwise, CCW - counterClockwise")]
        public UI_Knob.Direction direction;

        [HideInInspector] public float knobValue;

        [Tooltip(
            "Max value of the knob, maximum RAW output value knob can reach, overrides snap step, IF set to 0 or higher than loops, max value will be set by loops")]
        public float maxValue;

        [Tooltip("How many rotations knob can do, if higher than max value, the latter will limit max value")]
        public int loops = 1;

        [Tooltip("Clamp output value between 0 and 1, usefull with loops > 1")]
        public bool clampOutput01;

        [Tooltip("snap to position?")] public bool snapToPosition;

        [Tooltip("Number of positions to snap")]
        public int snapStepsPerLoop = 10;

        [Space(30f)] public KnobFloatValueEvent OnValueChanged;

        private float _currentLoops;

        private float _previousValue;

        private float _initAngle;

        private float _currentAngle;

        private Vector2 _currentVector;

        private Quaternion _initRotation;

        private bool _canDrag;

        public enum Direction
        {
            CW,
            CCW
        }
    }
}