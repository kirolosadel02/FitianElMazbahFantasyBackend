using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        // Identity already configures Id, UserName, Email, and PasswordHash
        // We only need to configure our custom properties
            
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(u => u.UpdatedAt);

        // Relationships
        builder.HasOne(u => u.UserTeam)
            .WithOne(ut => ut.User)
            .HasForeignKey<UserTeam>(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}