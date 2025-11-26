using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Application.Features.Submissions;
using QTip.Domain.Entities;
using QTip.Infrastructure.Persistence;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public class SubmitTextCommandHandlerTests
{
    private sealed class TestValidator : AbstractValidator<SubmitTextCommand>
    {
        public TestValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .MaximumLength(10000);
        }
    }

    private static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_TokenizesEmails_PersistsSubmissionAndClassifications()
    {
        const string originalText = "Hello john@example.com and jane@test.org.";

        using var dbContext = CreateInMemoryDbContext();

        var handler = new SubmitTextCommandHandler(
            dbContext,
            new EmailDetectionService(),
            new TokenGenerator(),
            new TestValidator());

        var command = new SubmitTextCommand(originalText);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(2, result.DetectedEmailCount);
        Assert.DoesNotContain("john@example.com", result.TokenizedText, StringComparison.Ordinal);
        Assert.DoesNotContain("jane@test.org", result.TokenizedText, StringComparison.Ordinal);

        var tokenCount = CountOccurrences(result.TokenizedText, "{{TKN-");
        Assert.Equal(2, tokenCount);

        var submissions = await dbContext.Submissions.ToListAsync();
        var classifications = await dbContext.ClassificationRecords.ToListAsync();

        Assert.Single(submissions);
        Assert.Equal(2, classifications.Count);

        Assert.Contains(classifications, x => x.OriginalValue == "john@example.com");
        Assert.Contains(classifications, x => x.OriginalValue == "jane@test.org");

        Assert.All(
            classifications,
            record => Assert.Equal(submissions[0].Id, record.SubmissionId));
    }

    private static int CountOccurrences(string text, string value)
    {
        if (text.Length == 0 || value.Length == 0)
        {
            return 0;
        }

        var count = 0;
        var index = 0;

        while (true)
        {
            index = text.IndexOf(value, index, StringComparison.Ordinal);
            if (index < 0)
            {
                break;
            }

            count++;
            index += value.Length;
        }

        return count;
    }
}


