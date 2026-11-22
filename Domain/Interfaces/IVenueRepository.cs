using Domain.Entities;

namespace Domain.Interfaces;

public interface IVenueRepository
{
    Task<Venue> AddAsync(Venue venue);
    Task<List<Venue>> GetAllAsync();
    Task<Venue?> GetWithEventsAsync(int id);
    Task DeleteAsync(Venue venue);
}