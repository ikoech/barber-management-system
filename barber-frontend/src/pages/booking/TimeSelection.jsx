// src/pages/booking/TimeSelection.jsx
import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { getWorkingHoursForBarber } from "../../api/workingHours";
import { useAuth } from "../../context/AuthContext";

function isValidDateYYYYMMDD(v) {
  return typeof v === "string" && /^\d{4}-\d{2}-\d{2}$/.test(v);
}

function parseId(v) {
  if (v === null || v === undefined || v === "") return null;
  const n = Number(v);
  return Number.isFinite(n) && n > 0 ? n : null;
}

export default function TimeSelection() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const { authFetch, loading: authLoading, user } = useAuth();
  const [slots, setSlots] = useState([]);
  const [selectedTime, setSelectedTime] = useState("");

  const barberId = useMemo(() => parseId(state?.barberId), [state?.barberId]);
  const serviceId = useMemo(() => parseId(state?.serviceId), [state?.serviceId]);

  const dateIso = useMemo(() => {
    const v = state?.date;
    // Accept either YYYY-MM-DD or a Date-like string and normalize to YYYY-MM-DD.
    if (isValidDateYYYYMMDD(v)) return v;
    if (!v) return "";
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return "";
    return d.toISOString().slice(0, 10);
  }, [state?.date]);

  useEffect(() => {
    if (authLoading) return;
    if (!state) return;

    if (!barberId || !serviceId || !isValidDateYYYYMMDD(dateIso)) {
      setSlots([]);
      setSelectedTime("");
      return;
    }

    (async () => {
      try {
        const availability = await getWorkingHoursForBarber(barberId, dateIso, serviceId, authFetch);
        const availableTimes = availability?.availableTimes;

        if (!Array.isArray(availableTimes) || availableTimes.length === 0) {
          setSlots([]);
          setSelectedTime("");
          return;
        }

        setSlots(availableTimes);
        setSelectedTime((prev) => (availableTimes.includes(prev) ? prev : ""));
      } catch {
        setSlots([]);
        setSelectedTime("");
      }
    })();
  }, [state, barberId, dateIso, serviceId, authFetch, authLoading]);

  if (!state) return <div className="page-container">Missing booking context.</div>;

  function handleNext() {
    if (!selectedTime) return;
    if (!slots.includes(selectedTime)) return;
    navigate("/booking/confirm", { state: { ...state, time: selectedTime } });
  }

  return (
    <div className="page-container">
      <h1>Select Time</h1>
      <div>
        {slots.length === 0 ? <div>No available times.</div> : null}
        {slots.map(t => (
          <button
            key={t}
            type="button"
            className={t === selectedTime ? "booking-card selected" : "booking-card"}
            onClick={() => setSelectedTime(t)}
          >
            {t}
          </button>
        ))}
      </div>
      <button onClick={handleNext} disabled={!selectedTime}>
        Next
      </button>
    </div>
  );
}

