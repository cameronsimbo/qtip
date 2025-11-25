using FluentValidation;

namespace QTip.Application.Features.Submissions;

public sealed class SubmitTextCommandValidator : AbstractValidator<SubmitTextCommand>
{
    public SubmitTextCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required.")
            .MaximumLength(10000)
            .WithMessage("Text must not exceed 10000 characters.");
    }
}


