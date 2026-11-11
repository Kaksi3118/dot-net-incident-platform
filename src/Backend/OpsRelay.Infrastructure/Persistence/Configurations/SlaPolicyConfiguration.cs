using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.Enums;

namespace OpsRelay.Infrastructure.Persistence.Configurations;

public class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
{
    private static readonly DateTimeOffset SeededAt =
        new(2026, 11, 11, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<SlaPolicy> builder)
    {
        builder.ToTable("SlaPolicies");

        builder.HasKey(policy => policy.Id);

        builder.Property(policy => policy.Id)
            .ValueGeneratedNever();

        builder.Property(policy => policy.Name)
            .HasMaxLength(SlaPolicy.MaxNameLength)
            .IsRequired();

        builder.Property(policy => policy.ResponseTimeHours)
            .IsRequired();

        builder.Property(policy => policy.ResolutionTimeHours)
            .IsRequired();

        builder.Property(policy => policy.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property<DateTimeOffset>("CreatedAt")
            .IsRequired();

        builder.Property<DateTimeOffset?>("UpdatedAt");

        builder.HasIndex(policy => policy.Priority)
            .IsUnique();

        builder.HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Low Priority",
                ResponseTimeHours = 8,
                ResolutionTimeHours = 48,
                Priority = Priority.Low,
                CreatedAt = SeededAt
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Medium Priority",
                ResponseTimeHours = 4,
                ResolutionTimeHours = 24,
                Priority = Priority.Medium,
                CreatedAt = SeededAt
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "High Priority",
                ResponseTimeHours = 2,
                ResolutionTimeHours = 8,
                Priority = Priority.High,
                CreatedAt = SeededAt
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Critical Priority",
                ResponseTimeHours = 1,
                ResolutionTimeHours = 4,
                Priority = Priority.Critical,
                CreatedAt = SeededAt
            });
    }
}
