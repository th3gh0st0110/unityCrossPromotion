using System.Collections.Generic;

namespace crosspromotion {
	public class CrossPromotionData {
		public List<CrossPromotionItemData> items = new List<CrossPromotionItemData>();

		public CrossPromotionItemData GetItem(int itemId) {
			for (int i = 0; i < items.Count; i++) {
				if (items[i].id == itemId) {
					return items[i];
				}
			}
			CrossPromotionItemData item = new CrossPromotionItemData();
			item.id = itemId;
			items.Add(item);
			return item;
		}
	}
}