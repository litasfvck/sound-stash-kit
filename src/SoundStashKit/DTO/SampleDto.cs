using SoundStashKit.Domain;

namespace SoundStashKit.DTO
{
    public record SampleDto(
        Guid Id,
        string Name,
        string FilePath,     // URL из MinIO
        SampleType? Type,
        int? Bpm,
        string? Key);

    public record CreateSampleDto(
        string Name,
        IFormFile File,
        SampleType? Type,
        int? Bpm,
        string? Key);

    public record UpdateSampleDto(
        string Name,
        SampleType? Type,
        int? Bpm,
        string? Key);  
}