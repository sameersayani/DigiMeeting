// Fixed, professional palette — one color per room, assigned deterministically by roomId.
const ROOM_PALETTE = [
  '#3454D1', // indigo
  '#0EA5A4', // teal
  '#F59E0B', // amber
  '#EF4444', // coral red
  '#8B5CF6', // violet
  '#10B981', // emerald
  '#EC4899', // pink
];

export function getRoomColor(roomId: number): string {
  const index = roomId % ROOM_PALETTE.length;
  return ROOM_PALETTE[index];
}