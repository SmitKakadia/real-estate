namespace RealEstateAPI.Model
{
    public class TransactionModel
    {
        public int? TransactionID { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Type { get; set; }

        public int PropertyID { get; set; }

        public int SellerID { get; set; }

        public int BuyerID { get; set; }

    }
}
