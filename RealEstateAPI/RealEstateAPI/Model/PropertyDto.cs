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
        public string? ImageUrl { get; set; } // Existing image URL for reselling
        public IFormFile? Image { get; set; } // Main property cover Image (nullable for resell)
        public List<string> ExistingAdditionalImages { get; set; } = new List<string>();
        public List<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();
        public bool IsApproved { get; set; }
    }
}
