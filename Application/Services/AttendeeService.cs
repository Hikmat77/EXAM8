using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class AttendeeService(IAttendeeRepository attendeeRepository) : IAttendeeService
{
    private IAttendeeRepository repository = attendeeRepository;

    public async Task<ApiResponse<AttendeeDto>> CreateAsync(AttendeeCreateDto dto)
    {
        if (await repository.GetByEmailAsync(dto.Email) != null)
        {
            return new ApiResponse<AttendeeDto>(HttpStatusCode.Conflict, "Attendee email already exists.");
        }
        var attendee = new Attendee()
        {
            FullName = dto.FullName,
            Email = dto.Email,
            RegisteredAt = DateTime.UtcNow
        };
        var res = await repository.AddAsync(attendee);
        var result = new AttendeeDto()
        {
            Id = res.Id,
            FullName = res.FullName,
            Email = res.Email,
            RegisteredAt = res.RegisteredAt
        };
        return new ApiResponse<AttendeeDto>(HttpStatusCode.Created, "Attendee created.", result);
    }

    public async Task<ApiResponse<PagedResult<AttendeeDto>>> GetPagedAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var attendees = await repository.GetPagedAsync(page, pageSize);
        var total = await repository.CountAsync();
        var result = attendees.Select(x => new AttendeeDto()
        {
            Id = x.Id,
            FullName = x.FullName,
            Email = x.Email,
            RegisteredAt = x.RegisteredAt,
            Profile = x.Profile == null ? null : new AttendeeProfileDto()
            {
                DateOfBirth = x.Profile.DateOfBirth,
                City = x.Profile.City,
                Bio = x.Profile.Bio
            }
        }).ToList();
        var data = new PagedResult<AttendeeDto>()
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
        return new ApiResponse<PagedResult<AttendeeDto>>(HttpStatusCode.OK, "List of attendees.", data);
    }

    public async Task<ApiResponse<AttendeeDto>> UpdateProfileAsync(int id, AttendeeProfileDto dto)
    {
        var attendee = await repository.GetAsync(id);
        if (attendee == null)
        {
            return new ApiResponse<AttendeeDto>(HttpStatusCode.NotFound, "Attendee not found.");
        }
        if (attendee.Profile == null)
        {
            attendee.Profile = new AttendeeProfile()
            {
                AttendeeId = id
            };
        }
        attendee.Profile.DateOfBirth = dto.DateOfBirth.HasValue
            ? ToUtc(dto.DateOfBirth.Value)
            : null;
        attendee.Profile.City = dto.City;
        attendee.Profile.Bio = dto.Bio;
        await repository.UpdateAsync();
        var result = new AttendeeDto()
        {
            Id = attendee.Id,
            FullName = attendee.FullName,
            Email = attendee.Email,
            RegisteredAt = attendee.RegisteredAt,
            Profile = new AttendeeProfileDto()
            {
                DateOfBirth = attendee.Profile.DateOfBirth,
                City = attendee.Profile.City,
                Bio = attendee.Profile.Bio
            }
        };
        return new ApiResponse<AttendeeDto>(HttpStatusCode.OK, "Profile updated.", result);
    }

    public async Task<ApiResponse<PagedResult<OrderDto>>> GetOrdersAsync(int attendeeId, int page, int pageSize)
    {
        if (!await repository.ExistsAsync(attendeeId))
        {
            return new ApiResponse<PagedResult<OrderDto>>(HttpStatusCode.NotFound, "Attendee not found.");
        }
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var orders = await repository.GetOrdersAsync(attendeeId, page, pageSize);
        var total = await repository.CountOrdersAsync(attendeeId);
        var result = orders.Select(x => new OrderDto()
        {
            Id = x.Id,
            Status = x.Status,
            TotalAmount = x.TotalAmount,
            CreatedAt = x.CreatedAt,
            Tickets = x.Tickets.Select(t => new TicketDto()
            {
                Code = t.Code,
                EventTitle = t.TicketType.Event.Title,
                TicketTypeName = t.TicketType.Name,
                Price = t.Price,
                IsCheckedIn = t.IsCheckedIn
            }).ToList()
        }).ToList();
        var data = new PagedResult<OrderDto>()
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
        return new ApiResponse<PagedResult<OrderDto>>(HttpStatusCode.OK, "Attendee orders.", data);
    }

    public async Task<ApiResponse<List<TicketDto>>> GetTicketsAsync(int attendeeId)
    {
        var attendee = await repository.GetAsync(attendeeId);
        if (attendee == null)
        {
            return new ApiResponse<List<TicketDto>>(HttpStatusCode.NotFound, "Attendee not found.");
        }
        var tickets = await repository.GetTicketsAsync(attendeeId);
        var result = tickets.Select(x => new TicketDto()
        {
            Code = x.Code,
            EventTitle = x.TicketType.Event.Title,
            TicketTypeName = x.TicketType.Name,
            Price = x.Price,
            IsCheckedIn = x.IsCheckedIn
        }).ToList();
        return new ApiResponse<List<TicketDto>>(HttpStatusCode.OK, "Attendee tickets.", result);
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
