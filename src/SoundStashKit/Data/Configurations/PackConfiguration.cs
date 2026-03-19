using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStashKit.Domain;

namespace SoundStashKit.Data.Configurations;

public class PackConfiguration : IEntityTypeConfiguration<Pack>
{
    public void Configure(EntityTypeBuilder<Pack> builder)
    {
        builder.ToTable("packs").HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name).HasColumnName("name");
        builder.Property(p => p.Description).HasColumnName("description");
        builder.Property(p => p.CoverImagePath).HasColumnName("coverImagePath");
        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
    }
}