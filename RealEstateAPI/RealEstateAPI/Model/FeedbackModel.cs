namespace RealEstateAPI.Model
{
    public class FeedbackModel
    {
        public int FeedbackID { get; set; }

        public string Description { get; set; }

        public int UserID { get; set; }

        public int PropertyID { get; set; }

    }
}
