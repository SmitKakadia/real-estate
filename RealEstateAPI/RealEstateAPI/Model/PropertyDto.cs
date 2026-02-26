using Microsoft.AspNetCore.Http;

namespace RealEstateAPI.Model
{
    public class PropertyDto
    {
        public int PropertyID { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int UserID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public IFormFile Image { get; set; } // Main property cover Image
        public List<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();
    }
}
