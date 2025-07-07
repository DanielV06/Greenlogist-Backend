using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;

namespace Greenlogist.Domain.ValueObjects
{
    /// <summary>
    /// Value object to represent the quantity of a product with its unit of measure.
    /// Ensures that the quantity is non-negative.
    /// </summary>
    public class Quantity : ValueObject
    {
        public decimal Value { get; private set; } // Quantity value
        public string Unit { get; private set; } // Unit of measure (e.g., "kg", "units", "liters")

        private Quantity() { } // Private constructor for ORM/deserialization

        public Quantity(decimal value, string unit)
        {
            if (value < 0)
                throw new DomainException("Quantity cannot be negative.");
            if (string.IsNullOrWhiteSpace(unit))
                throw new DomainException("Unit of measure cannot be null or empty.");

            Value = value;
            Unit = unit.ToLowerInvariant(); // Store unit in lowercase for consistency
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }

        // Implicit conversion to decimal for convenience
        public static implicit operator decimal(Quantity quantity) => quantity.Value;
    }
}
