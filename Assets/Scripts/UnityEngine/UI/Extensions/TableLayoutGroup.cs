using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Table Layout Group")]
	public class TableLayoutGroup : LayoutGroup
	{
		public TableLayoutGroup.Corner StartCorner
		{
			get
			{
				return this.startCorner;
			}
			set
			{
				base.SetProperty<TableLayoutGroup.Corner>(ref this.startCorner, value);
			}
		}

		public float[] ColumnWidths
		{
			get
			{
				return this.columnWidths;
			}
			set
			{
				base.SetProperty<float[]>(ref this.columnWidths, value);
			}
		}

		public float MinimumRowHeight
		{
			get
			{
				return this.minimumRowHeight;
			}
			set
			{
				base.SetProperty<float>(ref this.minimumRowHeight, value);
			}
		}

		public bool FlexibleRowHeight
		{
			get
			{
				return this.flexibleRowHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.flexibleRowHeight, value);
			}
		}

		public float ColumnSpacing
		{
			get
			{
				return this.columnSpacing;
			}
			set
			{
				base.SetProperty<float>(ref this.columnSpacing, value);
			}
		}

		public float RowSpacing
		{
			get
			{
				return this.rowSpacing;
			}
			set
			{
				base.SetProperty<float>(ref this.rowSpacing, value);
			}
		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			float num = (float)base.padding.horizontal;
			int num2 = Mathf.Min(base.rectChildren.Count, this.columnWidths.Length);
			for (int i = 0; i < num2; i++)
			{
				num += this.columnWidths[i];
				num += this.columnSpacing;
			}
			num -= this.columnSpacing;
			base.SetLayoutInputForAxis(num, num, 0f, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			int num = this.columnWidths.Length;
			int num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num);
			this.preferredRowHeights = new float[num2];
			float num3 = (float)base.padding.vertical;
			float num4 = (float)base.padding.vertical;
			if (num2 > 1)
			{
				float num5 = (float)(num2 - 1) * this.rowSpacing;
				num3 += num5;
				num4 += num5;
			}
			if (this.flexibleRowHeight)
			{
				for (int i = 0; i < num2; i++)
				{
					float num6 = this.minimumRowHeight;
					float num7 = this.minimumRowHeight;
					for (int j = 0; j < num; j++)
					{
						int num8 = i * num + j;
						if (num8 == base.rectChildren.Count)
						{
							break;
						}
						num7 = Mathf.Max(LayoutUtility.GetPreferredHeight(base.rectChildren[num8]), num7);
						num6 = Mathf.Max(LayoutUtility.GetMinHeight(base.rectChildren[num8]), num6);
					}
					num3 += num6;
					num4 += num7;
					this.preferredRowHeights[i] = num7;
				}
			}
			else
			{
				for (int k = 0; k < num2; k++)
				{
					this.preferredRowHeights[k] = this.minimumRowHeight;
				}
				num3 += (float)num2 * this.minimumRowHeight;
				num4 = num3;
			}
			num4 = Mathf.Max(num3, num4);
			base.SetLayoutInputForAxis(num3, num4, 1f, 1);
		}

		public override void SetLayoutHorizontal()
		{
			if (this.columnWidths.Length == 0)
			{
				this.columnWidths = new float[1];
			}
			int num = this.columnWidths.Length;
			int num2 = (int)((int)this.startCorner % (int)TableLayoutGroup.Corner.LowerLeft);
			float num3 = 0f;
			int num4 = Mathf.Min(base.rectChildren.Count, this.columnWidths.Length);
			for (int i = 0; i < num4; i++)
			{
				num3 += this.columnWidths[i];
				num3 += this.columnSpacing;
			}
			num3 -= this.columnSpacing;
			float num5 = base.GetStartOffset(0, num3);
			if (num2 == 1)
			{
				num5 += num3;
			}
			float num6 = num5;
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				int num7 = j % num;
				if (num7 == 0)
				{
					num6 = num5;
				}
				if (num2 == 1)
				{
					num6 -= this.columnWidths[num7];
				}
				base.SetChildAlongAxis(base.rectChildren[j], 0, num6, this.columnWidths[num7]);
				if (num2 == 1)
				{
					num6 -= this.columnSpacing;
				}
				else
				{
					num6 += this.columnWidths[num7] + this.columnSpacing;
				}
			}
		}

		public override void SetLayoutVertical()
		{
			int num = this.columnWidths.Length;
			int num2 = this.preferredRowHeights.Length;
			int num3 = (int)((int)this.startCorner /(int) TableLayoutGroup.Corner.LowerLeft);
			float num4 = 0f;
			for (int i = 0; i < num2; i++)
			{
				num4 += this.preferredRowHeights[i];
			}
			if (num2 > 1)
			{
				num4 += (float)(num2 - 1) * this.rowSpacing;
			}
			float num5 = base.GetStartOffset(1, num4);
			if (num3 == 1)
			{
				num5 += num4;
			}
			float num6 = num5;
			for (int j = 0; j < num2; j++)
			{
				if (num3 == 1)
				{
					num6 -= this.preferredRowHeights[j];
				}
				for (int k = 0; k < num; k++)
				{
					int num7 = j * num + k;
					if (num7 == base.rectChildren.Count)
					{
						break;
					}
					base.SetChildAlongAxis(base.rectChildren[num7], 1, num6, this.preferredRowHeights[j]);
				}
				if (num3 == 1)
				{
					num6 -= this.rowSpacing;
				}
				else
				{
					num6 += this.preferredRowHeights[j] + this.rowSpacing;
				}
			}
			this.preferredRowHeights = null;
		}

		[SerializeField]
		protected TableLayoutGroup.Corner startCorner;

		[SerializeField]
		protected float[] columnWidths = new float[]
		{
			96f
		};

		[SerializeField]
		protected float minimumRowHeight = 32f;

		[SerializeField]
		protected bool flexibleRowHeight = true;

		[SerializeField]
		protected float columnSpacing;

		[SerializeField]
		protected float rowSpacing;

		private float[] preferredRowHeights;

		public enum Corner
		{
			UpperLeft,
			UpperRight,
			LowerLeft,
			LowerRight
		}
	}
}
