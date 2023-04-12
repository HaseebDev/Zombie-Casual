using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers.Ads
{
	public class DummyAdSystem : BaseAdSystem
	{
		public override KeyValuePair<string, double> GetRewardParameters()
		{
			return new KeyValuePair<string, double>("coins", 1.0);
		}

		public override void Init()
		{
			if (!base.AdIsEnable())
			{
				return;
			}
			UnityEngine.Debug.Log("DUMMYADS: Инициализация завершена");
		}

		public override void ShowBanner()
		{
			if (!base.AdIsEnable())
			{
				return;
			}
			UnityEngine.Debug.Log("DUMMYADS: Показать баннер");
		}

		public override void HideBanner()
		{
			if (!base.AdIsEnable())
			{
				return;
			}
			UnityEngine.Debug.Log("DUMMYADS: Скрыть баннер");
		}

		public override void ShowInterstetial(Action<bool> IsLoaded)
		{
			if (!base.AdIsEnable())
			{
				IsLoaded(false);
				return;
			}
			IsLoaded(true);
			UnityEngine.Debug.Log("DUMMYADS: Показать межстраничку");
			if (this.interstitialAdListener != null)
			{
				this.interstitialAdListener.OnInterstitialShown();
				this.interstitialAdListener.OnInterstitialClosed();
			}
		}

		public override void ShowRewardedVideo(Action<bool> IsLoaded)
		{
			IsLoaded(true);
			UnityEngine.Debug.Log("DUMMYADS: Показать видео с наградой");
			if (this.rewardedVideoAdListener != null)
			{
				UnityEngine.Debug.Log("DUMMYADS: Показ видео с наградой успешно завершен");
				this.rewardedVideoAdListener.OnRewardedVideoShown();
				this.rewardedVideoAdListener.OnRewardedVideoFinished(1.0, "coins");
			}
		}

		public override bool rewardVideoIsLoaded()
		{
			UnityEngine.Debug.Log("DUMMYADS: Проверка на загрузку рекламы результат: true");
			return true;
		}
	}
}
