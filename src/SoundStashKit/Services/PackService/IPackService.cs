using SoundStashKit.DTO;

namespace SoundStashKit.Services.PackService
{
    public interface IPackService
    {
        Task<PackDto> CreatePackAsync(CreatePackDto command, CancellationToken ct);
        Task<PackDto?> GetPackByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<PackDto>> GetAllPacksAsync(CancellationToken ct);
        Task UpdatePackAsync(Guid id, UpdatePackDto command, CancellationToken ct);
        Task DeletePackAsync(Guid id, CancellationToken ct);
        Task AddSampleToPackAsync(Guid packId, Guid sampleId, CancellationToken ct);
        Task RemoveSampleFromPackAsync(Guid packId, Guid sampleId, CancellationToken ct);
        Task <IEnumerable<SampleDto>> GetSamplesInPackAsync(Guid packId, CancellationToken ct);
    }
}