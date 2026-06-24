// src/pages/booking/CustomerBookings.jsx
import { useEffect, useState } from "react";
import { getBookingsForUser, cancelBooking } from "../../api/booking";

export default function CustomerBookings({ authFetch, currentUser }) {
  const [bookings, setBookings] = useState([]);

  async function load() {
    const data = await getBookingsForUser(currentUser.id, authFetch);
    setBookings(data);
  }

  useEffect(() => {
    load();
  }, []);

  async function handleCancel(id) {
    try {
      await cancelBooking(id, authFetch);
      await load();
    } catch (err) {
      alert(err.message || "Failed to cancel booking");
    }
  }

  return (
    <div className="page-container">
      <h1>My Bookings</h1>
      {bookings.length === 0 && <p>No bookings yet.</p>}
      {bookings.map(b => (
        <div key={b.id} className="booking-card">
          <p>{b.serviceName} with {b.barberName}</p>
          <p>{new Date(b.start).toLocaleString()}</p>
          <button onClick={() => handleCancel(b.id)}>Cancel</button>
        </div>
      ))}
    </div>
  );
}
