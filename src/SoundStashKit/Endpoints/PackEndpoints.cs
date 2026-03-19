using System.Net;
using SoundStashKit.DTO;
using SoundStashKit.Services.PackService;

namespace SoundStashKit.Endpoints
{
    public static class PackEndpoints
    {
        public static void MapPackEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/packs");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:guid}", Replace);
            group.MapPatch("/{id:guid}", Update);
            group.MapPost("/{id:guid}/samples/{sampleId:guid}", AddSampleToPack);
            group.MapDelete("/{id:guid}/samples/{sampleId:guid}", RemoveSampleFromPack);
            group.MapDelete("/{id:guid}", Delete);
        }
        
        private static async Task<IResult> Create(IPackService packService,  CreatePackDto command, CancellationToken ct)
        {
            var pack = await packService.CreatePackAsync(command, ct);
            return TypedResults.Created($"/api/packs/{pack.Id}", pack);
        }

        private static async Task<IResult> GetAll(IPackService packService, PackDto command, CancellationToken ct)
        {
            var packs = await packService.GetAllPacksAsync(ct);
            return TypedResults.Ok(packs);
        }

        private static async Task<IResult> GetById(IPackService packService, Guid id, CancellationToken ct)
        {
            var pack = await packService.GetPackByIdAsync(id, ct);
            return pack is null
                ? TypedResults.NotFound(new { Message = $"Pack with ID {id} not found." })
                : TypedResults.Ok(pack);
        }
        
        private static async Task<IResult> Replace(IPackService packService, Guid id, UpdatePackDto command, CancellationToken ct)
        {
            await packService.UpdatePackAsync(id, command, ct);
            return TypedResults.NoContent();
        }

        private static async Task<IResult> Update(IPackService packService, Guid id, UpdatePackDto command, CancellationToken ct)
        {
            await packService.UpdatePackAsync(id, command, ct);
            return TypedResults.NoContent();
        }

        private static async Task<IResult> AddSampleToPack(IPackService packService, Guid id, Guid sampleId, CancellationToken ct)
        {
            await packService.AddSampleToPackAsync(id, sampleId, ct);
            return TypedResults.NoContent();
        }   

        private static async Task<IResult> RemoveSampleFromPack(IPackService packService, Guid id, Guid sampleId, CancellationToken ct)
        {
            await packService.RemoveSampleFromPackAsync(id, sampleId, ct);
            return TypedResults.NoContent();
        }   
        
        private static async Task<IResult> Delete(IPackService packService, Guid id, CancellationToken ct)
        {
            await packService.DeletePackAsync(id, ct);
            return TypedResults.NoContent();
        }
        
    }
}