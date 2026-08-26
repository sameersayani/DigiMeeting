import type { Room } from '../types/room';
import type { Team } from '../types/Team';

interface CalendarFiltersProps {
  rooms: Room[];
  teams: Team[];
  selectedRoomId: number | null;
  selectedTeamId: number | null;
  onRoomChange: (roomId: number | null) => void;
  onTeamChange: (teamId: number | null) => void;
}

export function CalendarFilters({
  rooms,
  teams,
  selectedRoomId,
  selectedTeamId,
  onRoomChange,
  onTeamChange,
}: CalendarFiltersProps) {
  return (
    <div className="calendar-filters">
      <select
        value={selectedRoomId ?? ''}
        onChange={(e) => onRoomChange(e.target.value ? Number(e.target.value) : null)}
      >
        <option value="">All Rooms</option>
        {rooms.map((r) => (
          <option key={r.id} value={r.id}>{r.name}</option>
        ))}
      </select>

      <select
        value={selectedTeamId ?? ''}
        onChange={(e) => onTeamChange(e.target.value ? Number(e.target.value) : null)}
      >
        <option value="">All Teams</option>
        {teams.map((t) => (
          <option key={t.id} value={t.id}>{t.name}</option>
        ))}
      </select>
    </div>
  );
}