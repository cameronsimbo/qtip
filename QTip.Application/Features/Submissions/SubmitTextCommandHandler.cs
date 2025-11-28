using System.Text;
using FluentValidation;
using MediatR;
using QTip.Application.Abstractions;
using QTip.Domain.Entities;

namespace QTip.Application.Features.Submissions;

public sealed class SubmitTextCommandHandler : IRequestHandler<SubmitTextCommand, SubmitTextResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IClassificationDetectionService _classificationDetectionService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IValidator<SubmitTextCommand> _validator;

    public SubmitTextCommandHandler(
        IApplicationDbContext dbContext,
        IClassificationDetectionService classificationDetectionService,
        ITokenGenerator tokenGenerator,
        IValidator<SubmitTextCommand> validator)
    {
        _dbContext = dbContext;
        _classificationDetectionService = classificationDetectionService;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
    }

    public async Task<SubmitTextResult> Handle(SubmitTextCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        string text = request.Text;

        List<DetectedClassification> detectedClassifications = _classificationDetectionService
            .Detect(text)
            .OrderBy(x => x.StartIndex)
            .ToList();

        Guid submissionId = Guid.NewGuid();
        Submission submission = new Submission
        {
            Id = submissionId,
            TokenizedText = string.Empty,
            CreatedAtUtc = DateTime.UtcNow
        };

        List<ClassificationRecord> classificationRecords = new List<ClassificationRecord>(detectedClassifications.Count);

        StringBuilder builder = new StringBuilder(text.Length);
        int currentIndex = 0;

        foreach (DetectedClassification classification in detectedClassifications)
        {
            if (classification.StartIndex < currentIndex)
            {
                // Skip overlapping or invalid match to avoid corrupting output.
                continue;
            }

            int lengthToCopy = classification.StartIndex - currentIndex;
            if (lengthToCopy > 0)
            {
                builder.Append(text.AsSpan(currentIndex, lengthToCopy));
            }

            string token = _tokenGenerator.GenerateToken();
            builder.Append(token);

            classificationRecords.Add(new ClassificationRecord
            {
                Id = Guid.NewGuid(),
                SubmissionId = submissionId,
                Token = token,
                OriginalValue = classification.Value,
                Tag = classification.Tag,
                CreatedAtUtc = DateTime.UtcNow
            });

            currentIndex = classification.StartIndex + classification.Length;
        }

        if (currentIndex < text.Length)
        {
            builder.Append(text.AsSpan(currentIndex, text.Length - currentIndex));
        }

        submission.TokenizedText = builder.ToString();

        _dbContext.Submissions.Add(submission);

        if (classificationRecords.Count > 0)
        {
            _dbContext.ClassificationRecords.AddRange(classificationRecords);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitTextResult(submission.TokenizedText, classificationRecords.Count);
    }
}


