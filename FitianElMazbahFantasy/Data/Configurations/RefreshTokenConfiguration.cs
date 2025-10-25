using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        
        builder.HasKey(rt => rt.Id);
        
        builder.Property(rt => rt.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(rt => rt.UserId)
            .IsRequired();
            
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
            
        builder.Property(rt => rt.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(rt => rt.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(rt => rt.RevokedAt);

        // Indexes
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.UserId);
        builder.HasIndex(rt => rt.ExpiresAt);
        builder.HasIndex(rt => rt.IsRevoked);
        
        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}