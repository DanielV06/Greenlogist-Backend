using System.ComponentModel.DataAnnotations;

namespace Greenlogist.Application.DTOs.Auth
{
    /// <summary>
    /// DTO para la solicitud de registro de usuario.
    /// </summary>
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [RegularExpression("^(Consumer|Producer)$", ErrorMessage = "El rol debe ser 'Consumer' o 'Producer'.")]
        public string Role { get; set; } = string.Empty;
    }
}
