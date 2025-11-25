using System.Text;
using MediatR;
using QTip.Application.Abstractions;
using QTip.Application.Common;
using QTip.Domain.Entities;

namespace QTip.Application.Features.Submissions;

public sealed class SubmitTextCommandHandler : IRequestHandler<SubmitTextCommand, SubmitTextResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailDetectionService _emailDetectionService;
    private readonly ITokenGenerator _tokenGenerator;

    public SubmitTextCommandHandler(
        IApplicationDbContext dbContext,
        IEmailDetectionService emailDetectionService,
        ITokenGenerator tokenGenerator)
    {
        _dbContext = dbContext;
        _emailDetectionService = emailDetectionService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<SubmitTextResult> Handle(SubmitTextCommand request, CancellationToken cancellationToken)
    {
        var text = request.Text;

        var detectedEmails = _emailDetectionService
            .Detect(text)
            .OrderBy(x => x.StartIndex)
            .ToList();

        var submissionId = Guid.NewGuid();
        var submission = new Submission
        {
            Id = submissionId,
            TokenizedText = string.Empty,
            CreatedAtUtc = DateTime.UtcNow
        };

        var classificationRecords = new List<ClassificationRecord>(detectedEmails.Count);

        var builder = new StringBuilder(text.Length);
        var currentIndex = 0;

        foreach (var email in detectedEmails)
        {
            if (email.StartIndex < currentIndex)
            {
                // Skip overlapping or invalid match to avoid corrupting output.
                continue;
            }

            var lengthToCopy = email.StartIndex - currentIndex;
            if (lengthToCopy > 0)
            {
                builder.Append(text.AsSpan(currentIndex, lengthToCopy));
            }

            var token = _tokenGenerator.GenerateToken();
            builder.Append(token);

            classificationRecords.Add(new ClassificationRecord
            {
                Id = Guid.NewGuid(),
                SubmissionId = submissionId,
                Token = token,
                OriginalValue = email.Value,
                Tag = PiiTags.PiiEmailTag,
                CreatedAtUtc = DateTime.UtcNow
            });

            currentIndex = email.StartIndex + email.Length;
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


