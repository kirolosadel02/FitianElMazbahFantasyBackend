using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Data.Configurations;

public class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
    public void Configure(EntityTypeBuilder<MatchEvent> builder)
    {
        builder.ToTable("MatchEvents");
        
        builder.HasKey(me => me.Id);
        
        builder.Property(me => me.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(me => me.FixtureId)
            .IsRequired();
            
        builder.Property(me => me.PlayerId)
            .IsRequired();
            
        builder.Property(me => me.EventType)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(me => me.Points)
            .IsRequired();
            
        builder.Property(me => me.Minute);
        
        builder.Property(me => me.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(me => me.FixtureId);
        builder.HasIndex(me => me.PlayerId);
        builder.HasIndex(me => me.EventType);
        
        // Relationships
        builder.HasOne(me => me.Fixture)
            .WithMany(f => f.MatchEvents)
            .HasForeignKey(me => me.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(me => me.Player)
            .WithMany(p => p.MatchEvents)
            .HasForeignKey(me => me.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}