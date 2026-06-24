import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getBarbers } from "../../api/barber";

//  Helper: Find next working day for the barber
const getNextWorkingDay = async (barberId, date) => {
  const res = await authFetch(`http://localhost:5078/api/workinghours/${barberId}`);
  const workingHours = await res.json();

  // Extract active days (e.g., "Monday", "Tuesday")
  const activeDays = new Set(
    workingHours.filter(w => w.isActive).map(w => w.dayOfWeek)
  );

  let next = new Date(date);

  // Look ahead up to 7 days
  for (let i = 0; i < 7; i++) {
    next.setDate(next.getDate() + 1);

    const dayName = next.toLocaleDateString("en-US", { weekday: "long" });

    if (activeDays.has(dayName)) {
      return next;
    }
  }

  return null;
};

export default function TimeSelection() {
  const { authFetch, user } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const serviceId = searchParams.get("serviceId");
  const barberId = searchParams.get("barberId") || 1;
  const date = searchParams.get("date");

  const [service, setService] = useState(null);
  const [slots, setSlots] = useState([]);
  const [loading, setLoading] = useState(true);
  const [redirecting, setRedirecting] = useState(false);
  const [error, setError] = useState("");
  const [selectedTime, setSelectedTime] = useState("");

  //  Load service + availability
  useEffect(() => {
    async function loadData() {
      try {
        // Load service info
        const serviceRes = await authFetch(
          `http://localhost:5078/api/services/${serviceId}`
        );
        if (!serviceRes.ok) throw new Error("Failed to load service");
        const serviceData = await serviceRes.json();
        setService(serviceData);

        // Load availability
        const availRes = await authFetch(
          `http://localhost:5078/api/availability?serviceId=${serviceId}&barberId=${barberId}&date=${date}`
        );
        if (!availRes.ok) throw new Error("Failed to load availability");

        const slotData = await availRes.json();

        // Auto‑redirect if no slots
        if (slotData.length === 0) {
          setRedirecting(true);

          const nextDay = await getNextWorkingDay(barberId, new Date(date));

          if (nextDay) {
            const formatted = nextDay.toISOString().split("T")[0];

            navigate(
              `/booking/time?serviceId=${serviceId}&barberId=${barberId}&date=${formatted}`
            );
          }

          return;
        }

        // Convert UTC → Local
        const localSlots = slotData.map((utcString) => {
          const d = new Date(utcString);
          return {
            utc: utcString,
            local: d.toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit",
            }),
          };
        });

        setSlots(localSlots);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, [serviceId, barberId, date, navigate]);

  //UI states
  if (redirecting) {
    return (
      <div className="p-10 text-center text-lg">
        Finding next available day…
      </div>
    );
  }

  if (loading) {
    return <div className="p-10 text-center text-lg">Loading times…</div>;
  }

  if (error) {
    return <div className="p-10 text-center text-red-600 text-lg">{error}</div>;
  }

  // Main UI
  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6 text-center">
        Choose a Time
      </h1>

      {service && (
        <div className="mb-6 p-4 border rounded-lg shadow bg-gray-50">
          <div className="text-lg font-medium">{service.name}</div>
          <div className="text-sm text-gray-600">
            {service.durationMinutes} min • {service.price} SEK
          </div>
          <div className="text-sm text-gray-600 mt-1">Date: {date}</div>
        </div>
      )}

      <div className="grid grid-cols-3 gap-3">
        {slots.length === 0 && (
          <div className="col-span-3 text-center text-gray-500">
            No available times
          </div>
        )}

        {slots.map((slot) => (
          <button
            key={slot.utc}
            onClick={() => setSelectedTime(slot.utc)}
            className={`p-3 rounded border text-center ${
              selectedTime === slot.utc
                ? "bg-purple-600 text-white"
                : "bg-white hover:bg-gray-100"
            }`}
          >
            {slot.local}
          </button>
        ))}
      </div>

      <button
        onClick={() =>
          navigate(
            `/booking/confirm?serviceId=${serviceId}&barberId=${barberId}&date=${date}&time=${selectedTime}`
          )
        }
        disabled={!selectedTime}
        className={`mt-6 w-full p-3 rounded text-white text-lg ${
          selectedTime ? "bg-purple-600 hover:bg-purple-700" : "bg-gray-400"
        }`}
      >
        Continue
      </button>
    </div>
  );
}
