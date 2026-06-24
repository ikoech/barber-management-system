// src/pages/booking/TimeSelection.jsx
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { getWorkingHoursForBarber } from "../../api/workingHours";
import { getServiceById } from "../../api/services";
import { useAuth } from "../../context/AuthContext";


function generateSlots(workingHours, serviceDurationMinutes) {
  if (!workingHours) return [];
  const slots = [];
  const start = workingHours.startTime; // assume "09:00:00"
  const end = workingHours.endTime;     // assume "17:00:00"

  const [sh, sm] = start.split(":").map(Number);
  const [eh, em] = end.split(":").map(Number);

  let current = sh * 60 + sm;
  const endMinutes = eh * 60 + em;

  while (current + serviceDurationMinutes <= endMinutes) {
    const h = Math.floor(current / 60);
    const m = current % 60;
    slots.push(`${h.toString().padStart(2, "0")}:${m.toString().padStart(2, "0")}`);
    current += serviceDurationMinutes;
  }

  return slots;
}

export default function TimeSelection() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const { authFetch, loading: authLoading } = useAuth();
  const [slots, setSlots] = useState([]);
  const [selectedTime, setSelectedTime] = useState("");

  useEffect(() => {
    if (authLoading) return;
    if (!state) return;

    (async () => {
      try {
        const barberId = state?.barberId;
        if (barberId === null || barberId === undefined || barberId === "") {
          setSlots([]);
          return;
        }

        const service = await getServiceById(state.serviceId, authFetch);
        const durationMinutes = Number(service?.durationMinutes ?? 0);

        const wh = await getWorkingHoursForBarber(barberId, state.date, authFetch);
        const generated = generateSlots(Array.isArray(wh) ? wh[0] ?? wh : wh, durationMinutes);
        setSlots(Array.isArray(generated) ? generated : []);
      } catch {
        setSlots([]);
      }
    })();
  }, [state, authFetch, authLoading]);

  if (!state) return <div className="page-container">Missing booking context.</div>;

  function handleNext() {
    if (!selectedTime) return;
    navigate("/booking/confirm", { state: { ...state, time: selectedTime } });
  }

  return (
    <div className="page-container">
      <h1>Select Time</h1>
      <div>
        {slots.map(t => (
          <button
            key={t}
            className={t === selectedTime ? "booking-card selected" : "booking-card"}
            onClick={() => setSelectedTime(t)}
          >
            {t}
          </button>
        ))}
      </div>
      <button onClick={handleNext}>Next</button>
    </div>
  );
}
