using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QTip.Domain.Entities;

namespace QTip.Infrastructure.Persistence.Configurations;

public sealed class ClassificationRecordConfiguration : IEntityTypeConfiguration<ClassificationRecord>
{
    public void Configure(EntityTypeBuilder<ClassificationRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.OriginalValue)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Tag)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.Tag);

        builder.HasOne<Submission>()
            .WithMany()
            .HasForeignKey(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


