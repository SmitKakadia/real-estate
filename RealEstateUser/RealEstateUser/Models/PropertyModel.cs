using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateUser.Models
{
  
    public class PropertyModel
    {
        public int? PropertyID { get; set; } 

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location must be less than 100 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must be less than 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000000, ErrorMessage = "Price must be between 1 and 100,000,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Property Type is required.")]
        [StringLength(50, ErrorMessage = "Type must be less than 50 characters.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status must be less than 50 characters.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; }

        public string CreatedBy { get; set; }
        public string BuyerName { get; set; }
        public System.Collections.Generic.List<string> AdditionalImages { get; set; } = new System.Collections.Generic.List<string>();
    }

    public class PropertyDto
    {
        public int? PropertyID { get; set; } 

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location must be less than 100 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must be less than 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000000, ErrorMessage = "Price must be between 1 and 100,000,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Property Type is required.")]
        [StringLength(50, ErrorMessage = "Type must be less than 50 characters.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status must be less than 50 characters.")]
        public string Status { get; set; }

       
        [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile ImageUrl { get; set; }

        public System.Collections.Generic.List<IFormFile> AdditionalImages { get; set; } = new System.Collections.Generic.List<IFormFile>();
    }
}
