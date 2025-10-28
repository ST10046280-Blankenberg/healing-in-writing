using HealingInWriting.Domain.Users;
using HealingInWriting.Domain.Books;
using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Data;

/// <summary>
/// Database context for the Healing In Writing application.
/// Inherits from IdentityDbContext to provide ASP.NET Core Identity tables
/// and user management functionality.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    // TODO [Future]: Add DbSet properties for other domain entities when needed
    // public DbSet<UserProfile> UserProfiles { get; set; }
    // public DbSet<Story> Stories { get; set; }
    // public DbSet<Event> Events { get; set; }
    // public DbSet<Donation> Donations { get; set; }
    // public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize Identity table names (optional - remove if you prefer default names)
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        // TODO [Future]: Add model configurations for other entities here
        // Example:
        // builder.Entity<Story>()
        //     .HasOne(s => s.User)
        //     .WithMany(u => u.Stories)
        //     .HasForeignKey(s => s.UserId);

        // Configure ImageLinks as an owned type of Book
        builder.Entity<Book>().OwnsOne(b => b.ImageLinks);

        // Persist each ISBN entry properly instead of the single-capacity blob EF creates by default.
        builder.Entity<Book>().OwnsMany(b => b.IndustryIdentifiers, ii =>
        {
            ii.WithOwner().HasForeignKey("BookId");
            ii.Property<int>("Id").ValueGeneratedOnAdd();
            ii.HasKey("Id");
            ii.HasIndex("BookId");
            ii.Property(i => i.Type).HasColumnName("Type");
            ii.Property(i => i.Identifier).HasColumnName("Identifier");
            ii.ToTable("BookIndustryIdentifiers");
        });
        
        // Event relationships
        builder.Entity<Event>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Event>()
            .HasOne(e => e.Address)
            .WithMany()
            .HasForeignKey(e => e.AddressId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Many-to-many: Events and Tags
        builder.Entity<Event>()
            .HasMany(e => e.EventTags)
            .WithMany()
            .UsingEntity(j => j.ToTable("EventTags"));
        
        // UserProfile relationship
        builder.Entity<UserProfile>()
            .HasOne(up => up.User)
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        

    }
}
