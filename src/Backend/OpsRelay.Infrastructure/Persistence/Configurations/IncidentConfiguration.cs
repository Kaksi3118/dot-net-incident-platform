using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsRelay.Domain.Entities;
using OpsRelay.Domain.ValueObjects;

namespace OpsRelay.Infrastructure.Persistence.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");

        builder.HasKey(incident => incident.Id);

        builder.Property(incident => incident.Id)
            .ValueGeneratedNever();

        builder.Property(incident => incident.Title)
            .HasConversion(
                title => title.Value,
                value => IncidentTitle.Create(value))
            .HasMaxLength(IncidentTitle.MaxLength)
            .IsRequired();

        builder.Property(incident => incident.Description)
            .HasMaxLength(Incident.MaxDescriptionLength)
            .IsRequired();

        builder.Property(incident => incident.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(incident => incident.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(incident => incident.ReportedAt)
            .IsRequired();

        builder.Property(incident => incident.ResolvedAt);

        builder.Property(incident => incident.SlaDeadline)
            .IsRequired();

        builder.Property(incident => incident.AssignedTeamId)
            .HasConversion(
                teamId => teamId.Value,
                value => TeamId.Create(value))
            .HasMaxLength(TeamId.MaxLength)
            .IsRequired();

        builder.Property(incident => incident.RowVersion)
            .IsRowVersion();

        builder.Property<DateTimeOffset>("CreatedAt")
            .IsRequired();

        builder.Property<DateTimeOffset?>("UpdatedAt");

        builder.HasIndex(incident => incident.Status);
        builder.HasIndex(incident => incident.Priority);
        builder.HasIndex(incident => incident.ReportedAt);
    }
}
