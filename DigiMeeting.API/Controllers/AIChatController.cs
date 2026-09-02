using DigiMeeting.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace DigiMeeting.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIChatController : ControllerBase
    {
        // Replace with your actual service or logic to handle AI chat commands
        private readonly IBookingRepository _bookingRepo;
         private readonly IWaitlistRepository _waitlistRepo;

        public AIChatController(IBookingRepository bookingRepo, 
            IWaitlistRepository waitlistRepo)
        {
            _bookingRepo = bookingRepo;
            _waitlistRepo = waitlistRepo;
        }

        [HttpPost]
        public async Task<IActionResult> HandleCommand([FromBody] string command)
        {
            try
            {
                if (Regex.IsMatch(command, @"create meeting '(.+)' at (\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z) in Room (\d+)( for Team (\d+))?"))
                {
                    var match = Regex.Match(command, @"create meeting '(.+)' at (\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z) in Room (\d+)( for Team (\d+))?" );
                    string meetingName = match.Groups[1].Value;
                    string dateTime = match.Groups[2].Value;
                    int roomId = int.Parse(match.Groups[3].Value);
                    int teamId = match.Groups[5].Success ? int.Parse(match.Groups[5].Value) : 0;

                    await _bookingRepo.CreateMeetingAsync(meetingName, dateTime, roomId, teamId);
                    return Ok($"Meeting '{meetingName}' created");
                }
                else if (Regex.IsMatch(command, @"cancel meeting (\d+)"))
                {
                    int meetingId = int.Parse(Regex.Match(command, @"cancel meeting (\d+)").Groups[1].Value);
                    await _bookingRepo.CancelMeetingAsync(meetingId);
                    return Ok($"Meeting {meetingId} canceled");
                }
                else if (Regex.IsMatch(command, @"add user (\d+) to waiting list for meeting (\d+)"))
                {
                    int userId = int.Parse(Regex.Match(command, @"add user (\d+) to waiting list for meeting (\d+)").Groups[1].Value);
                    int meetingId = int.Parse(Regex.Match(command, @"add user (\d+) to waiting list for meeting (\d+)").Groups[2].Value);
                    await _waitlistRepo.JoinWaitlist(userId, meetingId, DateTime.UtcNow, DateTime.UtcNow.AddHours(1)); // Assuming a 1-hour slot for simplicity
                    return Ok($"User {userId} added to waiting list for meeting {meetingId}");
                }
                else if (Regex.IsMatch(command, @"^add room '([^']+)' with capacity (\d+)$", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(command, @"^add room '([^']+)' with capacity (\d+)$", RegexOptions.IgnoreCase);
                    string roomName = match.Groups[1].Value;
                    int capacity = int.Parse(match.Groups[2].Value);

                    if (capacity <= 0)
                    {
                        return BadRequest("Room capacity must be greater than zero.");
                    }

                    await _bookingRepo.AddRoomAsync(roomName, capacity);
                    return Ok("Room added successfully.");
                }
                else if (Regex.IsMatch(command, @"^add team '([^']+)' with members '([^']+)'$", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(command, @"^add team '([^']+)' with members '([^']+)'$", RegexOptions.IgnoreCase);
                    string teamName = match.Groups[1].Value;
                    var memberEmailIds = match.Groups[2].Value
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (memberEmailIds.Count == 0)
                    {
                        return BadRequest("At least one team member email is required.");
                    }

                    await _bookingRepo.AddTeamAsync(teamName, memberEmailIds.Count, memberEmailIds);
                    return Ok("Team added successfully.");
                }
                else
                {
                    return Ok("Unknown command format. Supported formats:\n- create meeting '<name>' at <date> in Room <id> [for Team <id>]\n- cancel meeting <id>\n- add user <id> to waiting list for meeting <id>\n- add room '<name>' with capacity <number>\n- add team '<name>' with members '<email1,email2,...>'");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}