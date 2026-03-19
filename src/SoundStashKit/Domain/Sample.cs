namespace SoundStashKit.Domain
{
    public class Sample
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public SampleType? Type { get; set; }

        public TimeSpan? Duration { get; set; }
        public int? Bpm { get; set; }
        public string? Key { get; set; }

        public Guid UserId { get; set; }

        public ICollection<SampleTag> Tags { get; set; } = [];
        public ICollection<PackSample> Packs { get; set; } = [];
    }

    public enum SampleType { OneShot, Loop, FX, Instrument }
}