using System;
using System.IO;
using System.Text;

namespace Framework.Parsing
{
	public class JSONString : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		public override bool IsString
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
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			string text = obj as string;
			if (text != null)
			{
				return this.m_Data == text;
			}
			JSONString jsonstring = obj as JSONString;
			return jsonstring != null && this.m_Data == jsonstring.m_Data;
		}

		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		private string m_Data;
	}
}
