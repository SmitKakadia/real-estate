using FluentValidation;
using RealEstateAPI.Model;

namespace RealEstateAPI.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            

            RuleFor(user => user.UserName)
                .NotEmpty()
                .WithMessage("UserName is required.")
                .MinimumLength(3)
                .WithMessage("UserName must be at least 3 characters long.")
                .MaximumLength(50)
                .WithMessage("UserName must not exceed 50 characters.");

            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email is not in a valid format.");

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
                //.MinimumLength(8)
                //.WithMessage("Password must be at least 8 characters long.")
                //.Matches("[A-Z]")
                //.WithMessage("Password must contain at least one uppercase letter.")
                //.Matches("[a-z]")
                //.WithMessage("Password must contain at least one lowercase letter.")
                //.Matches("[0-9]")
                //.WithMessage("Password must contain at least one digit.")
                //.Matches("[^a-zA-Z0-9]")
                //.WithMessage("Password must contain at least one special character.");

            RuleFor(c => c.PhoneNo).NotEmpty().NotNull().Length(10).WithMessage("Enter valid PhoneNo");

            RuleFor(user => user.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role => new[] { "Buyer", "Seller"}.Contains(role))
                .WithMessage("Role must be one of the following: Buyer, Seller.");

        }

    }
}
