import { useEffect, useState } from "react";

export default function AdminBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  const [filters, setFilters] = useState({
    barberId: "",
    userId: "",
    serviceId: "",
    date: "",
  });

  const loadBookings = async () => {
    try {
      setLoading(true);

      const params = new URLSearchParams();
      Object.entries(filters).forEach(([key, value]) => {
        if (value) params.append(key, value);
      });

      const res = await fetch(
        `http://localhost:5078/api/admin/bookings?${params.toString()}`,
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
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBookings();
  }, []);

  const handleFilterChange = (e) => {
    setFilters({ ...filters, [e.target.name]: e.target.value });
  };

  const applyFilters = () => loadBookings();

  return (
    <div className="space-y-6">

      {/* Filters */}
      <div className="p-4 bg-white border rounded-lg shadow-sm">
        <h2 className="text-lg font-semibold mb-3">Filters</h2>

        <div className="grid md:grid-cols-4 gap-4">
          <input
            name="barberId"
            placeholder="Barber ID"
            className="border p-2 rounded"
            value={filters.barberId}
            onChange={handleFilterChange}
          />
          <input
            name="userId"
            placeholder="User ID"
            className="border p-2 rounded"
            value={filters.userId}
            onChange={handleFilterChange}
          />
          <input
            name="serviceId"
            placeholder="Service ID"
            className="border p-2 rounded"
            value={filters.serviceId}
            onChange={handleFilterChange}
          />
          <input
            type="date"
            name="date"
            className="border p-2 rounded"
            value={filters.date}
            onChange={handleFilterChange}
          />
        </div>

        <button
          onClick={applyFilters}
          className="mt-3 bg-purple-600 text-white px-4 py-2 rounded"
        >
          Apply Filters
        </button>
      </div>

      {/* Bookings Table */}
      <div className="bg-white border rounded-lg shadow-sm p-4">
        <h2 className="text-lg font-semibold mb-3">All Bookings</h2>

        {loading && <p>Loading bookings...</p>}

        {!loading && bookings.length === 0 && (
          <p className="text-gray-500">No bookings found.</p>
        )}

        <div className="space-y-3">
          {bookings.map((b) => (
            <div
              key={b.bookingId}
              className="border rounded-lg p-4 bg-gray-50 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-2"
            >
              <div>
                <div className="font-medium">
                  {b.serviceName} — {b.barberName}
                </div>

                <div className="text-sm text-gray-600">
                  Customer: {b.userName} ({b.customerEmail})
                </div>

                <div className="text-sm text-gray-600">
                  {new Date(b.start).toLocaleString()}
                </div>
              </div>

              <div className="text-sm text-gray-500">
                Status: {b.status}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
