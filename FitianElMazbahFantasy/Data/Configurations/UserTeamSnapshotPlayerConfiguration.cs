using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserTeamSnapshotPlayerConfiguration : IEntityTypeConfiguration<UserTeamSnapshotPlayer>
{
    public void Configure(EntityTypeBuilder<UserTeamSnapshotPlayer> builder)
    {
        builder.ToTable("UserTeamSnapshotPlayers");
        
        builder.HasKey(utsp => utsp.Id);
        
        builder.Property(utsp => utsp.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(utsp => utsp.SnapshotId)
            .IsRequired();
            
        builder.Property(utsp => utsp.PlayerId)
            .IsRequired();
            
        builder.Property(utsp => utsp.PlayerName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(utsp => utsp.Position)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(utsp => utsp.TeamName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(utsp => utsp.AddedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(utsp => utsp.SnapshotId);
        builder.HasIndex(utsp => utsp.PlayerId);
        builder.HasIndex(utsp => new { utsp.SnapshotId, utsp.PlayerId }).IsUnique();
        
        // Relationships
        builder.HasOne(utsp => utsp.Snapshot)
            .WithMany(uts => uts.PlayerSnapshots)
            .HasForeignKey(utsp => utsp.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(utsp => utsp.Player)
            .WithMany()
            .HasForeignKey(utsp => utsp.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}