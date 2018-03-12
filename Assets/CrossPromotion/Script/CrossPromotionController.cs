using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using UnityEngine;

namespace crosspromotion {
	public class CrossPromotionController {
		static string fileName = "crossPromotion.data";
		private string domain;
		private UniWebView _webView;
		private CrossPromotionConfig crossPromotionConfig = new CrossPromotionConfig();
		private Action<Error, string> errorAction = delegate (Error error, string s) { };
		private Action<string, int> rewardAction = delegate (string s, int i) { };
		private Action onOpen = delegate { };
		private Action onClose = delegate { };
		private Action<Interact, string> interact = delegate (Interact interact1, string appID) { };
		private CrossPromotionData data;
		public CrossPromotionController(string domain) {
			this.domain = domain;
		}
		public void UpdateUrl(string domain) {
			this.domain = domain;
		}

		public CrossPromotionController ListenError(Action<Error, string> action) {
			errorAction += action;
			return this;
		}

		public CrossPromotionController ListenClaimReward(Action<string, int> action) {
			rewardAction += action;
			return this;
		}

		public CrossPromotionController ListenInteractAction(Action<Interact, string> action) {
			interact += action;
			return this;
		}

		public CrossPromotionController ListenOnOpenAction(Action action) {
			onOpen += action;
			return this;
		}

		public CrossPromotionController ListenOnCloseAction(Action action) {
			onClose += action;
			return this;
		}

		public void Show(CrossPromotionData data) {
			this.data = data;
			Action<bool> action = delegate (bool hasInternet) {
				if (hasInternet) {

					Action<bool, string> loadConfigCallBack = delegate (bool success, string s) {
						if (success) {
							CheckCrosspromotionStatus();
							OpenWebview();
						}
						else {
							errorAction.Invoke(Error.LoadConfig, s);
						}
					};
					Coroutiner.StartCoroutine(LoadConfig(loadConfigCallBack));

				}
				else {
					errorAction.Invoke(Error.NoInternet, "");
				}
			};
			Coroutiner.StartCoroutine(Utils.CheckInternetConnection(action, domain));
		}

		IEnumerator LoadConfig(Action<bool, string> callback) {
			WWW w = new WWW(domain + "/config/CrossPromotionConfig.txt");
			yield return w;
			if (!string.IsNullOrEmpty(w.error)) {
				callback.Invoke(false, w.error);
			}
			else {
				crossPromotionConfig = JsonMapper.ToObject<CrossPromotionConfig>(w.text);
				callback.Invoke(true, "");
			}
		}

		void CheckCrosspromotionStatus() {

			List<CrossPromotionItemConfig> items = crossPromotionConfig.GetList();
			for (int i = 0; i < items.Count; i++) {
				CrossPromotionItemConfig item = items[i];
				if (item.enable)
					CheckCrosspromotionStatus(item);
			}

		}

		void CheckCrosspromotionStatus(CrossPromotionItemConfig item) {
			string value = item.androidID;
#if UNITY_IPHONE
			value = item.iosID;
#endif
			if (Utils.checkPackageAppIsPresent(Utils.GetValueFromUrl(value, "id"))) {
				data.GetItem(item.id).SetComplete();
			}
		}
		void OpenWebview() {
			if (_webView != null) {
				return;
			}
			_webView = CreateWebView();
			string t = string.Format("\"{0}\"", JsonMapper.ToJson(data));
			_webView.url = domain + "/?data=" + JsonMapper.ToJson(data);
			int bottomInset = UniWebViewHelper.screenHeight;
			_webView.insets = new UniWebViewEdgeInsets(0, 0, 0, 0);
			_webView.OnReceivedMessage += OnReceivedMessage;

			_webView.Load();
			_webView.Show();
			onOpen.Invoke();
		}
		void OnReceivedMessage(UniWebView webView, UniWebViewMessage message) {

			if (message.path == "close") {
				Close(webView);
			}
			if (message.path == "reward") {
				int id = 0;
				int value = 0;
				string type = message.args["type"];
				if (int.TryParse(message.args["value"], out value) & int.TryParse(message.args["id"], out id)) {
					string appId = crossPromotionConfig.GetAppId(id);
					data.GetItem(id).Claim();
					rewardAction.Invoke(type, value);
					interact.Invoke(Interact.ClaimReward, appId);
				}

			}
			if (message.path == "OpenApp") {
				Close(webView);
				try {
					string id = message.args["id"];
					string appId = crossPromotionConfig.GetAppId(int.Parse(id));
					Coroutiner.StartCoroutine(DelayOpenApp(appId));

				}
				catch (Exception e) {
					errorAction.Invoke(Error.LoadConfig, e.ToString());
				}

			}
		}

		private IEnumerator DelayOpenApp(string appId) {
			yield return null;
			yield return null;
			interact.Invoke(Interact.InstallApp, appId);
			Application.OpenURL(appId);
		}

		void Close(UniWebView webView) {
			GameObject.Destroy(webView);
			_webView = null;
			onClose.Invoke();
		}
		UniWebView CreateWebView() {
			var webViewGameObject = GameObject.Find("WebView");
			if (webViewGameObject == null) {
				webViewGameObject = new GameObject("WebView");
			}

			var webView = webViewGameObject.AddComponent<UniWebView>();

			webView.toolBarShow = true;
			return webView;
		}
		public CrossPromotionData LoadUserData() {
			if (HasUserData()) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream stream = new FileStream(FilePath(), FileMode.Open);
				string data = (string) bf.Deserialize(stream);
				stream.Close();
				return JsonMapper.ToObject<CrossPromotionData>(data);
			}
			else {
				return new CrossPromotionData();
			}
		}
		public bool HasUserData() {
			return File.Exists(FilePath());
		}
		public static string FilePath() {
			return Application.persistentDataPath + "/" + fileName;
		}
	}
}


