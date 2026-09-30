using FluentValidation;
using UserDirectory.Api.Contracts.Client.Requests;

namespace UserDirectory.Api.Validators;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required.")
            .Length(2, 150).WithMessage("Client name must be between 2 and 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number must contain exactly 10 digits.")
            .Matches(@"^\d{10}$").WithMessage("Phone number must contain exactly 10 digits.");

        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company name is required.");

        RuleFor(x => x.StaffId)
            .GreaterThan(0).WithMessage("A valid assigned staff member must be selected.");
    }
}

public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required.")
            .Length(2, 150).WithMessage("Client name must be between 2 and 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number must contain exactly 10 digits.")
            .Matches(@"^\d{10}$").WithMessage("Phone number must contain exactly 10 digits.");

        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company name is required.");

        RuleFor(x => x.StaffId)
            .GreaterThan(0).WithMessage("A valid assigned staff member must be selected.");
    }
}

