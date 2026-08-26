import type { Booking } from '../types/booking';

export interface CalendarEvent {
    id: number;
    title: string;
    start: Date;
    end: Date;
    resource: {
        roomId: number;
        roomName: string;
        teamId: number;
        teamName: string;
    };
}

export function mapBookingsToEvents(bookings: Booking[]): CalendarEvent[] {
 return bookings.map((b) => ({
    id: b.id,
    title: `${b.teamName} - ${b.roomName}`,
    start: new Date(b.startTime),
    end: new Date(b.endTime),
    resource: {
        roomId: b.roomId,
        roomName: b.roomName,
        teamId: b.teamId,
        teamName: b.teamName,
    },
 }))   
}