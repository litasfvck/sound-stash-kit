namespace SoundStashKit.DTO
{
    public record PackDto(
        Guid Id,
        string Name,
        string? Description,
        string? CoverImagePath,
        Guid UserId);

    public record CreatePackDto(
        string Name,
        string? Description,
        IFormFile? CoverImage); 

    public record UpdatePackDto(
        string? Name,
        string? Description,
        IFormFile? CoverImage); 
}