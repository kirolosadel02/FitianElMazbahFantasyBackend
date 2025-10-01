using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserTeamMatchweekPointsConfiguration : IEntityTypeConfiguration<UserTeamMatchweekPoints>
{
    public void Configure(EntityTypeBuilder<UserTeamMatchweekPoints> builder)
    {
        builder.ToTable("UserTeamMatchweekPoints");
        
        builder.HasKey(utmp => new { utmp.UserTeamId, utmp.MatchweekId });
        
        builder.Property(utmp => utmp.Points)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.Goals)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.Assists)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.CleanSheets)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.YellowCards)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.RedCards)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.Saves)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.Penalties)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(utmp => utmp.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(utmp => utmp.UpdatedAt);

        // Indexes
        builder.HasIndex(utmp => utmp.UserTeamId);
        builder.HasIndex(utmp => utmp.MatchweekId);
        builder.HasIndex(utmp => utmp.Points);
        
        // Relationships
        builder.HasOne(utmp => utmp.UserTeam)
            .WithMany(ut => ut.MatchweekPoints)
            .HasForeignKey(utmp => utmp.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(utmp => utmp.Matchweek)
            .WithMany(m => m.UserTeamMatchweekPoints)
            .HasForeignKey(utmp => utmp.MatchweekId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}