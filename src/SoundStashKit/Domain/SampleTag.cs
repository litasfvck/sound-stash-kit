namespace SoundStashKit.Domain
{
    public class SampleTag
    {
        public Guid SampleId { get; set; }
        public Sample? Sample { get; set; }

        public Guid TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}