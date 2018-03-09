namespace crosspromotion
{
	public class CrossPromotionItemData
	{
		public int id;
		public bool isClaimed = false;
		public bool isComplete = false;
		public bool invalid = false;

		public void SetComplete()
		{
			this.isComplete = true;
		}

		public void Claim()
		{
			isClaimed = true;
		}

		public void SetInvalid()
		{
			this.invalid = true;
		}
	}
}