using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserTeamPlayerConfiguration : IEntityTypeConfiguration<UserTeamPlayer>
{
    public void Configure(EntityTypeBuilder<UserTeamPlayer> builder)
    {
        builder.ToTable("UserTeamPlayers");
        
        builder.HasKey(utp => utp.Id);
        
        builder.Property(utp => utp.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(utp => utp.UserTeamId)
            .IsRequired();
            
        builder.Property(utp => utp.PlayerId)
            .IsRequired();
            
        builder.Property(utp => utp.AddedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(utp => utp.UserTeamId);
        builder.HasIndex(utp => utp.PlayerId);
        builder.HasIndex(utp => new { utp.UserTeamId, utp.PlayerId }).IsUnique();
        
        // Relationships
        builder.HasOne(utp => utp.UserTeam)
            .WithMany(ut => ut.UserTeamPlayers)
            .HasForeignKey(utp => utp.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(utp => utp.Player)
            .WithMany(p => p.UserTeamPlayers)
            .HasForeignKey(utp => utp.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}