import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function BarberDashboard() {
  const { user, authFetch, logout } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user || user.role !== "Barber") return;

    const loadBookings = async () => {
      try {
        const res = await authFetch(
          `http://localhost:5078/api/bookings/barber/${user.barberId}`
        );

        if (!res.ok) throw new Error("Failed to load bookings");

        const data = await res.json();
        setBookings(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadBookings();
  }, [user]);

  if (!user) return <p>Loading...</p>;

  return (
    <div className="p-10 max-w-3xl mx-auto">
      <h1 className="text-3xl font-semibold mb-6">
        Barber Dashboard — {user.fullName}
      </h1>

      <div className="flex gap-4 mb-8">
        <button
          onClick={logout}
          className="bg-gray-300 px-4 py-2 rounded-lg hover:bg-gray-400"
        >
          Logout
        </button>
      </div>

      <h2 className="text-xl font-semibold mb-4">Your Schedule</h2>

      {loading && <p>Loading...</p>}

      {!loading && bookings.length === 0 && (
        <p className="text-gray-600">You have no upcoming bookings.</p>
      )}

      <div className="space-y-4">
        {bookings.map((b) => (
          <div
            key={b.id}
            className="p-4 border rounded-lg shadow-sm bg-white"
          >
            <div className="font-medium">{b.serviceName}</div>

            <div className="text-gray-600 text-sm">
              {new Date(b.start).toLocaleString()}
            </div>

            <div className="text-gray-600 text-sm">
              Customer: {b.customerName}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
