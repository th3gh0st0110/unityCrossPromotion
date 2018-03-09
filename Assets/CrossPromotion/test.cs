using System.Collections;
using System.Collections.Generic;
using crosspromotion;
using LitJson;
using UnityEngine;

public class test : MonoBehaviour {
	public string domain = "https://test-adbdb.firebaseapp.com";
	private string text = "";
	private CrossPromotionData data;
	void Start() {
		CrossPromotion.Initialize(domain);
		data = new CrossPromotionData();
		CrossPromotion.ListenError(delegate (Error error, string message) {
			text = "\n" + error + ": " + message;
		});
		CrossPromotion.ListenClaimReward(delegate (string type, int value) {
			text = type + " " + value;
		});
		CrossPromotion.ListenOnOpenAction(delegate () {
			text = "Open";
		});
		CrossPromotion.ListenInteractAction(delegate (Interact interact, string appId) {
			text = interact + " " + appId;
		});
		CrossPromotion.ListenOnCloseAction(delegate()
		{
			text = "Close";
		});
	}

	void OnGUI() {
		if (GUILayout.Button("Show")) {
			CrossPromotion.Show(data);
		}
		GUILayout.Label(text);
	}
}
