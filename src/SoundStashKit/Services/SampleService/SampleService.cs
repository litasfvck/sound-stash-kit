using Microsoft.EntityFrameworkCore;
using SoundStashKit.Data;
using SoundStashKit.Domain;
using SoundStashKit.DTO;
using SoundStashKit.Services.FakeUserService;

namespace SoundStashKit.Services.SampleService
{
    public class SampleService(SoundstashDbContext context, IFakeUser fakeUser) : ISampleService
    {
        /// <summary>
        /// Создать новый семпл
        /// </summary>
        public async Task<SampleDto> CreateSampleAsync(CreateSampleDto command, CancellationToken ct)
        {
            var sample = new Sample()
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                FilePath = command.File.FileName,
                Type = command.Type,
                Bpm = command.Bpm,
                Key = command.Key,
                UserId = fakeUser.UserId
            };

            await context.Samples.AddAsync(sample, ct);
            await context.SaveChangesAsync(ct);

            return new SampleDto(sample.Id, sample.Name, sample.FilePath, sample.Type, sample.Bpm, sample.Key);
        }

        /// <summary>
        /// Получить список семплов
        /// </summary>
        public async Task<IEnumerable<SampleDto>> GetAllSamplesAsync(CancellationToken ct)
        {
            var query = context.Samples.AsNoTracking();

            // тут позже будут:
            // query = query.Where(...)
            // if (sampleQuery.FilterOn is not null)
            // {
            //     query = query.Where(s => s.Name == sampleQuery.FilterOn);
            // }

            // // query = query.OrderBy(...)
            // query = sampleQuery.SortBy switch
            // {
            //     "name" => sampleQuery.Desc
            //         ? query.OrderByDescending(s => s.Name)
            //         : query.OrderBy(s => s.Name),

            //     _ => query.OrderBy(s => s.Id)
            // };

            // // total count
    
            // // query = query.Skip(...).Take(...)
            // query = query
            //     .Skip((sampleQuery.Page - 1) * sampleQuery.PageSize)
            //     .Take(sampleQuery.PageSize);

            return await query
                .Select(sample => new SampleDto(
                    sample.Id,
                    sample.Name,
                    sample.FilePath,
                    sample.Type,
                    sample.Bpm,
                    sample.Key
                ))
                .ToListAsync(ct);
        }

        /// <summary>
        /// Получить семпл по Id
        /// </summary>
        public async Task<SampleDto?> GetSampleByIdAsync(Guid id, CancellationToken ct)
        {
            var sample = await context.Samples
                .AsNoTracking()
                .FirstOrDefaultAsync(sample => sample.Id == id, cancellationToken: ct);

            return sample == null 
                // ? throw new NotFoundException("Sample", id)
                ? throw new Exception($"not found Sample {id}")
                : new SampleDto(sample.Id, sample.Name, sample.FilePath, sample.Type, sample.Bpm, sample.Key);
        }

        /// <summary>
        /// Обновить данные семпла
        /// </summary>
        public async Task UpdateSampleAsync(Guid id, UpdateSampleDto command, CancellationToken ct)
        {
            var sampleToUpdate = await context.Samples.FindAsync([id], cancellationToken: ct) 
                // ?? throw new NotFoundException("Sample", id);
                ?? throw new Exception($"not found Sample {id}");

            sampleToUpdate.Name = command.Name;
            sampleToUpdate.Type = command.Type;
            sampleToUpdate.Bpm = command.Bpm;
            sampleToUpdate.Key = command.Key;

            await context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Удалить семпл
        /// </summary>
        public async Task DeleteSampleAsync(Guid id, CancellationToken ct)
        {
            var sampleToDelete = await context.Samples.FindAsync([id], cancellationToken: ct);

            if (sampleToDelete != null)
            {
                context.Samples.Remove(sampleToDelete);
                await context.SaveChangesAsync(ct);
            } else
            {
                // throw new NotFoundException("Sample", id);
                throw new Exception($"not found Sample {id}");
            }
        }
    }
}