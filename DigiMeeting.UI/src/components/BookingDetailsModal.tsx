import { format } from 'date-fns';
import type { CalendarEvent } from '../utils/mapToEvents';
import { getRoomColor } from '../utils/roomColors';

interface BookingDetailsModalProps {
  event: CalendarEvent | null;
  onClose: () => void;
}

export function BookingDetailsModal({ event, onClose }: BookingDetailsModalProps) {
  if (!event) return null;

  const accentColor = getRoomColor(event.resource.roomId);

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-accent-bar" style={{ backgroundColor: accentColor }} />
        <div className="modal-body">
          <h2>Booking Details</h2>
          <div className="modal-row">
            <span className="label">Team</span>
            <span className="value">{event.resource.teamName}</span>
          </div>
          <div className="modal-row">
            <span className="label">Room</span>
            <span className="value">{event.resource.roomName}</span>
          </div>
          <div className="modal-row">
            <span className="label">From</span>
            <span className="value mono">{format(event.start, 'MMM d, yyyy · h:mm a')}</span>
          </div>
          <div className="modal-row">
            <span className="label">To</span>
            <span className="value mono">{format(event.end, 'MMM d, yyyy · h:mm a')}</span>
          </div>
          <button className="modal-close-btn" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
}