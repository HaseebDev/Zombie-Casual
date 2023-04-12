using System;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
	private void Start()
	{
		this.text.text = (Taptic.tapticOn ? "TURN OFF" : "TURN ON");
	}

	public void TriggerTaptic(string type)
	{
		if (type == "warning")
		{
			Taptic.Warning();
			return;
		}
		if (type == "failure")
		{
			Taptic.Failure();
			return;
		}
		if (type == "success")
		{
			Taptic.Success();
			return;
		}
		if (type == "light")
		{
			Taptic.Light();
			return;
		}
		if (type == "medium")
		{
			Taptic.Medium();
			return;
		}
		if (type == "heavy")
		{
			Taptic.Heavy();
			return;
		}
		if (type == "default")
		{
			Taptic.Default();
			return;
		}
		if (type == "vibrate")
		{
			Taptic.Vibrate();
			return;
		}
		if (type == "selection")
		{
			Taptic.Selection();
		}
	}

	public void Toggle()
	{
		Taptic.tapticOn = !Taptic.tapticOn;
		this.text.text = (Taptic.tapticOn ? "TURN OFF" : "TURN ON");
		this.tapticImage.color = (Taptic.tapticOn ? this.on : this.off);
		Taptic.Selection();
	}

	[SerializeField]
	private Text text;

	[SerializeField]
	private Image tapticImage;

	[SerializeField]
	private Color on;

	[SerializeField]
	private Color off;
}
