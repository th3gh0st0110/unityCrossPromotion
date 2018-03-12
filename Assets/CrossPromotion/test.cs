using System.Collections;
using System.Collections.Generic;
using crosspromotion;
using LitJson;
using UnityEngine;

public class test : MonoBehaviour {
	public string domain = "https://test-adbdb.firebaseapp.com";
	private string text = "";
	void Start() {
		CrossPromotion.ListenError(delegate (Error error, string message) {
			text += "\n" + error + ": " + message;
		});
		CrossPromotion.ListenClaimReward(delegate (string type, int value) {
			text += "\n" + type + " " + value;
		});
		CrossPromotion.ListenOnOpenAction(delegate () {
			text += "\n Open";
		});
		CrossPromotion.ListenInteractAction(delegate (Interact interact, string appId) {
			text += "\n" + interact + " " + appId;
		});
		CrossPromotion.ListenOnCloseAction(delegate () {
			text += "\n Close";
		});
		string url =
			"market://details?id=com.Zonmob.GodofEra&referrer=utm_source%3DShadow_of_death%26utm_medium%3DGiftbox%26utm_campaign%3D3%2524_Giftbox_SD";
		Debug.Log(Utils.GetValueFromUrl(url, "id"));
	}

	void OnGUI() {
		if (GUI.Button(new Rect(0, 0, 200, 200), "Show")) {
			CrossPromotion.Show(domain);
		}
		GUILayout.Label(text);
	}
}
