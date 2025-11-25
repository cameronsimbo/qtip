namespace QTip.Domain.Entities;

public class ClassificationRecord
{
    public Guid Id { get; set; }

    public Guid SubmissionId { get; set; }

    public string Token { get; set; } = string.Empty;

    public string OriginalValue { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}


