using System;
using System.IO;
using System.Text;

namespace Framework.Parsing
{
	public class JSONNumber : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				double data;
				if (double.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		public override double AsDouble
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(4);
			aWriter.Write(this.m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data);
		}

		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return this.m_Data == jsonnumber.m_Data;
			}
			return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
		}

		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		private double m_Data;
	}
}
