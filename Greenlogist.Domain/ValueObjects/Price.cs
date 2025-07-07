using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;

namespace Greenlogist.Domain.ValueObjects
{
    /// <summary>
    /// Value object to represent a product's price.
    /// Ensures that the price is non-negative.
    /// </summary>
    public class Price : ValueObject
    {
        public decimal Value { get; private set; } // Price value
        public string Currency { get; private set; } // Currency (e.g., "PEN", "USD")

        private Price() { } // Private constructor for ORM/deserialization

        public Price(decimal value, string currency)
        {
            if (value < 0)
                throw new DomainException("Price cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Currency cannot be null or empty.");

            Value = value;
            Currency = currency.ToUpperInvariant(); // Store currency in uppercase for consistency
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Currency;
        }

        // Implicit conversion to decimal for convenience
        public static implicit operator decimal(Price price) => price.Value;
        // Explicit conversion from decimal (assuming a default currency)
        // Consider if this explicit conversion is always desirable or if a factory method is better
        // public static explicit operator Price(decimal value) => new Price(value, "PEN");
    }
}

