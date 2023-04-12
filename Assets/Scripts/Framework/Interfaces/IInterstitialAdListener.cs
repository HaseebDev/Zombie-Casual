using System;

namespace Framework.Interfaces
{
	public interface IInterstitialAdListener
	{
		void OnInterstitialLoaded(bool isPrecache);

		void OnInterstitialShown();

		void OnInterstitialClosed();

		void OnInterstitialClicked();
	}
}
