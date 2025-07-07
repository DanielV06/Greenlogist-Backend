using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;

namespace Greenlogist.Domain.ValueObjects
{
    /// <summary>
    /// Value object to represent a geographical location.
    /// </summary>
    public class Location : ValueObject
    {
        public string Address { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        // Potentially add Latitude, Longitude if precise coordinates are needed

        private Location() { } // Private constructor for ORM/deserialization

        public Location(string address, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Address cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country cannot be null or empty.");

            Address = address;
            City = city;
            Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
            yield return City;
            yield return Country;
        }
    }
}
