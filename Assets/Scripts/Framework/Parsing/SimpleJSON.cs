using System;

namespace Framework.Parsing
{
	public static class SimpleJSON
	{
		public static JSONNode Parse(string aJSON)
		{
			return JSONNode.Parse(aJSON);
		}
	}
}
