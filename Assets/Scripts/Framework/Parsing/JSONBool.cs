using System;
using System.IO;
using System.Text;

namespace Framework.Parsing
{
	public class JSONBool : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		public override bool IsBoolean
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
				bool data;
				if (bool.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		public override bool AsBool
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

		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(6);
			aWriter.Write(this.m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data ? "true" : "false");
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && this.m_Data == (bool)obj;
		}

		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		private bool m_Data;
	}
}
