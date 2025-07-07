namespace Greenlogist.Application.DTOs.User
{
    /// <summary>
    /// DTO para la respuesta del perfil del productor (modelo de lectura).
    /// </summary>
    public record ProducerProfileDto(
        Guid Id,
        string FullName,
        string Email,
        string? Description,
        string? ProfileImageUrl
    );
}