// src/pages/booking/ConfirmBooking.jsx
import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { createBooking } from "../../api/booking";
import { useAuth } from "../../context/AuthContext";

function isValidDateYYYYMMDD(v) {
  return typeof v === "string" && /^\d{4}-\d{2}-\d{2}$/.test(v);
}

function parseId(v) {
  const n = Number(v);
  return Number.isFinite(n) && n > 0 ? n : null;
}

export default function ConfirmBooking() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const { authFetch, user, loading: authLoading } = useAuth();

  const bookingContext = useMemo(() => (state && typeof state === "object" ? state : null), [state]);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
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

    const serviceId = parseId(bookingContext.serviceId);
    const barberId = parseId(bookingContext.barberId);
    const dateStr = bookingContext.date;
    const timeStr = bookingContext.time;

    if (!serviceId) return setError("Missing/invalid serviceId."), void 0;
    if (!barberId) return setError("Missing/invalid barberId."), void 0;
    if (!isValidDateYYYYMMDD(dateStr) || !timeStr) return setError("Missing date/time."), void 0;

    // Backend availability/engine treat times as UTC clock; send ISO with Z.
    const startIso = new Date(`${dateStr}T${timeStr}:00Z`).toISOString();

    if (!startIso || Number.isNaN(new Date(startIso).getTime())) {
      setError("Invalid date/time format.");
      return;
    }

    const dto = {
      userId: Number(user.id),
      serviceId,
      barberId,
      start: startIso,
    };

    try {
      setSubmitting(true);
      const result = await createBooking(dto, authFetch);
      navigate("/bookings/my", { state: { justBooked: result } });
    } catch (err) {
      // createBooking throws message text; backend now returns {message}
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


