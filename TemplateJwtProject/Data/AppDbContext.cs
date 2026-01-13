using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Artists> Artists { get; set; }
    public DbSet<Songs> Songs { get; set; }
    public DbSet<Top2000Entries> Top2000Entries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // RefreshToken configuratie
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        // Artist en Songs relatie (1-op-n) expliciet configureren
        builder.Entity<Artists>()
            .HasMany(a => a.Songs)
            .WithOne(s => s.Artist)
            .HasForeignKey(s => s.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite key for Top2000Entries: Position, Year, SongId
        builder.Entity<Top2000Entries>()
            .HasKey(t => new { t.Position, t.Year, t.SongId });

        // Relationship Top2000Entries -> Songs
        builder.Entity<Top2000Entries>()
            .HasOne(t => t.Song)
            .WithMany()
            .HasForeignKey(t => t.SongId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}