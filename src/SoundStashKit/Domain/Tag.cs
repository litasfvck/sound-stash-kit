namespace SoundStashKit.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<SampleTag> Samples { get; set; } = [];
    }
}