using System;

namespace Framework.Interfaces
{
	public interface IRewardedVideoAdListener
	{
		void OnRewardedVideoLoaded();

		void OnRewardedVideoShown();

		void OnRewardedVideoFailedToLoad();

		void OnRewardedVideoClosed(bool finished);

		void OnRewardedVideoFinished(double amount, string name);
	}
}
