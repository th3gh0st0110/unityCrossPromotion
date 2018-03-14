
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace crosspromotion {
	public class CrossPromotion {
		static string fileName = "crossPromotion.data";
		private static UniWebView _webView;
		private static CrossPromotionConfig crossPromotionConfig = new CrossPromotionConfig();
		private static Action<Error, string> errorAction = delegate (Error error, string s) { };
		private static Action<string, int> rewardAction = delegate (string s, int i) { };
		private static Action onOpen = delegate { };
		private static Action onClose = delegate { };
		private static Action<Interact, CrossPromotionItemConfig> interact = delegate (Interact interact1, CrossPromotionItemConfig item) { };
		private static CrossPromotionData data;

		public static void ListenError(Action<Error, string> action) {
			errorAction += action;
		}

		public static void ListenClaimReward(Action<string, int> action) {
			rewardAction += action;
		}

		public static void ListenInteractAction(Action<Interact, CrossPromotionItemConfig> action) {
			interact += action;
		}

		public static void ListenOnOpenAction(Action action) {
			onOpen += action;
		}

		public static void ListenOnCloseAction(Action action) {
			onClose += action;
		}

		public static void Show(string domain) {
			Action<bool> action = delegate (bool hasInternet) {
				if (hasInternet) {

					Action<bool, string> loadConfigCallBack = delegate (bool success, string s) {
						if (success) {
							data = LoadUserData();
							CheckCrosspromotionStatus(data, false);
							OpenWebview(domain);
						}
						else {
							errorAction.Invoke(Error.LoadConfig, s);
						}
					};
					Coroutiner.StartCoroutine(LoadConfig(domain, loadConfigCallBack));

				}
				else {
					errorAction.Invoke(Error.NoInternet, "");
				}
			};
			Coroutiner.StartCoroutine(Utils.CheckInternetConnection(action, domain));
		}

		static IEnumerator LoadConfig(string domain, Action<bool, string> callback) {
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

		static void CheckCrosspromotionStatus(CrossPromotionData data, bool isInit) {

			List<CrossPromotionItemConfig> items = crossPromotionConfig.GetList();
			for (int i = 0; i < items.Count; i++) {
				CrossPromotionItemConfig item = items[i];
				if (item.enable || isInit)
					CheckCrosspromotionStatus(data, item, isInit);
			}

		}

		static void CheckCrosspromotionStatus(CrossPromotionData data, CrossPromotionItemConfig item, bool isInit) {
			string id = Utils.GetValueFromUrl(item.GetLink(), "id");
			if (Utils.checkPackageAppIsPresent(id)) {
				if (isInit) {
					data.GetItem(item.id).SetInvalid();
				}
				else {
					if (data.GetItem(item.id).isValid)
						data.GetItem(item.id).SetComplete();
				}
			}
		}
		static void OpenWebview(string domain) {
			if (_webView != null) {
				return;
			}
			_webView = CreateWebView();
			string t = string.Format("\"{0}\"", JsonMapper.ToJson(data));
			_webView.url = domain + "/?data=" + JsonMapper.ToJson(data);
			_webView.CleanCache();
			_webView.backButtonEnable = true;
			int bottomInset = UniWebViewHelper.screenHeight;
			_webView.insets = new UniWebViewEdgeInsets(0, 0, 0, 0);
			_webView.OnReceivedMessage += OnReceivedMessage;

			_webView.Load();
			_webView.Show();
			onOpen.Invoke();
		}
		static void OnReceivedMessage(UniWebView webView, UniWebViewMessage message) {

			if (message.path == "close") {
				Close(webView);
			}
			if (message.path == "reward") {
				int id = 0;
				int value = 0;
				string type = message.args["type"];
				if (int.TryParse(message.args["value"], out value) & int.TryParse(message.args["id"], out id)) {
					CrossPromotionItemConfig item = crossPromotionConfig.GetCrossPromotionItemConfig(id);
					data.GetItem(id).Claim();
					Save();
					rewardAction.Invoke(type, value);
					interact.Invoke(Interact.ClaimReward, item);
				}

			}
			if (message.path == "OpenApp") {
				Close(webView);
				try {
					string id = message.args["id"];
					Coroutiner.StartCoroutine(DelayOpenApp(crossPromotionConfig.GetCrossPromotionItemConfig(int.Parse(id))));

				}
				catch (Exception e) {
					errorAction.Invoke(Error.LoadConfig, e.ToString());
				}

			}
		}

		private static IEnumerator DelayOpenApp(CrossPromotionItemConfig item) {
			yield return null;
			yield return null;
			interact.Invoke(Interact.InstallApp, item);
			Application.OpenURL(item.GetLink());
		}
		static void Close(UniWebView webView) {
			GameObject.Destroy(webView);
			_webView = null;
			onClose.Invoke();
		}
		static UniWebView CreateWebView() {
			var webViewGameObject = GameObject.Find("WebView");
			if (webViewGameObject == null) {
				webViewGameObject = new GameObject("WebView");
			}

			var webView = webViewGameObject.AddComponent<UniWebView>();

			webView.toolBarShow = true;
			return webView;
		}
		private static CrossPromotionData LoadUserData() {
			if (HasUserData()) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream stream = new FileStream(FilePath(), FileMode.Open);
				string data = (string) bf.Deserialize(stream);
				stream.Close();
				return JsonMapper.ToObject<CrossPromotionData>(data);
			}
			else {
				CrossPromotionData ret = new CrossPromotionData();
				CheckCrosspromotionStatus(ret, true);
				Save();
				return ret;
			}
		}
		private static bool HasUserData() {
			return File.Exists(FilePath());
		}
		private static string FilePath() {
			return Application.persistentDataPath + "/" + fileName;
		}
		private static void Save() {
			if (data != null) {
				string text = JsonMapper.ToJson(data);
				BinaryFormatter bf = new BinaryFormatter();
				FileStream stream = new FileStream(FilePath(), FileMode.Create);
				bf.Serialize(stream, text);
				stream.Close();
			}
		}
	}
}


