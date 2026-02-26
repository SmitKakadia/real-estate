using System.Collections.Generic;

namespace RealEstateAPI.Model
{
    public class PropertyModel
    {
        public int PropertyID { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int UserID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public List<string> AdditionalImages { get; set; } = new List<string>();

        // New fields to capture the username of the property creator and buyer
        public string CreatedBy { get; set; }
        public string BuyerName { get; set; }
    }
}
