using SoundStashKit.DTO;

namespace SoundStashKit.Services.SampleService
{
    public interface ISampleService
    {
        Task<SampleDto> CreateSampleAsync(CreateSampleDto command, CancellationToken ct);
        Task<SampleDto?> GetSampleByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<SampleDto>> GetAllSamplesAsync(CancellationToken ct);
        Task UpdateSampleAsync(Guid id, UpdateSampleDto command, CancellationToken ct);
        Task DeleteSampleAsync(Guid id, CancellationToken ct);
    }
}
