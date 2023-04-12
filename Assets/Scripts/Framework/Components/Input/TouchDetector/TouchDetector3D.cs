using System;
using System.Linq;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Components.Input.TouchDetector
{
    public class TouchDetector3D : MonoBehaviour
    {
        private RaycastHit[] _raycastHit;

        private void Start()
        {
            _raycastHit = new RaycastHit[5];
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    CastLine(UnityEngine.Input.mousePosition);
                    //CastSingleLine(UnityEngine.Input.mousePosition);
                }

            }

#else
            foreach (Touch touch in UnityEngine.Input.touches)
            {
                int id = touch.fingerId;
                if (!EventSystem.current.IsPointerOverGameObject(id))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        CastLine(touch.position);
                        //CastSingleLine(touch.position);
                    }
                }
            }
#endif
        }

        public void CastLine(Vector2 postion)
        {
            if (!EnableTouch)
                return;

            var mousePos = postion;
            if (this.lastMousePos != mousePos)
            {
                Vector2 screenPos = new Vector2(mousePos.x, mousePos.y);
                int numHit = Physics.RaycastNonAlloc(GamePlayController.instance.GetMainCamera().ScreenPointToRay(UnityEngine.Input.mousePosition), _raycastHit, 1000f, this.collisionLayerMask);
                if (numHit > 0)
                {
                    for (int i = 0; i < numHit; i++)
                    {
                        if (_raycastHit[i].collider.tag.Equals(TagConstant.TAG_CLICKABLE))
                        {
                            var iClick = _raycastHit[i].collider.GetComponent<IClickableObject>();
                            if (iClick != null)
                            {
                                iClick.OnPointerUp();
                            }
                        }
                        else
                        {
                            EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_GROUND, _raycastHit[i].point, true);
                        }
                    }
                }


            }

        }

        //public void CastSingleLine(Vector2 postion)
        //{
        //    if (!EnableTouch)
        //        return;

        //    var mousePos = postion;
        //    if (this.lastMousePos != mousePos)
        //    {
        //        Vector2 screenPos = new Vector2(mousePos.x, mousePos.y);
        //        RaycastHit hitInfo;
        //        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hitInfo, int.MaxValue, this.collisionLayerMask);
        //        if (hit)
        //        {
        //            if (hitInfo.collider.tag.Equals(TagConstant.TAG_CLICKABLE))
        //            {
        //                Debug.Log("clicked TAG_CLICKABLED!!!");
        //                var iClick = hitInfo.collider.GetComponent<IClickableObject>();
        //                if (iClick != null)
        //                {
        //                    iClick.OnPointerUp();
        //                }
        //            }
        //            //else if (hitInfo.collider.tag.Equals(TagConstant.TAG_ZOMBIE))
        //            //{
        //            //    Health health = hitInfo.collider.GetComponent<Health>();
        //            //    EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_ZOMBIE, health);
        //            //}
        //            else
        //            {
        //                EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_GROUND, hitInfo.point, true);
        //            }
        //        }


        //    }
        //}

        public Vector3 GetTouchWorldPos(Vector2 mousePos)
        {
            Vector3 result = Vector3.zero;
            Vector2 screenPos = new Vector2(mousePos.x, mousePos.y);

            //RaycastHit[] array = (from h in Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePos), 1000f, collisionLayerMask)
            //                      orderby h.distance
            //                      select h).ToArray<RaycastHit>();

            int numHit = Physics.RaycastNonAlloc(GamePlayController.instance.MainCamera.ScreenPointToRay(mousePos), _raycastHit, 1000f, collisionLayerMask);

            if (numHit > 0)
            {
                for (int i = 0; i < _raycastHit.Length; i++)
                {
                    // Debug.DrawLine(Camera.main.transform.position, _raycastHit[i].point, Color.red);
                    result = _raycastHit[i].point;
                    break;
                }
            }

            return result;
        }

        // private Vector2 mousePos;
        private Vector2 lastMousePos;

        [SerializeField]
        private LayerMask collisionLayerMask;

        public bool EnableTouch { get; private set; }

        public void SetEnableTouch(bool value)
        {
            EnableTouch = value;
        }
    }
}
