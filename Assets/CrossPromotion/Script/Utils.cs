using System;
using System.Collections;
using UnityEngine;

namespace crosspromotion {
	public static class Utils {
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

		public static string GetValueFromUrl(string url, string parameterId) {
			string[] split = url.Split(Convert.ToChar("?"));
			for (int i = 1; i < split.Length; i++) {
				string t = split[i];
				string[] split2 = t.Split(Convert.ToChar("&"));
				for (int j = 0; j < split2.Length; j++)
				{
					string t2 = split2[j];
					string[] split3 = t2.Split(Convert.ToChar("="));
					if (split3.Length == 2)
					{
						string key = split3[0];
						string val = split3[1];
						if (key == parameterId) return val;
					}
				}
			}
			return "";
		}
	}
}