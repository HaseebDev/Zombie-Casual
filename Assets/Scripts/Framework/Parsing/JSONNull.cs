using System;
using System.IO;
using System.Text;

namespace Framework.Parsing
{
	public class JSONNull : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		public override bool IsNull
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
				return "null";
			}
			set
			{
			}
		}

		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(5);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}
