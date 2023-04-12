using System;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class DropDownListItem
	{
		public string Caption
		{
			get
			{
				return this._caption;
			}
			set
			{
				this._caption = value;
				if (this.OnUpdate != null)
				{
					this.OnUpdate();
				}
			}
		}

		public Sprite Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this._image = value;
				if (this.OnUpdate != null)
				{
					this.OnUpdate();
				}
			}
		}

		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				this._isDisabled = value;
				if (this.OnUpdate != null)
				{
					this.OnUpdate();
				}
			}
		}

		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public DropDownListItem(string caption = "", string inId = "", Sprite image = null, bool disabled = false, Action onSelect = null)
		{
			this._caption = caption;
			this._image = image;
			this._id = inId;
			this._isDisabled = disabled;
			this.OnSelect = onSelect;
		}

		[SerializeField]
		private string _caption;

		[SerializeField]
		private Sprite _image;

		[SerializeField]
		private bool _isDisabled;

		[SerializeField]
		private string _id;

		public Action OnSelect;

		internal Action OnUpdate;
	}
}
