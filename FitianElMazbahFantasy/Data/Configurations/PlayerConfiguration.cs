using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.Position)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(p => p.TeamId)
            .IsRequired();
            
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(p => p.UpdatedAt);

        // Indexes
        builder.HasIndex(p => p.Position);
        builder.HasIndex(p => p.TeamId);
        
        // Relationships
        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(p => p.UserTeamPlayers)
            .WithOne(utp => utp.Player)
            .HasForeignKey(utp => utp.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(p => p.MatchEvents)
            .WithOne(me => me.Player)
            .HasForeignKey(me => me.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}