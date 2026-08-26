import type { DashboardResponse } from "../types/DashboardResponse";
import type { Room } from "../types/room";
import { API_BASE_URL } from "./api";

export async function getDashboard(): Promise<DashboardResponse> {
  const res = await fetch(`${API_BASE_URL}/api/booking/dashboard`);
  if (!res.ok) {
    throw new Error(`Failed to fetch dashboard: ${res.status} ${res.statusText}`);
  }
  return res.json();
}

export async function createRoom(roomData: Room): Promise<Room>{
  const res = await fetch(`${API_BASE_URL}/api/room`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(roomData)
  });

   if (!res.ok) {
    throw new Error(`Failed to add room: ${res.status} ${res.statusText}`);
  }

  return res.json();
}