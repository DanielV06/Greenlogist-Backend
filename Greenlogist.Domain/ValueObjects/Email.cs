using Greenlogist.Domain.Common; // Asume que tienes una clase base para ValueObjects
using Greenlogist.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Greenlogist.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de valor para representar un correo electrónico.
    /// Garantiza la validez del formato del correo electrónico.
    /// </summary>
    public class Email : ValueObject
    {
        public string Value { get; private set; }

        private Email() { } // Constructor privado para ORM/deserialización

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El correo electrónico no puede ser nulo o vacío.");

            // Expresión regular para una validación de correo electrónico básica
            if (!IsValidEmail(value))
                throw new DomainException($"El formato del correo electrónico '{value}' no es válido.");

            Value = value.ToLowerInvariant(); // Almacenar en minúsculas para consistencia
        }

        /// <summary>
        /// Valida el formato del correo electrónico.
        /// </summary>
        /// <param name="email">El string del correo electrónico a validar.</param>
        /// <returns>True si el formato es válido, false en caso contrario.</returns>
        private static bool IsValidEmail(string email)
        {
            // Una expresión regular más robusta para validación de correo electrónico
            // Fuente: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string value) => new Email(value);
    }
}
