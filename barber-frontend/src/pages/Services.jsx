import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Services() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
  async function loadServices() {
    try {
      console.log("Fetching services...");
      const res = await fetch("http://localhost:5078/api/services");

      console.log("Status:", res.status);

      const text = await res.text();
      console.log("Response text:", text);

      if (!res.ok) {
        throw new Error("Failed to load services");
      }

      const data = JSON.parse(text);
      setServices(data);
    } catch (err) {
      console.error("Fetch error:", err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  loadServices();
}, []);

  if (loading) {
    return <div className="p-10 text-center text-lg">Loading services...</div>;
  }

  if (error) {
    return (
      <div className="p-10 text-center text-red-600 text-lg">
        {error}
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6 text-center">
        Choose a Service
      </h1>

      <div className="space-y-4">
        {services.map((service) => (
          <div
            key={service.id}
            className="p-4 border rounded-lg shadow hover:bg-gray-50 cursor-pointer"
            onClick={() =>
              navigate(`/booking/date?serviceId=${service.id}`)
            }
          >
            <div className="text-lg font-medium">{service.name}</div>
            <div className="text-sm text-gray-600">
              {service.duration} min • {service.price} SEK
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
