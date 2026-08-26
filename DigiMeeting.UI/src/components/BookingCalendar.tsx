import { useCallback, useEffect, useMemo, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { Calendar, dateFnsLocalizer, Views } from 'react-big-calendar';
import { getDashboard } from '../config/bookingClient';
import { mapBookingsToEvents } from '../utils/mapToEvents';
import { CalendarFilters } from './CalendarFilters';
import type { Room } from '../types/room';
import type { Team } from '../types/Team';
import type { CalendarEvent } from '../utils/mapToEvents';
import { format, parse, startOfWeek, getDay } from 'date-fns';
import { enUS } from 'date-fns/locale';
// ...localizer setup (format, parse, startOfWeek, getDay from date-fns)
import { BookingDetailsModal } from './BookingDetailsModal';
import { getRoomColor } from '../utils/roomColors';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import '../App.css';

const locales = { 'en-US': enUS };

const localizer = dateFnsLocalizer({
  format,
  parse,
  startOfWeek,
  getDay,
  locales,
});

export function BookingCalendar() {
  const { logout } = useAuth0();
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [teams, setTeams] = useState<Team[]>([]);
  const [selectedRoomId, setSelectedRoomId] = useState<number | null>(null);
  const [selectedTeamId, setSelectedTeamId] = useState<number | null>(null);

  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);

  const handleNavigate = useCallback((newDate: Date) => {
    setCurrentDate(newDate);
  }, []);

  const handleViewChange = useCallback((newView: any) => {
    setCurrentView(newView);
  }, []);

useEffect(() => {
  getDashboard().then((data) => {
    //            ^ data: DashboardResponse — TS infers this automatically
    //              from the Promise<DashboardResponse> return type
    setEvents(mapBookingsToEvents(data.bookings)); // data.bookings: Booking[]
    setRooms(data.rooms);                          // data.rooms: Room[]
    setTeams(data.teams);                           // data.teams: Team[]
  });
}, []);


const handleSelectEvent = useCallback((event: CalendarEvent) => {
  setSelectedEvent(event);

  // Nudge the "+more" popup closed by dispatching a real mousedown
  // on document — matches what the overlay's outside-click listener expects.
  setTimeout(() => {
    document.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
  }, 0);
}, []);

// const handleSelectSlot = useCallback((slotInfo: any) => {
//   alert('selected slot'+ slotInfo);
//   // add your slot-based behavior here, e.g. open a booking form
// }, []);

  // filtering happens here, derived from state — recompute only when inputs change
  const filteredEvents = useMemo(() => {
    return events.filter(
      (e) =>
        (!selectedRoomId || e.resource.roomId === selectedRoomId) &&
        (!selectedTeamId || e.resource.teamId === selectedTeamId)
    );
  }, [events, selectedRoomId, selectedTeamId]);

  const eventPropGetter = useCallback((event: CalendarEvent) => {
  const color = getRoomColor(event.resource.roomId);
  return {
    style: {
      backgroundColor: color,
      borderLeft: `4px solid ${color}`,
      borderRadius: '4px',
      color: '#fff',
      opacity: 0.9,
    },
  };
}, []);

  const handleLogout = () => {
    logout({
      logoutParams: {
        returnTo: window.location.origin,
      },
    });
  };

  return (
    <div>
        <div className="app-header">
          <h1>Dashboard</h1>
          <div className="dashboard-actions">
          <CalendarFilters
            rooms={rooms}
            teams={teams}
            selectedRoomId={selectedRoomId}
            selectedTeamId={selectedTeamId}
            onRoomChange={setSelectedRoomId}
            onTeamChange={setSelectedTeamId}
          />
          <button type="button" className="logout-button" onClick={handleLogout}>
            Logout
          </button>
          </div>
        </div>
       <div className="calendar-container">
          <Calendar
            events={filteredEvents}
            localizer={localizer}
            startAccessor="start"
            endAccessor="end"
            date={currentDate}
            view={currentView}
            onNavigate={handleNavigate}
            onView={handleViewChange}
            onSelectEvent={handleSelectEvent}
            eventPropGetter={eventPropGetter}
            popup
          />
      </div>
      <BookingDetailsModal
        event={selectedEvent}
        onClose={() => setSelectedEvent(null)}
      />
    </div>
  );
}
