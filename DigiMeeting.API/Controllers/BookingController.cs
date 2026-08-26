using DigiMeeting.API.Data;
using DigiMeeting.API.DTOs;
using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SchedulerDbContext _context; // Used to quickly verify Room capacities

    public BookingController(IUnitOfWork unitOfWork, SchedulerDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    [HttpPost("room")]
    public async Task<IActionResult> AddMeetingRoom([FromBody] RoomRequest request)
    {
        var room = new MeetingRoom
        {
            Agenda = request.Name,
            Capacity = request.Capacity,
            CreatedBy = "test"
        };

        await _unitOfWork.Rooms.AddAsync(room);
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Room added successfully!", room.Id });
    }

    [HttpPost("team")]
    public async Task<IActionResult> AddTeam([FromBody] TeamRequest request)
    {
        var distinctEmails = request.Email.Distinct().ToList();
        var team = new Team
        {
            Name = request.Name,
            Email = request.Email,
            MemberCount = distinctEmails.Count,
            CreatedBy = "test"
        };

        await _unitOfWork.Teams.AddAsync(team);
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Team added successfully!", team.Id });
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookRoom([FromBody] BookingRequest request)
    {
        if (request.StartTime >= request.EndTime)
        {
            return BadRequest("End time must be after start time.");
        }

        // 1. Fetch Room and Team details to verify physical capacity limits
        var room = await _context.Rooms.FindAsync(request.RoomId);
        var team = await _context.Teams.FindAsync(request.TeamId);

        if (room == null || team == null)
        {
            return NotFound("Invalid Team ID or Room ID.");
        }

        // 2. Capacity Check
        if (team.MemberCount > room.Capacity)
        {
            return BadRequest($"Room capacity ({room.Capacity}) is too small for team size ({team.MemberCount}).");
        }

        // 3. Golden Rule Overlap Check
        bool isOverlapped = await _unitOfWork.Bookings.HasOverlapAsync(request.RoomId, request.StartTime, request.EndTime);
        if (isOverlapped)
        {
            return Conflict("This room is already booked during the requested time slot.");
        }

        // 4. Create and Save Booking
        var booking = new Booking
        {
            TeamId = request.TeamId,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Room booked successfully!", BookingId = booking.Id });
    }

    [HttpPost("waitlist")]
    public async Task<IActionResult> JoinWaitlist([FromBody] WaitlistRequestDto request)
    {
        var team = await _context.Teams.FindAsync(request.TeamId);
        if (team == null) return NotFound("Team not found.");

        var waitlistEntry = new WaitingList
        {
            TeamId = request.TeamId,
            RequiredCapacity = team.MemberCount,
            TargetStartTime = request.TargetStartTime,
            TargetEndTime = request.TargetEndTime,
            Status = "Active"
        };

        await _unitOfWork.Waitlists.AddAsync(waitlistEntry);
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Added to waitlist successfully!", WaitlistId = waitlistEntry.Id });
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        // 1. Find the booking securely
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null || booking.IsCancelled)
        {
            return NotFound("Active booking not found.");
        }

        // 2. Soft cancel the booking
        booking.IsCancelled = true;

        // 3. Reclaim Logic: Check if anyone on the waitlist can utilize this slot
        if (booking.Room != null)
        {
            var nextInLine = await _unitOfWork.Waitlists.GetNextTeamForSlotAsync(
                booking.Room.Capacity, 
                booking.StartTime, 
                booking.EndTime
            );

            if (nextInLine != null)
            {
                // Create a booking for the waiting team automatically
                var reclaimedBooking = new Booking
                {
                    TeamId = nextInLine.TeamId,
                    RoomId = booking.RoomId,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime
                };

                await _unitOfWork.Bookings.AddAsync(reclaimedBooking);
                nextInLine.Status = "Fulfilled";

                // Add an entry to the background notification buffer queue
                var notification = new NotificationQueue
                {
                    RecipientTeamName = nextInLine.Team?.Name ?? $"Team {nextInLine.TeamId}",
                    Message = $"Great news! You have been auto-allocated into {booking.Room.Agenda} from {booking.StartTime} to {booking.EndTime}."
                };
                await _context.NotificationQueues.AddAsync(notification);
            }
        }

        // 4. Commit all transaction parts safely at once
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Booking cancelled safely. Slot has been updated or reassigned." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeetingRoom(int id, [FromBody] RoomRequest request)
{
    var room = await _unitOfWork.Rooms.GetByIdAsync(id);
    if (room == null)
    {
        return NotFound($"Room with ID {id} not found.");
    }

    room.Agenda = request.Name;
    room.Capacity = request.Capacity;

    await _unitOfWork.Rooms.UpdateAsync(room);
    await _unitOfWork.CompleteAsync();

    return Ok(new { Message = "Room updated successfully!", room.Id });
}

    [HttpPut("team/{id}")]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamRequest request)
{
    var team = await _unitOfWork.Teams.GetByIdAsync(id);
    if (team == null)
    {
        return NotFound($"Team with ID {id} not found.");
    }

    team.Name = request.Name;
    team.Email = request.Email;
    var distinctEmails = request.Email.Distinct().ToList();
    team.MemberCount = distinctEmails.Count();
    
    await _unitOfWork.Teams.UpdateAsync(team);
    await _unitOfWork.CompleteAsync();

    return Ok(new { Message = "Team updated successfully!", team.Id });
}

    [HttpPut("book/{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingRequest request)
{
    if (request.StartTime >= request.EndTime)
    {
        return BadRequest("End time must be after start time.");
    }

    var booking = await _context.Bookings.FindAsync(id);
    if (booking == null)
    {
        return NotFound("Booking not found.");
    }

    // 1. Fetch Room and Team details to verify physical capacity limits
    var room = await _context.Rooms.FindAsync(request.RoomId);
    var team = await _context.Teams.FindAsync(request.TeamId);
    if (room == null || team == null)
    {
        return NotFound("Invalid Team ID or Room ID.");
    }

    // 2. Capacity Check
    if (team.MemberCount > room.Capacity)
    {
        return BadRequest($"Room capacity ({room.Capacity}) is too small for team size ({team.MemberCount}).");
    }

    // 3. Golden Rule Overlap Check (excluding this booking itself)
    bool isOverlapped = await _unitOfWork.Bookings.HasOverlapAsync(
        request.RoomId, request.StartTime, request.EndTime, excludeBookingId: id);
    if (isOverlapped)
    {
        return Conflict("This room is already booked during the requested time slot.");
    }

    // 4. Update and Save Booking
    booking.TeamId = request.TeamId;
    booking.RoomId = request.RoomId;
    booking.StartTime = request.StartTime;
    booking.EndTime = request.EndTime;

    await _unitOfWork.Bookings.UpdateAsync(booking);
    await _unitOfWork.CompleteAsync();

    return Ok(new { Message = "Booking updated successfully!", BookingId = booking.Id });
}

    [HttpPut("waitlist/{id}")]
    public async Task<IActionResult> UpdateWaitlistEntry(int id, [FromBody] WaitlistRequestDto request)
{
    var waitlistEntry = await _context.Waitlists.FindAsync(id);
    if (waitlistEntry == null)
    {
        return NotFound("Waitlist entry not found.");
    }

    var team = await _context.Teams.FindAsync(request.TeamId);
    if (team == null) return NotFound("Team not found.");

    waitlistEntry.TeamId = request.TeamId;
    waitlistEntry.RequiredCapacity = team.MemberCount;
    waitlistEntry.TargetStartTime = request.TargetStartTime;
    waitlistEntry.TargetEndTime = request.TargetEndTime;

    await _unitOfWork.Waitlists.UpdateAsync(waitlistEntry);
    await _unitOfWork.CompleteAsync();

    return Ok(new { Message = "Waitlist entry updated successfully!", WaitlistId = waitlistEntry.Id });
}

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var activeBookings = await _context.Bookings
            .Where(b => !b.IsCancelled)
            .Include(b => b.Room)
            .Include(b => b.Team)
            .Select(b => new {
                b.Id,
                RoomId = b.RoomId,
                RoomName = b.Room != null ? b.Room.Agenda : "Unknown Room",
                TeamId = b.TeamId,
                TeamName = b.Team != null ? b.Team.Name : "Unknown Team",
                b.StartTime,
                b.EndTime
            })
            .ToListAsync();

        var rooms = await _context.Rooms.ToListAsync();
        var teams = await _context.Teams.ToListAsync();

        return Ok(new { Bookings = activeBookings, Rooms = rooms, Teams = teams });
    }
}
