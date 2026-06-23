import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { getBarberBookings } from "../../api/barber";

export default function BarberDashboard() {
  const { user, authFetch, logout, loading: authLoading } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  if (authLoading) return <p>Loading authentication...</p>;
  if (!user) return <p>You must log in.</p>;

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const data = await getBarberBookings(user.barberId, authFetch);
      setBookings(data);
      setLoading(false);
    };

    load();
  }, [user]);

  return (
    <div className="p-10 max-w-3xl mx-auto">
      <h1 className="text-3xl font-semibold mb-6">
        Barber Dashboard — {user.fullName}
      </h1>

      <div className="flex flex-wrap gap-4 mb-8">
        <button
          onClick={logout}
          className="bg-gray-300 px-4 py-2 rounded-lg hover:bg-gray-400"
        >
          Logout
        </button>

        <Link to="/barber/daysoff" className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600">
          Manage Days Off
        </Link>

        <Link to="/barber/working-hours" className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600">
          Working Hours
        </Link>

        <Link to="/barber/breaks" className="bg-purple-500 text-white px-4 py-2 rounded-lg hover:bg-purple-600">
          Breaks
        </Link>

        <Link to="/barber/calendar" className="bg-orange-500 text-white px-4 py-2 rounded-lg hover:bg-orange-600">
          Calendar
        </Link>
      </div>

      <h2 className="text-xl font-semibold mb-4">Your Schedule</h2>

      {loading && <p>Loading bookings...</p>}

      {!loading && bookings.length === 0 && (
        <p className="text-gray-600">You have no upcoming bookings.</p>
      )}

      <div className="space-y-4">
        {bookings.map((b) => (
          <div key={b.id} className="p-4 border rounded-lg shadow-sm bg-white">
            <div className="font-medium">{b.serviceName}</div>
            <div className="text-gray-600 text-sm">{new Date(b.start).toLocaleString()}</div>
            <div className="text-gray-600 text-sm">Customer: {b.customerName}</div>
          </div>
        ))}
      </div>
    </div>
  );
}
