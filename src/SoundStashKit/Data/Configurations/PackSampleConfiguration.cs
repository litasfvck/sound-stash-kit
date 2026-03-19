using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStashKit.Domain;

namespace SoundStashKit.Data.Configurations;

public class PackSampleConfiguration : IEntityTypeConfiguration<PackSample>
{
    public void Configure(EntityTypeBuilder<PackSample> builder)
    {
        builder.ToTable("pack_samples").HasKey(ps => new {ps.PackId, ps.SampleId});

        builder.HasOne(ps => ps.Pack)
            .WithMany(s => s.Samples)
            .HasForeignKey(s => s.PackId);

        builder.HasOne(ps => ps.Sample)
            .WithMany(p => p.Packs)
            .HasForeignKey(p => p.SampleId);
    }
}