using Microsoft.EntityFrameworkCore;
using SoundStashKit.Data;
using SoundStashKit.Domain;
using SoundStashKit.DTO;
using SoundStashKit.Services.FakeUserService;

namespace SoundStashKit.Services.PackService
{
    public class PackService(SoundstashDbContext context,  IFakeUser fakeUser) : IPackService
    {
        /// <summary>
        /// Создать новый пак
        /// </summary>
        public async Task<PackDto> CreatePackAsync(CreatePackDto command, CancellationToken ct)
        {
            var pack = new Pack()
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                CoverImagePath = command.CoverImage?.FileName,
                UserId = fakeUser.UserId
            };

            await context.Packs.AddAsync(pack, ct);
            await context.SaveChangesAsync(ct);

            return new PackDto(pack.Id, pack.Name, pack.Description, pack.CoverImagePath, pack.UserId);
        }

        /// <summary>
        /// Удалить пак
        /// </summary>
        public async Task DeletePackAsync(Guid id, CancellationToken ct)
        {
            var packToDelete = await context.Packs.FindAsync([id], cancellationToken: ct);

            if (packToDelete != null)
            {
                context.Packs.Remove(packToDelete);
                await context.SaveChangesAsync(ct);
            }
            else
            {
                // throw new NotFoundException("Pack", id);
                throw new Exception($"not found pack {id}");
            }
        }

        /// <summary>
        /// Получить список паков
        /// </summary>
        public async Task<IEnumerable<PackDto>> GetAllPacksAsync(CancellationToken ct)
        {
            return await context.Packs
               .AsNoTracking()
               .Select(pack => new PackDto(
                       pack.Id,
                       pack.Name,
                       pack.Description,
                       pack.CoverImagePath,
                       pack.UserId
                   ))
               .ToListAsync(ct);
        }

        /// <summary>
        /// Получить пак по Id
        /// </summary>
        public async Task<PackDto?> GetPackByIdAsync(Guid id, CancellationToken ct)
        {
            var pack = await context.Packs
                .AsNoTracking()
                .FirstOrDefaultAsync(pack => pack.Id == id, ct);

            return pack == null
                // ? throw new NotFoundException("Pack", id)
                ? throw new Exception($"not found pack {id}")
                : new PackDto(pack.Id, pack.Name, pack.Description, pack.CoverImagePath, pack.UserId);
        }

        /// <summary>
        /// Обновить данные пака
        /// </summary>
        public async Task UpdatePackAsync(Guid id, UpdatePackDto command, CancellationToken ct)
        {
            var packToUpdate = await context.Packs.FindAsync([id], cancellationToken: ct) 
                ?? throw new Exception($"not found pack {id}");

            packToUpdate.Name = command.Name;
            packToUpdate.Description = command.Description;
            packToUpdate.CoverImagePath = command.CoverImage?.FileName;

            await context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Добавить семпл в пак
        /// </summary>
        public async Task AddSampleToPackAsync(Guid packId, Guid sampleId, CancellationToken ct)
        {
            var pack = await context.Packs
                .Include(s => s.Samples)
                .FirstOrDefaultAsync(p => p.Id == packId, ct)
                // ?? throw new NotFoundException("Pack", packId);
                ?? throw new Exception("unable to add sample to the pack");

            var sampleExists = await context.Samples.AnyAsync(s => s.Id == sampleId, ct);

            if (!sampleExists)
            {
                // throw new NotFoundException("Sample", sampleId);
                throw new Exception($"not found sample {sampleId}");
            }

            var alreadyAdded = pack.Samples.Any(ps => ps.SampleId == sampleId);

            if (alreadyAdded)
            {
                // throw new ConflictException($"Sample {sampleId} has been already added to this pack");
                throw new Exception($"Sample {sampleId} has been already added to this pack");
            }

            pack.Samples.Add(new PackSample
            {
                PackId = packId,
                SampleId = sampleId,
            });

            await context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Удалить сэмпл из пака
        /// </summary
        public async Task RemoveSampleFromPackAsync(Guid packId, Guid sampleId, CancellationToken ct)
        {
            var pack = await context.Packs
                .Include(p => p.Samples)
                .FirstOrDefaultAsync(p => p.Id == packId, ct)
                // ?? throw new NotFoundException("Pack", packId);
                ?? throw new Exception($"not found pack {packId}");

            var link = pack.Samples.FirstOrDefault(ps => ps.SampleId == sampleId) 
                // ?? throw new NotFoundException("Sample", sampleId);
                ?? throw new Exception($"not found sample {sampleId}");


            pack.Samples.Remove(link);

            await context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Получить все семплы в паке
        /// </summary>
        public async Task<IEnumerable<SampleDto>> GetSamplesInPackAsync(Guid packId, CancellationToken ct)
        {
            //var pack = await context.Packs
            //    .Include(p => p.Samples)
            //    .ThenInclude(ps => ps.Sample)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(p => p.Id == packId, ct)
            //    ?? throw new NotFoundException(packId);

            //return pack.Samples.Select(ps => new SampleDto(
            //    ps.Sample.Id,
            //    ps.Sample.Name,
            //    ps.Sample.FilePath
            //));

            var samples = await context.PackSamples
                .Where(ps => ps.PackId == packId)
                .Include(ps => ps.Sample)
                .AsNoTracking()
                .Select(ps => new SampleDto(
                    ps.Sample.Id, 
                    ps.Sample.Name, 
                    ps.Sample.FilePath,
                    ps.Sample.Type,
                    ps.Sample.Bpm,
                    ps.Sample.Key
                ))
                .ToListAsync(ct);

            if (samples.Count == 0)
            {
                // Could be: pack doesn't exist OR pack has no samples
                var packExists = await context.Packs.AnyAsync(p => p.Id == packId, ct);
                if (!packExists)
                    // throw new NotFoundException("Pack", packId);
                    throw new Exception($"not found pack {packId}");
            }

            return samples;
        }
    }
}
