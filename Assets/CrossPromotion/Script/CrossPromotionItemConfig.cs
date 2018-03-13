using System;

namespace crosspromotion {
	public class CrossPromotionItemConfig {
		public int id { get; set; }
		public string name { get; set; }
		public string desctiption { get; set; }
		public string icon { get; set; }
		public int reward { get; set; }
		public string rewardType { get; set; }
		public int require { get; set; }
		public string androidLink { get; set; }
		public string iosLink { get; set; }
		public bool enable { get; set; }
		public string GetAppId() {
#if UNITY_IPHONE
				return iosLink;
#else
				return androidLink;
#endif
		}

	}

	public enum ConditionType {
		INSTALL_APP
	}
}