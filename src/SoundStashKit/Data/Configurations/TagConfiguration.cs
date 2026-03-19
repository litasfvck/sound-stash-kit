using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStashKit.Domain;

namespace SoundStashKit.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags").HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name).HasColumnName("name");
    }
}