namespace crosspromotion
{
	public class CrossPromotionItemData
	{
		public int id;
		public bool isClaimed;
		public bool isComplete;

		public void SetComplete()
		{
			this.isComplete = true;
		}

		public void Claim()
		{
			isClaimed = true;
		}
	}
}