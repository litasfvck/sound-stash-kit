namespace SoundStashKit.Domain
{
    public class PackSample
    {
        public Guid PackId { get; set; }
        public Pack? Pack { get; set; }

        public Guid SampleId { get; set; }
        public Sample? Sample { get; set; }

        public DateTime AddedAt {get; set; } = DateTime.UtcNow;
    }
}