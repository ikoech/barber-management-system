// src/pages/booking/CustomerBookings.jsx
import { useEffect, useState } from "react";
import { getBookingsForUser, cancelBooking } from "../../api/booking";
import { useAuth } from "../../context/AuthContext";

function toUserId(v) {
  if (v === null || v === undefined) return null;
  const n = Number(v);
  return Number.isFinite(n) && n > 0 ? n : null;
}

export default function CustomerBookings() {
  const { authFetch, loading, user } = useAuth();
  const [bookings, setBookings] = useState([]);
  const userId = toUserId(user?.id);

  async function load() {
    if (!userId) return;
    const data = await getBookingsForUser(userId, authFetch);
    setBookings(Array.isArray(data) ? data : []);
  }

  useEffect(() => {
    if (loading) return;
    if (!userId) {
      setBookings([]);
      return;
    }
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [loading, userId]);

  async function handleCancel(id) {
    try {
      await cancelBooking(id, authFetch);
      await load();
    } catch (err) {
      alert(err?.message || "Failed to cancel booking");
    }
  }

  if (loading) return <div className="page-container">Loading...</div>;

  return (
    <div className="page-container">
      <h1>My Bookings</h1>
      {bookings.length === 0 && <p>No bookings yet.</p>}
      {bookings.map((b) => (
        <div key={b.id} className="booking-card">
          <p>
            {b.serviceName} with {b.barberName}
          </p>
          <p>{b.start ? new Date(b.start).toLocaleString() : ""}</p>
          <button onClick={() => handleCancel(b.id)}>Cancel</button>
        </div>
      ))}
    </div>
  );
}

