using Microsoft.AspNetCore.Identity;

namespace Greenlogist.Domain;

// Heredamos de IdentityUser para obtener todas las propiedades de identidad por defecto.
public class ApplicationUser : IdentityUser
{
    // Añadimos las propiedades personalizadas que nuestro usuario necesita.
    public string FullName { get; set; } = string.Empty;
}