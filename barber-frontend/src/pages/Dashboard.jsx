import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function Dashboard() {
  const { user, authFetch, logout } = useAuth();
  const navigate = useNavigate();
  const [bookings, setBookings] = useState([]);

  async function loadBookings() {
    try {
      const res = await authFetch(
        `http://localhost:5078/api/bookings/user/${user.id}`
      );

      if (!res.ok) throw new Error("Failed to load bookings");

      const data = await res.json();
      setBookings(data);
    } catch (err) {
      console.error(err);
    }
  }

  useEffect(() => {
    if (user) loadBookings();
  }, [user]);

  async function cancelBooking(id) {
    if (!confirm("Are you sure you want to cancel this booking?")) return;

    try {
      const res = await authFetch(`http://localhost:5078/api/bookings/${id}`, {
        method: "DELETE",
      });

      if (!res.ok) throw new Error("Failed to cancel booking");

      loadBookings();
    } catch (err) {
      console.error(err);
    }
  }

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div className="page-container max-w-4xl mx-auto">

      {/* Header */}
      <div className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-semibold">Welcome, {user?.email}</h1>

          {user.role === "Admin" && (
            <span className="px-2 py-1 bg-gray-200 text-gray-700 rounded text-xs">
              CUSTOMER VIEW
            </span>
          )}
        </div>

        <div className="flex gap-3">
          {user.role === "Admin" && (
            <button
              onClick={() => navigate("/admin")}
              className="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700"
            >
              Admin Dashboard
            </button>
          )}

          <button
            onClick={handleLogout}
            className="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Logout
          </button>
        </div>
      </div>

      {/* Start Booking */}
      <button
        onClick={() => navigate("/booking/services")}
        className="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700 mb-8"
      >
        Start Booking
      </button>

      {/* Upcoming Bookings */}
      <h2 className="text-xl font-semibold mb-4">Your Upcoming Bookings</h2>

      {bookings.length === 0 && (
        <p className="text-gray-500">You have no upcoming bookings.</p>
      )}

      <div className="space-y-4">
        {bookings.map((b) => (
          <div
            key={b.bookingId}
            className="card p-4 flex flex-col md:flex-row md:items-center md:justify-between gap-3"
          >
            <div>
              <div className="font-medium">{b.serviceName}</div>
              <div className="text-sm opacity-80">
                {new Date(b.start).toLocaleString()}
              </div>
              <div className="text-sm opacity-80">
                Barber: {b.barberName}
              </div>
            </div>

            <button
              onClick={() => cancelBooking(b.bookingId)}
              className="px-3 py-1 bg-red-600 text-white rounded hover:bg-red-700"
            >
              Cancel Booking
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
