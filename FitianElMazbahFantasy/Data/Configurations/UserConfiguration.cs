using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(u => u.UpdatedAt);

        // Unique constraints
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // Relationships
        builder.HasOne(u => u.UserTeam)
            .WithOne(ut => ut.User)
            .HasForeignKey<UserTeam>(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}