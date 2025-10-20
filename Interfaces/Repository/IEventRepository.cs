using HealingInWriting.Domain.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealingInWriting.Interfaces.Repository
{
    /// <summary>
    /// Specifies event persistence operations, separating data access concerns from domain logic.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Retrieves all events in the catalogue.
        /// </summary>
        Task<IEnumerable<Event>> GetAllAsync();

        /// <summary>
        /// Retrieves a single event by its unique identifier.
        /// </summary>
        Task<Event?> GetByIdAsync(int eventId);

        /// <summary>
        /// Adds a new event to the catalogue.
        /// </summary>
        Task AddAsync(Event @event);

        /// <summary>
        /// Updates an existing event in the catalogue.
        /// </summary>
        Task UpdateAsync(Event @event);

        /// <summary>
        /// Deletes an event from the catalogue by its unique identifier.
        /// </summary>
        Task DeleteAsync(int eventId);

        /// <summary>
        /// Retrieves events within a specified distance (in kilometers) from the given latitude and longitude.
        /// </summary>
        //Task<IEnumerable<Event>> GetByProximityAsync(double latitude, double longitude, double radiusKm);
        //Implementation will require discussion of retrieval vs manipulation responsibilities.
    }
}
