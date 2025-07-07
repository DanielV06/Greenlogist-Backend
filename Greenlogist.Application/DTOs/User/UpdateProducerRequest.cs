using System.ComponentModel.DataAnnotations;

namespace Greenlogist.Application.DTOs.User
{
    /// <summary>
    /// DTO para la solicitud de actualización del perfil del productor.
    /// </summary>
    public class UpdateProducerProfileRequest
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string FullName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? NewPassword { get; set; } // Opcional
    }
}