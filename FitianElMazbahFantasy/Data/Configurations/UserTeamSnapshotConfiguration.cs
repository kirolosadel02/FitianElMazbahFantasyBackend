using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserTeamSnapshotConfiguration : IEntityTypeConfiguration<UserTeamSnapshot>
{
    public void Configure(EntityTypeBuilder<UserTeamSnapshot> builder)
    {
        builder.ToTable("UserTeamSnapshots");
        
        builder.HasKey(uts => uts.Id);
        
        builder.Property(uts => uts.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(uts => uts.UserTeamId)
            .IsRequired();
            
        builder.Property(uts => uts.MatchweekId)
            .IsRequired();
            
        builder.Property(uts => uts.TeamName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(uts => uts.SnapshotDate)
            .IsRequired();
            
        builder.Property(uts => uts.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(uts => uts.UserTeamId);
        builder.HasIndex(uts => uts.MatchweekId);
        builder.HasIndex(uts => new { uts.UserTeamId, uts.MatchweekId }).IsUnique();
        
        // Relationships
        builder.HasOne(uts => uts.UserTeam)
            .WithMany(ut => ut.TeamSnapshots)
            .HasForeignKey(uts => uts.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(uts => uts.Matchweek)
            .WithMany(m => m.UserTeamSnapshots)
            .HasForeignKey(uts => uts.MatchweekId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(uts => uts.PlayerSnapshots)
            .WithOne(utsp => utsp.Snapshot)
            .HasForeignKey(utsp => utsp.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}