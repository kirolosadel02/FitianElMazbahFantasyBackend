using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class LeagueStandingConfiguration : IEntityTypeConfiguration<LeagueStanding>
{
    public void Configure(EntityTypeBuilder<LeagueStanding> builder)
    {
        builder.ToTable("LeagueStandings");
        
        builder.HasKey(ls => ls.Id);
        
        builder.Property(ls => ls.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(ls => ls.UserTeamId)
            .IsRequired();
            
        builder.Property(ls => ls.Position)
            .IsRequired();
            
        builder.Property(ls => ls.TotalPoints)
            .IsRequired();
            
        builder.Property(ls => ls.MatchWeek)
            .IsRequired();
            
        builder.Property(ls => ls.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(ls => ls.UpdatedAt);

        // Indexes
        builder.HasIndex(ls => ls.UserTeamId);
        builder.HasIndex(ls => ls.MatchWeek);
        builder.HasIndex(ls => ls.Position);
        builder.HasIndex(ls => new { ls.MatchWeek, ls.Position }).IsUnique();
        
        // Relationships
        builder.HasOne(ls => ls.UserTeam)
            .WithMany()
            .HasForeignKey(ls => ls.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}