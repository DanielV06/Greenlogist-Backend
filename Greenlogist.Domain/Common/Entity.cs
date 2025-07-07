namespace Greenlogist.Domain.Common
{
    /// <summary>
    /// Clase base abstracta para todas las entidades de dominio.
    /// Proporciona un identificador único (Guid) y manejo de eventos de dominio.
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; private set; }

        // Lista para almacenar eventos de dominio que ocurrieron en esta entidad
        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected Entity(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID de la entidad no puede ser Guid.Empty.", nameof(id));
            Id = id;
            _domainEvents = new List<IDomainEvent>();
        }

        // Constructor sin parámetros para EF Core u ORMs
        protected Entity()
        {
            _domainEvents = new List<IDomainEvent>();
        }

        /// <summary>
        /// Añade un evento de dominio a la lista de eventos.
        /// </summary>
        /// <param name="eventItem">El evento de dominio a añadir.</param>
        protected void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Elimina un evento de dominio de la lista.
        /// </summary>
        /// <param name="eventItem">El evento de dominio a eliminar.</param>
        protected void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        /// <summary>
        /// Borra todos los eventos de dominio de la entidad.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        // Sobrecarga de operadores de igualdad para entidades
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            Entity other = (Entity)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
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

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
