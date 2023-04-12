using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealPermissionCallbacks : AndroidJavaProxy
	{
		internal AppodealPermissionCallbacks(IPermissionGrantedListener listener) : base("com.appodeal.ads.utils.PermissionsHelper$AppodealPermissionCallbacks")
		{
			this.listener = listener;
		}

		private void writeExternalStorageResponse(int result)
		{
			this.listener.writeExternalStorageResponse(result);
		}

		private void accessCoarseLocationResponse(int result)
		{
			this.listener.accessCoarseLocationResponse(result);
		}

		private IPermissionGrantedListener listener;
	}
}
