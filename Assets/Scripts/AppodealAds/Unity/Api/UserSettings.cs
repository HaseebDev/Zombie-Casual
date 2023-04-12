using System;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Api
{
	public class UserSettings
	{
		private static IAppodealAdsClient getInstance()
		{
			if (UserSettings.client == null)
			{
				UserSettings.client = AppodealAdsClientFactory.GetAppodealAdsClient();
			}
			return UserSettings.client;
		}

		public UserSettings()
		{
			UserSettings.getInstance().getUserSettings();
		}

		public UserSettings setUserId(string id)
		{
			UserSettings.getInstance().setUserId(id);
			return this;
		}

		public UserSettings setAge(int age)
		{
			UserSettings.getInstance().setAge(age);
			return this;
		}

		public UserSettings setGender(UserSettings.Gender gender)
		{
			UserSettings.getInstance().setGender(gender);
			return this;
		}

		private static IAppodealAdsClient client;

		public enum Gender
		{
			OTHER,
			MALE,
			FEMALE
		}
	}
}
