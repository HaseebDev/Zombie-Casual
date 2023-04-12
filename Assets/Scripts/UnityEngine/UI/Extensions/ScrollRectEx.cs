using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/ScrollRectEx")]
	public class ScrollRectEx : ScrollRect
	{
		private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				foreach (Component component in parent.GetComponents<Component>())
				{
					if (component is T)
					{
						action((T)((object)((IEventSystemHandler)component)));
					}
				}
				parent = parent.parent;
			}
		}

		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			this.DoForParents<IInitializePotentialDragHandler>(delegate(IInitializePotentialDragHandler parent)
			{
				parent.OnInitializePotentialDrag(eventData);
			});
			base.OnInitializePotentialDrag(eventData);
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (this.routeToParent)
			{
				this.DoForParents<IDragHandler>(delegate(IDragHandler parent)
				{
					parent.OnDrag(eventData);
				});
				return;
			}
			base.OnDrag(eventData);
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (!base.horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
			{
				this.routeToParent = true;
			}
			else if (!base.vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
			{
				this.routeToParent = true;
			}
			else
			{
				this.routeToParent = false;
			}
			if (this.routeToParent)
			{
				this.DoForParents<IBeginDragHandler>(delegate(IBeginDragHandler parent)
				{
					parent.OnBeginDrag(eventData);
				});
				return;
			}
			base.OnBeginDrag(eventData);
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			if (this.routeToParent)
			{
				this.DoForParents<IEndDragHandler>(delegate(IEndDragHandler parent)
				{
					parent.OnEndDrag(eventData);
				});
			}
			else
			{
				base.OnEndDrag(eventData);
			}
			this.routeToParent = false;
		}

		public override void OnScroll(PointerEventData eventData)
		{
			if (!base.horizontal && Math.Abs(eventData.scrollDelta.x) > Math.Abs(eventData.scrollDelta.y))
			{
				this.routeToParent = true;
			}
			else if (!base.vertical && Math.Abs(eventData.scrollDelta.x) < Math.Abs(eventData.scrollDelta.y))
			{
				this.routeToParent = true;
			}
			else
			{
				this.routeToParent = false;
			}
			if (this.routeToParent)
			{
				this.DoForParents<IScrollHandler>(delegate(IScrollHandler parent)
				{
					parent.OnScroll(eventData);
				});
				return;
			}
			base.OnScroll(eventData);
		}

		private bool routeToParent;
	}
}
