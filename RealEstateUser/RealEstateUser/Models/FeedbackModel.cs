using System.ComponentModel.DataAnnotations;

namespace RealEstateUser.Models
{
    public class FeedbackModel
    {
        public int FeedbackID { get; set; }

        [Required(ErrorMessage = "Please provide your feedback.")]
        public string Description { get; set; }

        
        public int UserID { get; set; }

     
        [Required]
        public int PropertyID { get; set; }

      
    }
}
