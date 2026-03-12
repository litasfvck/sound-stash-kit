using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStashKit.Domain;

namespace SoundStashKit.Data.Configurations;

public class SampleConfiguration : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        builder.ToTable("samples").HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.Name).HasColumnName("name");
        builder.Property(s => s.FilePath).HasColumnName("filePath");
        builder.Property(s => s.Type).HasColumnName("type")
            .HasConversion<string>();
        builder.Property(s => s.Duration).HasColumnName("duration");
        builder.Property(s => s.Bpm).HasColumnName("bpm");
        builder.Property(s => s.Key).HasColumnName("key");
        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
    }
}