import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";

export default function ConfirmBooking() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const serviceId = searchParams.get("serviceId");
  const barberId = searchParams.get("barberId");
  const date = searchParams.get("date");
  const timeUtc = searchParams.get("time");

  const [service, setService] = useState(null);
  const [localTime, setLocalTime] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState("");

  // Load service info + convert time
  useEffect(() => {
    async function load() {
      try {
        const res = await fetch(`http://localhost:5078/api/services/${serviceId}`);
        if (!res.ok) throw new Error("Failed to load service");

        const data = await res.json();
        setService(data);

        // Convert UTC → Local
        const d = new Date(timeUtc);
        setLocalTime(
          d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
        );
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [serviceId, timeUtc]);

  const handleConfirm = async () => {
    setSubmitting(true);
    setError("");

    try {
        const res = await fetch("http://localhost:5078/api/bookings", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: 1, // TEMP until you add auth
            barberId: Number(barberId),
            serviceId: Number(serviceId),
            start: timeUtc // already a full ISO string
        })
        });

      if (!res.ok) throw new Error("Failed to create booking");

      setSuccess(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="p-10 text-center text-lg">Loading...</div>;
  }

  if (error) {
    return <div className="p-10 text-center text-red-600 text-lg">{error}</div>;
  }

  // SUCCESS SCREEN WITH ANIMATION
  if (success) {
    return (
      <div className="max-w-xl mx-auto p-6 text-center animate-fadeIn">
        <div className="flex justify-center mb-6">
          <div className="w-24 h-24 bg-green-500 rounded-full flex items-center justify-center animate-pop">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-14 w-14 text-white animate-draw"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              strokeWidth="3"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M5 13l4 4L19 7"
              />
            </svg>
          </div>
        </div>

        <h1 className="text-2xl font-semibold mb-2">Booking Confirmed!</h1>
        <p className="text-lg text-gray-700 mb-6">
          Your appointment has been successfully booked.
        </p>

        <button
          onClick={() => navigate("/")}
          className="w-full p-3 rounded bg-purple-600 text-white text-lg hover:bg-purple-700 transition"
        >
          Back to Home
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6 text-center">
        Confirm Your Booking
      </h1>

      <div className="mb-6 p-4 border rounded-lg shadow bg-gray-50">
        <div className="text-lg font-medium">{service.name}</div>
        <div className="text-sm text-gray-600">
          {service.durationMinutes} min • {service.price} SEK
        </div>
        <div className="text-sm text-gray-600 mt-1">Date: {date}</div>
        <div className="text-sm text-gray-600">Time: {localTime}</div>
        <div className="text-sm text-gray-600">Barber: {barberId}</div>
      </div>

      <button
        onClick={handleConfirm}
        disabled={submitting}
        className={`w-full p-3 rounded text-white text-lg ${
          submitting ? "bg-gray-400" : "bg-purple-600 hover:bg-purple-700"
        }`}
      >
        {submitting ? "Booking..." : "Confirm Booking"}
      </button>
    </div>
  );
}
