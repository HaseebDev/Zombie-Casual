using System;
using System.Collections.Generic;

public class AdPlacementEnum
{
	public AdPlacementEnum()
	{
		this.ads.Add("rewarded_video", "REWARDED VIDEO");
	}

	public string GetPlacment(string _key)
	{
		if (this.ads.ContainsKey(_key))
		{
			return this.ads[_key];
		}
		return "unknown";
	}

	private Dictionary<string, string> ads = new Dictionary<string, string>();
}
