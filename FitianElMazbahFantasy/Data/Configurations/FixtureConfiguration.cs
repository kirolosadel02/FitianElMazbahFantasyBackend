using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class FixtureConfiguration : IEntityTypeConfiguration<Fixture>
{
    public void Configure(EntityTypeBuilder<Fixture> builder)
    {
        builder.ToTable("Fixtures");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(f => f.Team1Id)
            .IsRequired();
            
        builder.Property(f => f.Team2Id)
            .IsRequired();
            
        builder.Property(f => f.MatchWeek)
            .IsRequired();
            
        builder.Property(f => f.MatchweekId)
            .IsRequired();
            
        builder.Property(f => f.MatchDate)
            .IsRequired();
        
        builder.Property(f => f.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(f => f.UpdatedAt);

        // Indexes
        builder.HasIndex(f => f.MatchWeek);
        builder.HasIndex(f => f.MatchweekId);
        builder.HasIndex(f => f.MatchDate);
        builder.HasIndex(f => f.IsCompleted);
        
        // Relationships
        builder.HasOne(f => f.Team1)
            .WithMany()
            .HasForeignKey(f => f.Team1Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(f => f.Team2)
            .WithMany()
            .HasForeignKey(f => f.Team2Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(f => f.Matchweek)
            .WithMany(m => m.Fixtures)
            .HasForeignKey(f => f.MatchweekId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(f => f.MatchResult)
            .WithOne(mr => mr.Fixture)
            .HasForeignKey<MatchResult>(mr => mr.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(f => f.MatchEvents)
            .WithOne(me => me.Fixture)
            .HasForeignKey(me => me.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Check constraint to prevent team playing against itself
        builder.HasCheckConstraint("CK_Fixture_DifferentTeams", "Team1Id <> Team2Id");
    }
}