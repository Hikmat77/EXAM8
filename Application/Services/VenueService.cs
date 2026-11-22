using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class VenueService(IVenueRepository venueRepository) : IVenueService
{
    private IVenueRepository repository = venueRepository;

    public async Task<ApiResponse<VenueDto>> CreateAsync(VenueCreateDto dto)
    {
        var venue = new Venue()
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            Capacity = dto.Capacity
        };
        var res = await repository.AddAsync(venue);
        var result = new VenueDto()
        {
            Id = res.Id,
            Name = res.Name,
            Address = res.Address,
            City = res.City,
            Capacity = res.Capacity
        };
        return new ApiResponse<VenueDto>(HttpStatusCode.Created, "Venue created.", result);
    }

    public async Task<ApiResponse<List<VenueDto>>> GetAllAsync()
    {
        var venues = await repository.GetAllAsync();
        var result = venues.Select(x => new VenueDto()
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            City = x.City,
            Capacity = x.Capacity
        }).ToList();
        return new ApiResponse<List<VenueDto>>(HttpStatusCode.OK, "List of venues.", result);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var venue = await repository.GetWithEventsAsync(id);
        if (venue == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Venue not found.");
        }
        if (venue.Events.Count != 0)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Venue has events.");
        }
        await repository.DeleteAsync(venue);
        return new ApiResponse<string>(HttpStatusCode.OK, "Venue deleted.", "Venue deleted.");
    }
}
