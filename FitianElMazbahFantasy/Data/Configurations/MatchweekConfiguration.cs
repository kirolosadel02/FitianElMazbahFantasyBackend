using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class MatchweekConfiguration : IEntityTypeConfiguration<Matchweek>
{
    public void Configure(EntityTypeBuilder<Matchweek> builder)
    {
        builder.ToTable("Matchweeks");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(m => m.WeekNumber)
            .IsRequired();
            
        builder.Property(m => m.DeadlineDate)
            .IsRequired();
            
        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(m => m.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(m => m.UpdatedAt);

        // Indexes
        builder.HasIndex(m => m.WeekNumber).IsUnique();
        builder.HasIndex(m => m.DeadlineDate);
        builder.HasIndex(m => m.IsActive);
        builder.HasIndex(m => m.IsCompleted);
        
        // Relationships
        builder.HasMany(m => m.Fixtures)
            .WithOne(f => f.Matchweek)
            .HasForeignKey(f => f.MatchweekId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(m => m.UserTeamMatchweekPoints)
            .WithOne(utmp => utmp.Matchweek)
            .HasForeignKey(utmp => utmp.MatchweekId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.UserTeamSnapshots)
            .WithOne(uts => uts.Matchweek)
            .HasForeignKey(uts => uts.MatchweekId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}