
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace crosspromotion {
	public class CrossPromotion {
		private static CrossPromotionController instance;

		public static void Initialize(string domain) {
			if (instance == null) {
				instance = new CrossPromotionController(domain);
			}
		}

		public static void UpdateDomain(string domain) {
			if (GetInstance() != null)
				GetInstance().UpdateUrl(domain);
		}

		public static void ListenError(Action<Error, string> action) {
			if (GetInstance() != null)
				GetInstance().ListenError(action);
		}

		public static void ListenClaimReward(Action<string, int> action) {
			if (GetInstance() != null)
				GetInstance().ListenClaimReward(action);
		}

		public static void ListenInteractAction(Action<Interact, string> action) {
			if (GetInstance() != null)
				GetInstance().ListenInteractAction(action);
		}

		public static void ListenOnOpenAction(Action action) {
			if (GetInstance() != null)
				GetInstance().ListenOnOpenAction(action);
		}

		public static void ListenOnCloseAction(Action action) {
			if (GetInstance() != null) {
				GetInstance().ListenOnCloseAction(action);
			}
		}

		public static void Show() {
			if (GetInstance() != null) {
				GetInstance().Show();
			}
		}

		static CrossPromotionController GetInstance() {
			if (instance != null) {
				return instance;
			}
			else {
				Debug.LogError("Not initialize");
				return null;
			}
		}
	}
}


