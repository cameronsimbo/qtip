namespace QTip.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; }

    public string TokenizedText { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}


