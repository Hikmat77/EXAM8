using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class OrganizerService(IOrganizerRepository organizerRepository) : IOrganizerService
{
    private IOrganizerRepository repository = organizerRepository;

    public async Task<ApiResponse<OrganizerDto>> CreateAsync(OrganizerCreateDto dto)
    {
        var organizer = new Organizer()
        {
            CompanyName = dto.CompanyName,
            ContactEmail = dto.ContactEmail,
            Phone = dto.Phone
        };
        var res = await repository.AddAsync(organizer);
        var result = new OrganizerDto()
        {
            Id = res.Id,
            CompanyName = res.CompanyName,
            ContactEmail = res.ContactEmail,
            Phone = res.Phone
        };
        return new ApiResponse<OrganizerDto>(HttpStatusCode.Created, "Organizer created.", result);
    }

    public async Task<ApiResponse<List<OrganizerDto>>> GetAllAsync()
    {
        var organizers = await repository.GetAllAsync();
        var result = organizers.Select(x => new OrganizerDto()
        {
            Id = x.Id,
            CompanyName = x.CompanyName,
            ContactEmail = x.ContactEmail,
            Phone = x.Phone
        }).ToList();
        return new ApiResponse<List<OrganizerDto>>(HttpStatusCode.OK, "List of organizers.", result);
    }

    public async Task<ApiResponse<List<EventDto>>> GetEventsAsync(int organizerId)
    {
        var organizer = await repository.GetAsync(organizerId);
        if (organizer == null)
        {
            return new ApiResponse<List<EventDto>>(HttpStatusCode.NotFound, "Organizer not found.");
        }
        var events = await repository.GetEventsAsync(organizerId);
        var result = events.Select(x => new EventDto()
        {
            Id = x.Id,
            Title = x.Title,
            StartDate = x.StartDate,
            City = x.Venue.City,
            Status = x.Status,
            MinPrice = x.TicketTypes.Count == 0 ? null : x.TicketTypes.Min(t => (decimal?)t.Price)
        }).ToList();
        return new ApiResponse<List<EventDto>>(HttpStatusCode.OK, "Organizer events.", result);
    }
}
