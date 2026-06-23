import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [bookings, setBookings] = useState([]);

  const loadBookings = async () => {
    try {
      const res = await fetch(
        `http://localhost:5078/api/bookings/user/${user.id}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (!res.ok) throw new Error("Failed to load bookings");

      const data = await res.json();
      setBookings(data);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    loadBookings();
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const cancelBooking = async (id) => {
    if (!confirm("Are you sure you want to cancel this booking?")) return;

    try {
      await fetch(`http://localhost:5078/api/bookings/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });

      loadBookings();
    } catch (err) {
      console.error("Failed to cancel booking", err);
    }
  };

  return (
    <div className="p-10 max-w-4xl mx-auto">

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
        onClick={() => navigate("/booking")}
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
            key={`${b.bookingId}-${b.start}`}
            className="border p-4 rounded bg-gray-50 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-3"
          >
            <div>
              <div className="font-medium">{b.serviceName}</div>
              <div className="text-sm text-gray-600">
                {new Date(b.start).toLocaleString()}
              </div>
              <div className="text-sm text-gray-600">
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
