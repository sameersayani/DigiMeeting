using DigiMeeting.API.Data;
using DigiMeeting.API.DTOs;
using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SchedulerDbContext _context; // Used to quickly verify Room capacities

    public BookingController(IUnitOfWork unitOfWork, SchedulerDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
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
                    Message = $"Great news! You have been auto-allocated into {booking.Room.Name} from {booking.StartTime} to {booking.EndTime}."
                };
                await _context.NotificationQueues.AddAsync(notification);
            }
        }

        // 4. Commit all transaction parts safely at once
        await _unitOfWork.CompleteAsync();

        return Ok(new { Message = "Booking cancelled safely. Slot has been updated or reassigned." });
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
            RoomName = b.Room != null ? b.Room.Name : "Unknown Room",
            TeamName = b.Team != null ? b.Team.Name : "Unknown Team",
            b.StartTime,
            b.EndTime
        })
        .ToListAsync();

    var rooms = await _context.Rooms.ToListAsync();

    return Ok(new { Bookings = activeBookings, Rooms = rooms });
    }
}
