import { useEffect, useState } from "react";

export default function AdminOverview() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadStats = async () => {
      try {
        const res = await fetch("http://localhost:5078/api/admin/stats", {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        });

        if (!res.ok) {
          throw new Error("Failed to load admin statistics");
        }

        const data = await res.json();
        setStats(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadStats();
  }, []);

  if (loading) return <p>Loading overview...</p>;
  if (!stats) return <p>Failed to load admin statistics.</p>;

  const items = [
    { label: "Total Bookings", value: stats.totalBookings },
    { label: "Total Customers", value: stats.totalCustomers },
    { label: "Total Barbers", value: stats.totalBarbers },
    { label: "Total Services", value: stats.totalServices },
    { label: "Today's Bookings", value: stats.todayBookings },
    { label: "Upcoming Bookings", value: stats.upcomingBookings },
  ];

  return (
    <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
      {items.map((item) => (
        <div
          key={item.label}
          className="p-6 bg-white shadow rounded-lg border"
        >
          <div className="text-gray-600">{item.label}</div>
          <div className="text-3xl font-semibold">{item.value}</div>
        </div>
      ))}
    </div>
  );
}
