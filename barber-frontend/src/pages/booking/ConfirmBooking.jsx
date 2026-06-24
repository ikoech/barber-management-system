// src/pages/booking/ConfirmBooking.jsx
import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { createBooking } from "../../api/booking";
import { useAuth } from "../../context/AuthContext";

export default function ConfirmBooking() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const { authFetch, user, loading: authLoading } = useAuth();

  const bookingContext = useMemo(() => (state && typeof state === "object" ? state : null), [state]);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    // clear error when context changes
    setError("");
  }, [bookingContext]);

  async function handleConfirm() {
    setError("");

    if (authLoading) return;
    if (!bookingContext) {
      setError("Missing booking context.");
      return;
    }
    if (!user?.id) {
      setError("User not loaded. Please try again.");
      return;
    }

    const serviceId = Number(bookingContext.serviceId);
    const barberIdRaw = bookingContext.barberId;
    const barberId = barberIdRaw === "" || barberIdRaw === null || barberIdRaw === undefined ? 0 : Number(barberIdRaw);

    const dateStr = bookingContext.date;
    const timeStr = bookingContext.time;

    if (!serviceId || Number.isNaN(serviceId)) {
      setError("Missing/invalid serviceId.");
      return;
    }

    if (!dateStr || !timeStr) {
      setError("Missing date/time.");
      return;
    }

    // Expected: state.time looks like "HH:MM" (from TimeSelection)
    // Convert local selection to UTC ISO string
    const startLocal = new Date(`${dateStr}T${timeStr}:00`);
    if (Number.isNaN(startLocal.getTime())) {
      setError("Invalid date/time format.");
      return;
    }

    const startUtc = new Date(startLocal.getTime() - startLocal.getTimezoneOffset() * 60000);

    const dto = {
      userId: Number(user.id),
      serviceId,
      barberId,
      start: startUtc.toISOString(),
    };

    // Avoid sending invalid IDs
    if (!dto.userId || !dto.serviceId || !dto.start) {
      setError("Invalid booking payload.");
      return;
    }

    try {
      setSubmitting(true);
      const result = await createBooking(dto, authFetch);
      navigate("/bookings/my", { state: { justBooked: result } });
    } catch (err) {
      setError(err?.message || "Failed to create booking.");
    } finally {
      setSubmitting(false);
    }
  }

  if (authLoading) {
    return (
      <div className="page-container">
        <div>Loading…</div>
      </div>
    );
  }

  if (!bookingContext) {
    return <div className="page-container">Missing booking context.</div>;
  }

  return (
    <div className="page-container">
      <h1>Confirm Booking</h1>
      <p>Date: {bookingContext.date}</p>
      <p>Time: {bookingContext.time}</p>
      {error ? <div style={{ color: "#b91c1c", marginTop: 8 }}>{error}</div> : null}
      <button onClick={handleConfirm} disabled={submitting}>
        {submitting ? "Confirming…" : "Confirm"}
      </button>
    </div>
  );
}

