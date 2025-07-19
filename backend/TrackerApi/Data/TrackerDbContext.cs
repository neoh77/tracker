using Microsoft.EntityFrameworkCore;
using TrackerApi.Models;

namespace TrackerApi.Data;

public class TrackerDbContext : DbContext
{
    public TrackerDbContext(DbContextOptions<TrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<WeightHistory> WeightHistories { get; set; }
    public DbSet<FeedingHistory> FeedingHistories { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Animal entity
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToTable("animals");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Breed).HasColumnName("breed");
            entity.Property(e => e.Morph).HasColumnName("morph");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.LastFeedingDate).HasColumnName("last_feeding_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasIndex(e => e.Name).HasDatabaseName("idx_animals_name");
            entity.HasIndex(e => e.Breed).HasDatabaseName("idx_animals_breed");
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_animals_user_id");
            
            entity.HasOne(d => d.User)
                  .WithMany(p => p.Animals)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure WeightHistory entity
        modelBuilder.Entity<WeightHistory>(entity =>
        {
            entity.ToTable("weight_history");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnimalId).HasColumnName("animal_id");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.RecordedAt).HasColumnName("recorded_at");

            entity.HasOne(d => d.Animal)
                  .WithMany(p => p.WeightHistories)
                  .HasForeignKey(d => d.AnimalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AnimalId).HasDatabaseName("idx_weight_history_animal_id");
        });

        // Configure FeedingHistory entity
        modelBuilder.Entity<FeedingHistory>(entity =>
        {
            entity.ToTable("feeding_history");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnimalId).HasColumnName("animal_id");
            entity.Property(e => e.FeedingDate).HasColumnName("feeding_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Animal)
                  .WithMany(p => p.FeedingHistories)
                  .HasForeignKey(d => d.AnimalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AnimalId).HasDatabaseName("idx_feeding_history_animal_id");
            entity.HasIndex(e => e.FeedingDate).HasDatabaseName("idx_feeding_history_date");
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("idx_users_username");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_users_email");
        });
    }
}
