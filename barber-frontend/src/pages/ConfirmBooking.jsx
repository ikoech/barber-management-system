import { useSearchParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";

export default function ConfirmBooking() {
  const { user, authFetch } = useAuth();
  const [params] = useSearchParams();
  const [error, setError] = useState("");

  const serviceId = params.get("serviceId");
  const barberId = params.get("barberId");
  const date = params.get("date");
  const time = params.get("time"); // ISO string

  const handleConfirm = async () => {
    try {
      const start = new Date(time).toISOString();

      const res = await authFetch("http://localhost:5078/api/bookings", {
        method: "POST",
        body: JSON.stringify({
          userId: user.id,
          serviceId: Number(serviceId),
          barberId: Number(barberId),
          start
        }),
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg);
      }

      window.location.href = "/dashboard";
    } catch (err) {
      setError(err.message);
    }
  };

  if (!user) return <p>Loading...</p>;

  return (
    <div className="p-10">
      <h1 className="text-2xl font-semibold mb-4">Confirm Booking</h1>

      {error && <p className="text-red-600 mb-2">{error}</p>}

      <p>Service ID: {serviceId}</p>
      <p>Barber ID: {barberId}</p>
      <p>Date: {date}</p>
      <p>Time: {time}</p>

      <button
        onClick={handleConfirm}
        className="mt-6 bg-purple-600 text-white px-4 py-2 rounded-lg"
      >
        Confirm Booking
      </button>
    </div>
  );
}
