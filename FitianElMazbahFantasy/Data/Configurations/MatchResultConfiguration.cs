using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class MatchResultConfiguration : IEntityTypeConfiguration<MatchResult>
{
    public void Configure(EntityTypeBuilder<MatchResult> builder)
    {
        builder.ToTable("MatchResults");
        
        builder.HasKey(mr => mr.Id);
        
        builder.Property(mr => mr.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(mr => mr.FixtureId)
            .IsRequired();
            
        builder.Property(mr => mr.FinalScore)
            .IsRequired()
            .HasMaxLength(10); // e.g., "10-9" should be enough
            
        builder.Property(mr => mr.Team1Score)
            .IsRequired();
            
        builder.Property(mr => mr.Team2Score)
            .IsRequired();
            
        builder.Property(mr => mr.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(mr => mr.UpdatedAt);

        // Indexes
        builder.HasIndex(mr => mr.FixtureId).IsUnique();
        
        // Relationships
        builder.HasOne(mr => mr.Fixture)
            .WithOne(f => f.MatchResult)
            .HasForeignKey<MatchResult>(mr => mr.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}