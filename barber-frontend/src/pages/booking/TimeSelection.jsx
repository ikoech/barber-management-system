// src/pages/booking/TimeSelection.jsx
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { getWorkingHoursForBarber } from "../../api/workingHours"; // you have something like this
import { getServiceById } from "../../api/services";

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

export default function TimeSelection({ authFetch }) {
  const { state } = useLocation();
  const navigate = useNavigate();
  const [slots, setSlots] = useState([]);
  const [selectedTime, setSelectedTime] = useState("");

  useEffect(() => {
    if (!state) return;
    (async () => {
      const service = await getServiceById(state.serviceId, authFetch);
      const wh = await getWorkingHoursForBarber(state.barberId, state.date, authFetch);
      const generated = generateSlots(wh, service.durationMinutes);
      setSlots(generated);
    })();
  }, [state, authFetch]);

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
