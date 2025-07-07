using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;

namespace Greenlogist.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de valor para almacenar el hash de una contraseña.
    /// Esto asegura que las contraseñas nunca se manejen en texto plano en el dominio.
    /// </summary>
    public class PasswordHash : ValueObject
    {
        public string Value { get; private set; }

        private PasswordHash() { } // Constructor privado para ORM/deserialización

        public PasswordHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El hash de la contraseña no puede ser nulo o vacío.");

            // Aquí podrías añadir validaciones sobre el formato del hash si lo necesitas
            // Por ejemplo, si esperas un hash de una longitud específica o un formato particular.
            // Para este ejemplo, asumimos que el valor ya es un hash válido.

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
        public static explicit operator PasswordHash(string value) => new PasswordHash(value);
    }
}
