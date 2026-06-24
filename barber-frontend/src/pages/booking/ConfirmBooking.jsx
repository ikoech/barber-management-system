// src/pages/booking/ConfirmBooking.jsx
import { useLocation, useNavigate } from "react-router-dom";
import { createBooking } from "../../api/booking";

export default function ConfirmBooking({ authFetch, currentUser }) {
  const { state } = useLocation();
  const navigate = useNavigate();

  if (!state) return <div className="page-container">Missing booking context.</div>;

  async function handleConfirm() {
    const startLocal = new Date(`${state.date}T${state.time}:00`);
    const startUtc = new Date(startLocal.getTime() - startLocal.getTimezoneOffset() * 60000);

    const dto = {
      userId: currentUser.id,
      barberId: state.barberId,
      serviceId: state.serviceId,
      start: startUtc.toISOString(),
    };

    try {
      const result = await createBooking(dto, authFetch);
      navigate("/bookings/my", { state: { justBooked: result } });
    } catch (err) {
      alert(err.message || "Failed to create booking");
    }
  }

  return (
    <div className="page-container">
      <h1>Confirm Booking</h1>
      <p>Date: {state.date}</p>
      <p>Time: {state.time}</p>
      <button onClick={handleConfirm}>Confirm</button>
    </div>
  );
}
