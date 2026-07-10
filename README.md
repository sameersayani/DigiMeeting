# 🗓️ DigiMeeting: Corporate Meeting Room Scheduler with Automated Waitlist Reclamation

A high-performance, real-time corporate meeting room booking engine built with **.NET Core Web API**, **Entity Framework Core**, and **PostgreSQL**. **DigiMeeting** prevents room booking overlaps using a deterministic time-collision algorithm and features an instant slot-reclamation engine that dynamically reallocates cancelled slots to matching waiting teams based on size and timestamp priority.

---

## 🧠 Brainstorming Blueprint & Architecture

Before a single line of code was written, DigiMeeting was mapped on paper using a strict multi-layered architectural boundary:

1. **The Grid Principle**: Time is modeled as a visual matrix where rooms represent columns and time blocks represent rows. Each cell can hold exactly one team assignment.
2. **The Capacity Filter**: Prevents structural alignment errors. Teams are blocked from viewing or booking rooms that cannot accommodate their total headcount.
3. **The Golden Rule of Scheduling**: Two meetings overlap if and only if:
   $$\text{New Meeting Start} < \text{Existing Meeting End} \quad \text{AND} \quad \text{New Meeting End} > \text{Existing Meeting Start}$$
4. **Unit of Work Pattern**: Bundles soft cancellations, waitlist status updates, and auto-reallocation insert procedures into a single, transactional database boundary to eliminate partial-write data corruption.

### System Topography

