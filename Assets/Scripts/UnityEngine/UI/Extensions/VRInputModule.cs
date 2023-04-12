using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Event/VR Input Module")]
	public class VRInputModule : BaseInputModule
	{
		protected override void Awake()
		{
			VRInputModule._singleton = this;
		}

		public override void Process()
		{
			if (VRInputModule.targetObject == null)
			{
				VRInputModule.mouseClicked = false;
				return;
			}
		}

		public static void PointerSubmit(GameObject obj)
		{
			VRInputModule.targetObject = obj;
			VRInputModule.mouseClicked = true;
			if (VRInputModule.mouseClicked)
			{
				BaseEventData baseEventData = new BaseEventData(VRInputModule._singleton.eventSystem);
				baseEventData.selectedObject = VRInputModule.targetObject;
				ExecuteEvents.Execute<ISubmitHandler>(VRInputModule.targetObject, baseEventData, ExecuteEvents.submitHandler);
				MonoBehaviour.print("clicked " + VRInputModule.targetObject.name);
				VRInputModule.mouseClicked = false;
			}
		}

		public static void PointerExit(GameObject obj)
		{
			MonoBehaviour.print("PointerExit " + obj.name);
			PointerEventData eventData = new PointerEventData(VRInputModule._singleton.eventSystem);
			ExecuteEvents.Execute<IPointerExitHandler>(obj, eventData, ExecuteEvents.pointerExitHandler);
			ExecuteEvents.Execute<IDeselectHandler>(obj, eventData, ExecuteEvents.deselectHandler);
		}

		public static void PointerEnter(GameObject obj)
		{
			MonoBehaviour.print("PointerEnter " + obj.name);
			PointerEventData pointerEventData = new PointerEventData(VRInputModule._singleton.eventSystem);
			pointerEventData.pointerEnter = obj;
			RaycastResult pointerCurrentRaycast = new RaycastResult
			{
				worldPosition = VRInputModule.cursorPosition
			};
			pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
			ExecuteEvents.Execute<IPointerEnterHandler>(obj, pointerEventData, ExecuteEvents.pointerEnterHandler);
		}

		public static GameObject targetObject;

		private static VRInputModule _singleton;

		private int counter;

		private static bool mouseClicked;

		public static Vector3 cursorPosition;
	}
}
