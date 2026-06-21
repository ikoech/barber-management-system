import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";

export default function DateSelection() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const serviceId = searchParams.get("serviceId");
  const barberId = searchParams.get("barberId") || 1;

  const [service, setService] = useState(null);
  const [workingDays, setWorkingDays] = useState(new Set());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [selectedDate, setSelectedDate] = useState("");

  //
  // ⭐ Load service + working hours
  //
  useEffect(() => {
    async function loadData() {
      try {
        // Load service
        const res = await fetch(`http://localhost:5078/api/services/${serviceId}`);
        if (!res.ok) throw new Error("Failed to load service");
        const data = await res.json();
        setService(data);

        // Load working hours
        const whRes = await fetch(`http://localhost:5078/api/workinghours/${barberId}`);
        if (!whRes.ok) throw new Error("Failed to load working hours");

        const whData = await whRes.json();

        const activeDays = new Set(
          whData.filter(w => w.isActive).map(w => w.dayOfWeek)
        );

        setWorkingDays(activeDays);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, [serviceId, barberId]);

  //
  // ⭐ Disable days the barber does not work
  //
  const isDayAllowed = (dateString) => {
    const date = new Date(dateString);
    const dayName = date.toLocaleDateString("en-US", { weekday: "long" });
    return workingDays.has(dayName);
  };

  const handleDateChange = (e) => {
    const value = e.target.value;

    if (!value) return;

    if (!isDayAllowed(value)) {
      alert("The barber does not work on this day. Please choose another date.");
      return;
    }

    setSelectedDate(value);
  };

  //
  // ⭐ Continue to time selection
  //
  const handleContinue = () => {
    if (!selectedDate) return;

    navigate(
      `/booking/time?serviceId=${serviceId}&barberId=${barberId}&date=${selectedDate}`
    );
  };

  //
  // ⭐ UI states
  //
  if (loading) {
    return <div className="p-10 text-center text-lg">Loading service...</div>;
  }

  if (error) {
    return (
      <div className="p-10 text-center text-red-600 text-lg">
        {error}
      </div>
    );
  }

  //
  // ⭐ Main UI
  //
  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6 text-center">
        Choose a Date
      </h1>

      <div className="mb-6 p-4 border rounded-lg shadow bg-gray-50">
        <div className="text-lg font-medium">{service.name}</div>
        <div className="text-sm text-gray-600">
          {service.durationMinutes} min • {service.price} SEK
        </div>
      </div>

      <label className="block mb-4 text-lg font-medium">
        Select a date:
      </label>

      <input
        type="date"
        className="border p-3 rounded w-full text-lg"
        value={selectedDate}
        onChange={handleDateChange}
        min={new Date().toISOString().split("T")[0]}
      />

      <button
        onClick={handleContinue}
        disabled={!selectedDate}
        className={`mt-6 w-full p-3 rounded text-white text-lg ${
          selectedDate ? "bg-purple-600 hover:bg-purple-700" : "bg-gray-400"
        }`}
      >
        Continue
      </button>
    </div>
  );
}
