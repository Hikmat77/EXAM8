using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrganizerRepository
{
    Task<Organizer> AddAsync(Organizer organizer);
    Task<List<Organizer>> GetAllAsync();
    Task<Organizer?> GetAsync(int id);
    Task<List<Event>> GetEventsAsync(int organizerId);
}