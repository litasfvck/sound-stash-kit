using Microsoft.EntityFrameworkCore;
using SoundStashKit.Data.Configurations;
using SoundStashKit.Domain;

namespace SoundStashKit.Data
{
    public class SoundstashDbContext(DbContextOptions<SoundstashDbContext> options) : DbContext(options)
    {
        public DbSet<Sample> Samples => Set<Sample>();

        public DbSet<Tag> Tags => Set<Tag>();
        
        public DbSet<SampleTag> SampleTags => Set<SampleTag>();

        public DbSet<Pack> Packs => Set<Pack>();

        public DbSet<PackSample> PackSamples => Set<PackSample>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SampleConfiguration());
            modelBuilder.ApplyConfiguration(new PackConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new SampleTagConfiguration());
            modelBuilder.ApplyConfiguration(new PackSampleConfiguration());
        }
    }
}