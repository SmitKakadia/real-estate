using FluentValidation;
using RealEstateAPI.Model;

namespace RealEstateAPI.Validators
{
    public class FeedbackValidator:AbstractValidator<FeedbackModel>
    {
        public FeedbackValidator()
        {
            RuleFor(feedback => feedback.Description)
               .NotEmpty()
               .WithMessage("Description is required.")
               .MaximumLength(500)
               .WithMessage("Description must not exceed 500 characters.");

            RuleFor(feedback => feedback.UserID)
                .GreaterThan(0)
                .WithMessage("UserID must be greater than 0.");

            RuleFor(feedback => feedback.PropertyID)
                .GreaterThan(0)
                .WithMessage("PropertyID must be greater than 0."); 
        }
    }
}
