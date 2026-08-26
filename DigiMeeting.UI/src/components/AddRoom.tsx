import * as React from 'react';
import { useNavigate } from 'react-router';
import { createRoom } from '../config/bookingClient';
import type { Room } from '../types/room';

export default function AddRoom(){
    const navigate = useNavigate();

    const[name, setName] = React.useState<string>('');
    const[capacity, setCapacity] = React.useState<number>(0);
    const[error, setError] = React.useState<string | null>(null);
    const[isSubmitting, setIsSubmitting] = React.useState<boolean>(false);

    async function onSubmit(e: React.FormEvent<HTMLElement>) {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try{
            const newRoomPayload: Room = {
                id: 0,
                name: name,
                capacity: capacity
            };

            await createRoom(newRoomPayload);
            navigate('/dashboard');
        } catch (err: any) {
            setError(err.message || 'Something went wrong creating new room');
        } finally{
            setIsSubmitting(false);
        }
    }

    return(
        <div style={{padding: '20px'}}>
            <h2> Add New Room</h2>
            {error && <p style={{color: 'red'}}>{error}</p>}
            <form onSubmit={onSubmit}>
                <div>
                    <label htmlFor="roomName">Room Name:</label>
                    <br />
                    <input
                    id="roomName"
                    type="text"
                    required
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    disabled={isSubmitting}
                    />
                </div>
                <br />
                 <div>
                    <label htmlFor="roomCapacity">Capacity:</label>
                    <br />
                    <input
                        id="roomCapacity"
                        type="number"
                        required
                        min="1"
                        value={capacity || ''} 
                        onChange={(e) => setCapacity(Number(e.target.value))}
                        disabled={isSubmitting}
                    />
                </div>
                <br />
                <button type="button" onClick={()=> navigate('/dashboard)')} disabled={isSubmitting}>
                    Cancel
                </button>
                <button disabled={isSubmitting}>
                    {isSubmitting ? 'Creating Room...' : 'Saved'}
                </button>
            </form>
        </div>
    )
}