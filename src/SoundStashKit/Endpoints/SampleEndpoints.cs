using SoundStashKit.DTO;
using SoundStashKit.Services.SampleService;

namespace SoundStashKit.Endpoints
{
    public static class SampleEndpoints
    {
        public static void MapSampleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/samples");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:guid}", Replace);
            group.MapPatch("/{id:guid}", Update);
            group.MapDelete("/{id:guid}", Delete);
        }        

        private static async Task<IResult> Create(ISampleService sampleService, CreateSampleDto command, CancellationToken ct)
        {
            var sample = await sampleService.CreateSampleAsync(command, ct);
            return TypedResults.Created($"/samples/{sample.Id}", sample);
        }

        private static async Task<IResult> GetAll(ISampleService sampleService, CancellationToken ct)
        {
            var samples = await sampleService.GetAllSamplesAsync(ct);
            return TypedResults.Ok(samples);
        }

        private static async Task<IResult> GetById(ISampleService sampleService, Guid id, CancellationToken ct)
        {
            var sample = await sampleService.GetSampleByIdAsync(id, ct);
            return sample is null
                ? TypedResults.NotFound(new { Message = $"Sample with ID {id} not found." })
                : TypedResults.Ok(sample);
        }

        private static async Task<IResult> Update(ISampleService sampleService, Guid id, UpdateSampleDto command, CancellationToken ct)
        {
            await sampleService.UpdateSampleAsync(id, command, ct);
            return TypedResults.NoContent();
        }

        private static async Task<IResult> Replace(ISampleService sampleService, Guid id, UpdateSampleDto command, CancellationToken ct)
        {
            await sampleService.UpdateSampleAsync(id, command, ct);
            return TypedResults.NoContent();
        }

        private static async Task<IResult> Delete(ISampleService sampleService, Guid id, CancellationToken ct)
        {
            await sampleService.DeleteSampleAsync(id, ct);
            return TypedResults.NoContent();
        }
    }
}