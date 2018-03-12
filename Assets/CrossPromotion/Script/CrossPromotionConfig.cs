using System.Collections.Generic;

namespace crosspromotion {
	public class CrossPromotionConfig {
		public Dictionary<string, CrossPromotionItemConfig> items = new Dictionary<string, CrossPromotionItemConfig>();


		public CrossPromotionItemConfig GetCrossPromotionItemConfig(int id)
		{
			foreach (KeyValuePair<string, CrossPromotionItemConfig> valuePair in items)
			{
				if (valuePair.Value.id == id)
				{
					return valuePair.Value;
				}
			}
			return null;
		}

		public List<CrossPromotionItemConfig> GetList() {
			List<CrossPromotionItemConfig> r = new List<CrossPromotionItemConfig>();
			foreach (KeyValuePair<string, CrossPromotionItemConfig> valuePair in items) {
				r.Add(valuePair.Value);
			}
			return r;
		}

		public string GetAppId(int id)
		{
			CrossPromotionItemConfig itemConfig = GetCrossPromotionItemConfig(id);
			if (itemConfig != null)
			{
#if UNITY_IPHONE
				return itemConfig.iosLink;
#else
				return itemConfig.androidLink;
#endif

			}
			else
			{
				return "invalid";
			}
		}
	}
}