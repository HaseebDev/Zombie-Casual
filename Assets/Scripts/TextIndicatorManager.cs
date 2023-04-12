using System;

public class TextIndicatorManager : PoolObjectManager
{
	public void SetTextIndi(string _text)
	{
		this.poolObject.name = _text;
		this.poolObject.SetActive(false);
		this.poolObject.SetActive(true);
	}
}
