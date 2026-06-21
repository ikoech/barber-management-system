import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function AdminDashboard() {
  const { user, authFetch } = useAuth();
  const [services, setServices] = useState([]);
  const [barbers, setBarbers] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      try {
        const [servicesRes, barbersRes, bookingsRes] = await Promise.all([
          authFetch("http://localhost:5078/api/services"),
          authFetch("http://localhost:5078/api/barbers"),
          authFetch("http://localhost:5078/api/bookings"),
        ]);

        const servicesData = await servicesRes.json();
        const barbersData = await barbersRes.json();
        const bookingsData = await bookingsRes.json();

        setServices(servicesData);
        setBarbers(barbersData);
        setBookings(bookingsData);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  if (user.role !== "Admin") {
    return (
      <div className="p-10 text-red-600 font-semibold">
        You do not have permission to view this page.
      </div>
    );
  }

  return (
    <div className="p-10 max-w-6xl mx-auto space-y-10">
      <h1 className="text-3xl font-semibold mb-4">Admin Dashboard</h1>
      <p className="text-gray-600 mb-6">
        Logged in as: {user.email} ({user.role})
      </p>

      {loading && <p>Loading admin data...</p>}

      {!loading && (
        <>
          <section>
            <h2 className="text-2xl font-semibold mb-3">Services</h2>
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
              {services.map((s) => (
                <div key={s.id} className="border rounded-lg p-4 bg-white shadow-sm">
                  <div className="font-medium">{s.name}</div>
                  <div className="text-sm text-gray-600">
                    Duration: {s.durationMinutes} min
                  </div>
                  <div className="text-sm text-gray-600">
                    Price: {s.price} kr
                  </div>
                </div>
              ))}
            </div>
          </section>

          <section>
            <h2 className="text-2xl font-semibold mb-3">Barbers</h2>
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
              {barbers.map((b) => (
                <div key={b.id} className="border rounded-lg p-4 bg-white shadow-sm">
                  <div className="font-medium">{b.name}</div>
                  <div className="text-sm text-gray-600">Email: {b.email}</div>
                </div>
              ))}
            </div>
          </section>

          <section>
            <h2 className="text-2xl font-semibold mb-3">All Bookings</h2>
            <div className="space-y-3">
              {bookings.map((bk) => (
                <div
                  key={bk.id}
                  className="border rounded-lg p-4 bg-white shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-2"
                >
                  <div>
                    <div className="font-medium">
                      {bk.serviceName} — {bk.barberName}
                    </div>
                    <div className="text-sm text-gray-600">
                      Customer: {bk.customerName} ({bk.customerEmail})
                    </div>
                    <div className="text-sm text-gray-600">
                      {new Date(bk.start).toLocaleString()}
                    </div>
                  </div>
                  <div className="text-sm text-gray-500">Status: {bk.status}</div>
                </div>
              ))}
            </div>
          </section>
        </>
      )}
    </div>
  );
}
