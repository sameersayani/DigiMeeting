import type { Booking } from "./booking";
import type { Room } from "./room";
import type { Team } from "./Team";

export interface DashboardResponse {
  bookings: Booking[];
  rooms: Room[];
  teams: Team[];
}