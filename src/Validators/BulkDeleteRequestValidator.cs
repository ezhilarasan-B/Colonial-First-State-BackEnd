using FluentValidation;
using UserDirectory.Api.Contracts.Common;

namespace UserDirectory.Api.Validators;

public class BulkDeleteRequestValidator : AbstractValidator<BulkDeleteRequest>
{
    public BulkDeleteRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one ID must be provided for deletion.")
            .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("All IDs must be valid positive integers.");
    }
}

