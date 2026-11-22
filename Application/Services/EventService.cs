using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class EventService(
    IEventRepository eventRepository,
    IEventQueryRepository eventQueryRepository) : IEventService
{
    private IEventRepository repository = eventRepository;
    private IEventQueryRepository queryRepository = eventQueryRepository;

    public async Task<ApiResponse<PagedResult<EventDto>>> GetAsync(
        string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice,
        string? search, int page, int pageSize, string sortBy, string sortDir)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        dateFrom = dateFrom.HasValue ? ToUtc(dateFrom.Value) : null;
        dateTo = dateTo.HasValue ? ToUtc(dateTo.Value) : null;
        var result = await queryRepository.GetPublishedAsync(
            city, categoryId, dateFrom, dateTo, maxPrice, search, page, pageSize, sortBy, sortDir);
        var total = await repository.CountPublishedAsync(city, categoryId, dateFrom, dateTo, maxPrice, search);
        var data = new PagedResult<EventDto>()
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
        return new ApiResponse<PagedResult<EventDto>>(HttpStatusCode.OK, "List of events.", data);
    }

    public async Task<ApiResponse<EventDetailsDto>> GetDetailsAsync(int id)
    {
        var entity = await repository.GetDetailsAsync(id);
        if (entity == null)
        {
            return new ApiResponse<EventDetailsDto>(HttpStatusCode.NotFound, "Event not found.");
        }
        var result = new EventDetailsDto()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Venue = new VenueDto()
            {
                Id = entity.Venue.Id,
                Name = entity.Venue.Name,
                Address = entity.Venue.Address,
                City = entity.Venue.City,
                Capacity = entity.Venue.Capacity
            },
            OrganizerName = entity.Organizer.CompanyName,
            Categories = entity.Categories.Select(x => new CategoryDto()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList(),
            TicketTypes = entity.TicketTypes.Select(x => new TicketTypeDto()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                SoldCount = x.SoldCount,
                Available = x.Quantity - x.SoldCount
            }).ToList(),
            AverageRating = entity.Reviews.Count == 0 ? 0 : (decimal)entity.Reviews.Average(x => x.Rating),
            ReviewsCount = entity.Reviews.Count
        };
        return new ApiResponse<EventDetailsDto>(HttpStatusCode.OK, "Event details.", result);
    }

    public async Task<ApiResponse<EventDto>> CreateAsync(EventCreateDto dto)
    {
        dto.StartDate = ToUtc(dto.StartDate);
        dto.EndDate = ToUtc(dto.EndDate);
        if (dto.EndDate <= dto.StartDate)
        {
            return new ApiResponse<EventDto>(HttpStatusCode.BadRequest, "EndDate must be after StartDate.");
        }
        if (!await repository.VenueExistsAsync(dto.VenueId))
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "Venue not found.");
        }
        if (!await repository.OrganizerExistsAsync(dto.OrganizerId))
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "Organizer not found.");
        }
        var categories = await repository.GetCategoriesAsync(dto.CategoryIds);
        if (categories.Count != dto.CategoryIds.Distinct().Count())
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "One or more categories were not found.");
        }
        var entity = new Event()
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            VenueId = dto.VenueId,
            OrganizerId = dto.OrganizerId,
            Status = EventStatus.Draft,
            Categories = categories
        };
        var res = await repository.AddAsync(entity);
        var result = new EventDto()
        {
            Id = res.Id,
            Title = res.Title,
            StartDate = res.StartDate,
            City = await repository.GetVenueCityAsync(dto.VenueId) ?? string.Empty,
            Status = res.Status
        };
        return new ApiResponse<EventDto>(HttpStatusCode.Created, "Event created.", result);
    }

    public async Task<ApiResponse<EventDto>> UpdateAsync(int id, EventUpdateDto dto)
    {
        dto.StartDate = ToUtc(dto.StartDate);
        dto.EndDate = ToUtc(dto.EndDate);
        var entity = await repository.GetWithCategoriesAsync(id);
        if (entity == null)
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "Event not found.");
        }
        if (entity.Status != EventStatus.Draft)
        {
            return new ApiResponse<EventDto>(HttpStatusCode.Conflict, "Only Draft events can be edited.");
        }
        if (dto.EndDate <= dto.StartDate)
        {
            return new ApiResponse<EventDto>(HttpStatusCode.BadRequest, "EndDate must be after StartDate.");
        }
        if (!await repository.VenueExistsAsync(dto.VenueId))
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "Venue not found.");
        }
        if (!await repository.OrganizerExistsAsync(dto.OrganizerId))
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "Organizer not found.");
        }
        var categories = await repository.GetCategoriesAsync(dto.CategoryIds);
        if (categories.Count != dto.CategoryIds.Distinct().Count())
        {
            return new ApiResponse<EventDto>(HttpStatusCode.NotFound, "One or more categories were not found.");
        }
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.VenueId = dto.VenueId;
        entity.OrganizerId = dto.OrganizerId;
        entity.Categories = categories;
        await repository.UpdateAsync();
        var result = new EventDto()
        {
            Id = entity.Id,
            Title = entity.Title,
            StartDate = entity.StartDate,
            City = await repository.GetVenueCityAsync(dto.VenueId) ?? string.Empty,
            Status = entity.Status
        };
        return new ApiResponse<EventDto>(HttpStatusCode.OK, "Event updated.", result);
    }

    public async Task<ApiResponse<string>> PublishAsync(int id)
    {
        var entity = await repository.GetWithTicketTypesAsync(id);
        if (entity == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Event not found.");
        }
        if (entity.Status != EventStatus.Draft)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Only Draft events can be published.");
        }
        if (entity.TicketTypes.Count == 0)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Event needs at least one TicketType.");
        }
        entity.Status = EventStatus.Published;
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Event published.", "Event published.");
    }

    public async Task<ApiResponse<string>> CancelAsync(int id)
    {
        var entity = await repository.GetAsync(id);
        if (entity == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Event not found.");
        }
        if (entity.Status == EventStatus.Cancelled || entity.Status == EventStatus.Completed)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Cancelled or completed events cannot be cancelled.");
        }
        entity.Status = EventStatus.Cancelled;
        var orders = await repository.GetPaidOrdersForEventAsync(id);
        foreach (var order in orders)
        {
            foreach (var ticket in order.Tickets)
            {
                ticket.TicketType.SoldCount--;
            }
            order.Status = OrderStatus.Cancelled;
        }
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Event cancelled.", "Event cancelled.");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var entity = await repository.GetAsync(id);
        if (entity == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Event not found.");
        }
        entity.IsDeleted = true;
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Event deleted.", "Event deleted.");
    }

    public async Task<ApiResponse<TicketTypeDto>> AddTicketTypeAsync(int eventId, TicketTypeCreateDto dto)
    {
        var entity = await repository.GetAsync(eventId);
        if (entity == null)
        {
            return new ApiResponse<TicketTypeDto>(HttpStatusCode.NotFound, "Event not found.");
        }
        if (entity.Status == EventStatus.Cancelled || entity.Status == EventStatus.Completed)
        {
            return new ApiResponse<TicketTypeDto>(HttpStatusCode.Conflict, "Ticket types cannot be added to cancelled or completed events.");
        }
        var ticketType = new TicketType()
        {
            EventId = eventId,
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity
        };
        var res = await repository.AddTicketTypeAsync(ticketType);
        var result = new TicketTypeDto()
        {
            Id = res.Id,
            Name = res.Name,
            Price = res.Price,
            Quantity = res.Quantity,
            SoldCount = res.SoldCount,
            Available = res.Quantity - res.SoldCount
        };
        return new ApiResponse<TicketTypeDto>(HttpStatusCode.Created, "Ticket type created.", result);
    }

    public async Task<ApiResponse<PagedResult<ReviewDto>>> GetReviewsAsync(int eventId, int page, int pageSize)
    {
        if (await repository.GetAsync(eventId) == null)
        {
            return new ApiResponse<PagedResult<ReviewDto>>(HttpStatusCode.NotFound, "Event not found.");
        }
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var reviews = await repository.GetReviewsAsync(eventId, page, pageSize);
        var total = await repository.CountReviewsAsync(eventId);
        var result = reviews.Select(x => new ReviewDto()
        {
            Id = x.Id,
            AttendeeId = x.AttendeeId,
            Rating = x.Rating,
            Comment = x.Comment,
            CreatedAt = x.CreatedAt
        }).ToList();
        var data = new PagedResult<ReviewDto>()
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
        return new ApiResponse<PagedResult<ReviewDto>>(HttpStatusCode.OK, "Event reviews.", data);
    }

    public async Task<ApiResponse<ReviewDto>> CreateReviewAsync(int eventId, ReviewCreateDto dto)
    {
        var entity = await repository.GetAsync(eventId);
        if (entity == null)
        {
            return new ApiResponse<ReviewDto>(HttpStatusCode.NotFound, "Event not found.");
        }
        if (DateTime.UtcNow <= entity.EndDate)
        {
            return new ApiResponse<ReviewDto>(HttpStatusCode.Conflict, "Event has not ended.");
        }
        if (await repository.AttendeeHasReviewAsync(eventId, dto.AttendeeId))
        {
            return new ApiResponse<ReviewDto>(HttpStatusCode.Conflict, "Attendee already reviewed this event.");
        }
        var eligible = await repository.AttendeeHasCheckedInPaidTicketAsync(eventId, dto.AttendeeId);
        if (!eligible)
        {
            return new ApiResponse<ReviewDto>(HttpStatusCode.Conflict, "Attendee has no checked-in paid ticket for this event.");
        }
        var review = new Review()
        {
            EventId = eventId,
            AttendeeId = dto.AttendeeId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
        var res = await repository.AddReviewAsync(review);
        var result = new ReviewDto()
        {
            Id = res.Id,
            AttendeeId = res.AttendeeId,
            Rating = res.Rating,
            Comment = res.Comment,
            CreatedAt = res.CreatedAt
        };
        return new ApiResponse<ReviewDto>(HttpStatusCode.Created, "Review created.", result);
    }

    private static DateTime ToUtc(DateTime date)
    {
        return date.Kind == DateTimeKind.Utc
            ? date
            : date.Kind == DateTimeKind.Local
                ? date.ToUniversalTime()
                : DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
}