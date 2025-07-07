namespace Greenlogist.Domain.Common
{
    /// <summary>
    /// Clase base abstracta para todos los objetos de valor de dominio.
    /// Proporciona una implementación de igualdad basada en componentes.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Obtiene los componentes que se utilizarán para determinar la igualdad.
        /// </summary>
        /// <returns>Una colección de objetos que representan los componentes del objeto de valor.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// Determina si dos objetos de valor son iguales.
        /// </summary>
        /// <param name="obj">El objeto a comparar con el objeto actual.</param>
        /// <returns>True si los objetos de valor son iguales, false en caso contrario.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Devuelve el código hash para este objeto de valor.
        /// </summary>
        /// <returns>Un código hash para el objeto de valor actual.</returns>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }
    }
}
