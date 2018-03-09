using System;
using System.Collections;
using UnityEngine;

namespace crosspromotion
{
	public static class Utils
	{
		public static bool checkPackageAppIsPresent(string package) {
			AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

			//take the list of all packages on the device
			AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
			int num = appList.Call<int>("size");
			for (int i = 0; i < num; i++) {
				AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
				string packageNew = appInfo.Get<string>("packageName");
				if (packageNew.CompareTo(package) == 0) {
					return true;
				}
			}
			return false;
		}

		public static bool HasInstalledFbApp() {
			return Utils.checkPackageAppIsPresent("com.facebook.katana");
		}
		public static IEnumerator CheckInternetConnection(Action<bool> action, string url = "http://google.com") {
			WWW www = new WWW(url);
			yield return www;
			if (www.error != null) {
				action(false);
			}
			else {
				action(true);
			}
		}
	}
}