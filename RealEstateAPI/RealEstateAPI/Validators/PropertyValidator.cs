using FluentValidation;
using RealEstateAPI.Model;


namespace RealEstateAPI.Validators
{

    public class PropertyValidator:AbstractValidator<PropertyModel>
    {
        public PropertyValidator()
        {
            RuleFor(property => property.Location)
                 .NotEmpty()
                 .WithMessage("Location is required.")
                 .MaximumLength(100)
                 .WithMessage("Location must not exceed 100 characters.");

            RuleFor(property => property.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(150)
                .WithMessage("Title must not exceed 150 characters.");

            RuleFor(property => property.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.");

            RuleFor(property => property.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(property => property.UserID)
                .GreaterThan(0)
                .WithMessage("UserID must be greater than 0.");

            RuleFor(property => property.Type)
                .NotEmpty()
                .WithMessage("Type is required.")
                .Must(type => new[] { "Buy", "Sell" }.Contains(type))
                .WithMessage("Type must be one of the following:Buy, Sell");

            RuleFor(property => property.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status => new[] { "Sold", "Pending" }.Contains(status))
                .WithMessage("Status must be one of the following: Sold, Pending");

            RuleFor(property => property.ImageUrl)
                .NotEmpty()
                .WithMessage("ImageUrl is required.");
                


        }
    }
}
