using System.Collections;
using System.Collections.Generic;
using crosspromotion;
using LitJson;
using UnityEngine;

namespace crosspromotion
{
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
			CrossPromotion.ListenInteractAction(delegate (Interact interact, CrossPromotionItemConfig appId) {
				text += "\n" + interact + " " + appId.name;
			});
			CrossPromotion.ListenOnCloseAction(delegate () {
				text += "\n Close";
			});
		}

		void OnGUI() {
			if (GUI.Button(new Rect(0, 0, 200, 200), "Show")) {
				CrossPromotion.Show(domain);
			}
			GUILayout.Label(text);
		}
	}


}
