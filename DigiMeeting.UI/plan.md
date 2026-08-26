## Plan: React calendar UI for room bookings

The goal is to add a TypeScript React front end that reads the existing booking dashboard from the ASP.NET API and presents a calendar-style view of room bookings by day, time, team, and room.

### Phase 1 — Project setup
1. Create a Vite React + TypeScript app in the existing DigiMeeting.UI folder.
2. Add the minimum dependencies needed for a calendar UI and API communication.
3. Configure the app to call the API running at localhost:5209.

### Phase 2 — UI and data integration
1. Create a shared API client that calls the existing /api/booking/dashboard endpoint.
2. Map the returned bookings and rooms into calendar events with date/time, room, and team details.
3. Build a calendar view that shows bookings by day and time, with room and team labels.
4. Add lightweight filters for room and date range so the calendar is easier to read.

### Phase 3 — Polish and verification
1. Style the calendar so it is readable and visually grouped by room/time.
2. Verify the app builds successfully and can render data from the API when the backend is running.

### Relevant files
- DigiMeeting.UI/* — new React application files
- DigiMeeting.API/Controllers/BookingController.cs — existing API contract to reuse

### Verification
1. Run the frontend build.
2. Start the backend and confirm the UI loads dashboard data from the endpoint.
