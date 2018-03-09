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
			text +="\n"+ type + " " + value;
		});
		CrossPromotion.ListenOnOpenAction(delegate () {
			text += "\n Open";
		});
		CrossPromotion.ListenInteractAction(delegate (Interact interact, string appId) {
			text += "\n"+interact + " " + appId;
		});
		CrossPromotion.ListenOnCloseAction(delegate()
		{
			text += "\n Close";
		});
	}

	void OnGUI() {
		if (GUILayout.Button("Show")) {
			CrossPromotion.Show(domain);
		}
		GUILayout.Label(text);
	}
}
