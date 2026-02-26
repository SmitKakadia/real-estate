using FluentValidation;
using RealEstateAPI.Model;

namespace RealEstateAPI.Validators
{
    public class TransactionValidator: AbstractValidator<TransactionModel>
    {
        public TransactionValidator()
        {
            RuleFor(transaction => transaction.Amount)
              .GreaterThan(0)
              .WithMessage("Amount must be greater than 0.");

            RuleFor(transaction => transaction.TransactionDate)
                .NotEmpty()
                .WithMessage("TransactionDate is required.")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("TransactionDate cannot be in the future.");

            RuleFor(transaction => transaction.Type)
                .NotEmpty()
                .WithMessage("Type is required.")
                .Must(type => new[] { "Credit", "Cash"}.Contains(type))
                .WithMessage("Type must be one of the following: Installment, Credit, Cash");

            RuleFor(transaction => transaction.PropertyID)
                .GreaterThan(0)
                .WithMessage("PropertyID must be greater than 0.");

            RuleFor(transaction => transaction.SellerID)
                .GreaterThan(0)
                .WithMessage("SellerID must be greater than 0.");

            RuleFor(transaction => transaction.BuyerID)
                .GreaterThan(0)
                .WithMessage("BuyerID must be greater than 0.");
        }
    }
}
