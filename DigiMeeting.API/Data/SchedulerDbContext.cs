using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class SchedulerDbContext : DbContext
{
    public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options) { }

    public DbSet<Team> Teams { get; set; }
    public DbSet<MeetingRoom> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<WaitingList> Waitlists { get; set; }
    public DbSet<NotificationQueue> NotificationQueues { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed initial data for your 7 rooms (4 large, 3 small)
        modelBuilder.Entity<MeetingRoom>().HasData(
            new MeetingRoom { Id = 1, Name = "Grand Boardroom", Capacity = 12 },
            new MeetingRoom { Id = 2, Name = "Oceanic Suite", Capacity = 12 },
            new MeetingRoom { Id = 3, Name = "Skyline Hall", Capacity = 15 },
            new MeetingRoom { Id = 4, Name = "The Hive", Capacity = 20 },
            new MeetingRoom { Id = 5, Name = "Huddle Pod A", Capacity = 6 },
            new MeetingRoom { Id = 6, Name = "Huddle Pod B", Capacity = 6 },
            new MeetingRoom { Id = 7, Name = "Focus Room", Capacity = 6 }
        );
    }
}
