using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QTip.Application.Abstractions;
using QTip.Application.Features.Submissions;
using QTip.Domain.Entities;
using QTip.Infrastructure.Persistence;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public class SubmitTextCommandHandlerTests : TestBase
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

    private static ServiceProvider CreateHandlerServiceProvider()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        ServiceProvider provider = CreateServiceProvider(services =>
        {
            services.AddSingleton(options);
            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IValidator<SubmitTextCommand>, TestValidator>();
            services.AddScoped<IClassificationDetectionService, ClassificationDetectionService>();
        });
        return provider;
    }

    [Fact]
    public async Task Handle_TokenizesEmails_PersistsSubmissionAndClassifications()
    {
        const string originalText = "Hello john@example.com and jane@test.org.";

        using ServiceProvider provider = CreateHandlerServiceProvider();

        ApplicationDbContext dbContext = provider.GetRequiredService<ApplicationDbContext>();
        SubmitTextCommandHandler handler = new SubmitTextCommandHandler(
            provider.GetRequiredService<IApplicationDbContext>(),
            provider.GetRequiredService<IClassificationDetectionService>(),
            provider.GetRequiredService<ITokenGenerator>(),
            provider.GetRequiredService<IValidator<SubmitTextCommand>>());

        SubmitTextCommand command = new SubmitTextCommand(originalText);

        SubmitTextResult result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(2, result.DetectedPiiEmails);
        Assert.DoesNotContain("john@example.com", result.TokenizedText, StringComparison.Ordinal);
        Assert.DoesNotContain("jane@test.org", result.TokenizedText, StringComparison.Ordinal);

        int tokenCount = CountOccurrences(result.TokenizedText, "{{TKN-");
        Assert.Equal(2, tokenCount);

        List<Submission> submissions = await dbContext.Submissions.ToListAsync();
        List<ClassificationRecord> classifications = await dbContext.ClassificationRecords.ToListAsync();

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

        int count = 0;
        int index = 0;

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


