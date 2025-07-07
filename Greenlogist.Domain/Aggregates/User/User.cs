using Greenlogist.Domain.Common; // Asume que tienes una clase base para entidades
using Greenlogist.Domain.Exceptions;
using Greenlogist.Domain.ValueObjects; // Para Email y PasswordHash

namespace Greenlogist.Domain.Aggregates.User
{
    // Enumeración para los roles de usuario
    public enum UserRole
    {
        Consumer,
        Producer
    }

    /// <summary>
    /// Representa la entidad raíz del agregado 'User'.
    /// </summary>
    public class User : Entity, IAggregateRoot
    {
        // Propiedades de la entidad
        public string FullName { get; private set; }
        public Email Email { get; private set; } // Objeto de valor para el correo electrónico
        public PasswordHash PasswordHash { get; private set; } // Objeto de valor para el hash de la contraseña
        public UserRole Role { get; private set; }
        public string? Description { get; private set; } // Campo opcional para la descripción del productor
        public string? ProfileImageUrl { get; private set; } // URL de la imagen de perfil

        // Constructor privado para la reconstrucción desde la persistencia (DDD)
        private User() { }

        /// <summary>
        /// Constructor para crear un nuevo usuario.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <param name="fullName">Nombre completo del usuario.</param>
        /// <param name="email">Correo electrónico del usuario (como objeto de valor).</param>
        /// <param name="passwordHash">Hash de la contraseña del usuario (como objeto de valor).</param>
        /// <param name="role">Rol del usuario (Consumidor o Productor).</param>
        public User(Guid id, string fullName, Email email, PasswordHash passwordHash, UserRole role) : base(id)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("El nombre completo no puede estar vacío.");
            if (email == null)
                throw new DomainException("El correo electrónico no puede ser nulo.");
            if (passwordHash == null)
                throw new DomainException("El hash de la contraseña no puede ser nulo.");

            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;

            // Aquí podrías añadir un evento de dominio, por ejemplo:
            // AddDomainEvent(new UserRegisteredEvent(id, fullName, email.Value, role.ToString()));
        }

        /// <summary>
        /// Actualiza la información del perfil del usuario.
        /// </summary>
        /// <param name="fullName">Nuevo nombre completo.</param>
        /// <param name="description">Nueva descripción.</param>
        /// <param name="profileImageUrl">Nueva URL de la imagen de perfil.</param>
        public void UpdateProfile(string fullName, string? description, string? profileImageUrl)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("El nombre completo no puede estar vacío.");

            FullName = fullName;
            Description = description;
            ProfileImageUrl = profileImageUrl;

            // Aquí podrías añadir un evento de dominio
            // AddDomainEvent(new ProducerProfileUpdatedEvent(Id, FullName, Description, ProfileImageUrl));
        }

        /// <summary>
        /// Cambia el correo electrónico del usuario.
        /// </summary>
        /// <param name="newEmail">El nuevo correo electrónico (como objeto de valor).</param>
        public void ChangeEmail(Email newEmail)
        {
            if (newEmail == null)
                throw new DomainException("El nuevo correo electrónico no puede ser nulo.");

            Email = newEmail;
            // Podrías añadir un evento de dominio: EmailChangedEvent
        }

        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        /// <param name="newPasswordHash">El nuevo hash de la contraseña (como objeto de valor).</param>
        public void ChangePassword(PasswordHash newPasswordHash)
        {
            if (newPasswordHash == null)
                throw new DomainException("El nuevo hash de la contraseña no puede ser nulo.");

            PasswordHash = newPasswordHash;
            // Podrías añadir un evento de dominio: PasswordChangedEvent
        }
    }
}
