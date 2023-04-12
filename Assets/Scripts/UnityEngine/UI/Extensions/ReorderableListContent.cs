using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	public class ReorderableListContent : MonoBehaviour
	{
		public void OnTransformChildrenChanged()
		{
			base.StartCoroutine(this.RefreshChildren());
		}

		public void Init(ReorderableList extList)
		{
			this._extList = extList;
			this._rect = base.GetComponent<RectTransform>();
			this._cachedChildren = new List<Transform>();
			this._cachedListElement = new List<ReorderableListElement>();
			base.StartCoroutine(this.RefreshChildren());
		}

		private IEnumerator RefreshChildren()
		{
			for (int i = 0; i < this._rect.childCount; i++)
			{
				if (!this._cachedChildren.Contains(this._rect.GetChild(i)))
				{
					this._ele = (this._rect.GetChild(i).gameObject.GetComponent<ReorderableListElement>() ?? this._rect.GetChild(i).gameObject.AddComponent<ReorderableListElement>());
					this._ele.Init(this._extList);
					this._cachedChildren.Add(this._rect.GetChild(i));
					this._cachedListElement.Add(this._ele);
				}
			}
			yield return 0;
			for (int j = this._cachedChildren.Count - 1; j >= 0; j--)
			{
				if (this._cachedChildren[j] == null)
				{
					this._cachedChildren.RemoveAt(j);
					this._cachedListElement.RemoveAt(j);
				}
			}
			yield break;
		}

		private List<Transform> _cachedChildren;

		private List<ReorderableListElement> _cachedListElement;

		private ReorderableListElement _ele;

		private ReorderableList _extList;

		private RectTransform _rect;
	}
}
