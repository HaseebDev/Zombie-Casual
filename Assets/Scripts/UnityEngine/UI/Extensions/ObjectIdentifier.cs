using System;

namespace UnityEngine.UI.Extensions
{
	public class ObjectIdentifier : MonoBehaviour
	{
		public void SetID()
		{
			this.id = Guid.NewGuid().ToString();
			this.CheckForRelatives();
		}

		private void CheckForRelatives()
		{
			if (base.transform.parent == null)
			{
				this.idParent = null;
				return;
			}
			foreach (ObjectIdentifier objectIdentifier in base.GetComponentsInChildren<ObjectIdentifier>())
			{
				if (objectIdentifier.transform.gameObject != base.gameObject)
				{
					objectIdentifier.idParent = this.id;
					objectIdentifier.SetID();
				}
			}
		}

		public string prefabName;

		public string id;

		public string idParent;

		public bool dontSave;
	}
}
