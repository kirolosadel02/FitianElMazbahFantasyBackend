using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
{
    public void Configure(EntityTypeBuilder<UserTeam> builder)
    {
        builder.ToTable("UserTeams");
        
        builder.HasKey(ut => ut.Id);
        
        builder.Property(ut => ut.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(ut => ut.UserId)
            .IsRequired();
            
        builder.Property(ut => ut.TeamName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(ut => ut.TotalPoints)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(ut => ut.IsLocked)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(ut => ut.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(ut => ut.UpdatedAt);

        // Indexes
        builder.HasIndex(ut => ut.UserId).IsUnique(); // Make UserId unique - one team per user
        builder.HasIndex(ut => ut.TotalPoints);
        
        // Relationships
        builder.HasOne(ut => ut.User)
            .WithOne(u => u.UserTeam)
            .HasForeignKey<UserTeam>(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(ut => ut.UserTeamPlayers)
            .WithOne(utp => utp.UserTeam)
            .HasForeignKey(utp => utp.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}