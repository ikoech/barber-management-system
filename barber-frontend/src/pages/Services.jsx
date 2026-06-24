import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getServices } from "../../api/services";
import { getBarbers } from "../../api/barber";
import { useAuth } from "../../context/AuthContext";

export default function Services() {
  const { authFetch, loading: authLoading } = useAuth(); //  wait for token
  const navigate = useNavigate();

  const [services, setServices] = useState([]);
  const [barbers, setBarbers] = useState([]);

  const [serviceId, setServiceId] = useState("");
  const [barberId, setBarberId] = useState("");

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (authLoading) return; // DO NOT FETCH UNTIL TOKEN IS READY

    async function load() {
      try {
        const s = await getServices(authFetch);
        const b = await getBarbers(authFetch);

        console.log("RAW BARBERS FROM API:", b);

        // ⭐ PERMANENT SANITIZATION
        const safeBarbers = (Array.isArray(b) ? b : [])
          .filter((item) => item && typeof item === "object")
          .map((item) => ({
            ...item,
            fullName: item.fullName || item.FullName || "(Unknown Barber)",
          }));

        setServices(Array.isArray(s) ? s : []);
        setBarbers(safeBarbers);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [authLoading]); // run only after auth is ready

  function handleNext() {
    if (!serviceId || !barberId) return;

    navigate("/booking/date", {
      state: {
        serviceId: Number(serviceId),
        barberId: Number(barberId),
      },
    });
  }

  if (loading || authLoading) {
    return <div className="page-container">Loading...</div>;
  }

  return (
    <div className="page-container max-w-xl mx-auto">
      <h1 className="text-2xl font-semibold mb-6">Select Service & Barber</h1>

      {/* SERVICE SELECT */}
      <div className="mb-6">
        <label className="block mb-2 font-medium">Service</label>
        <select
          className="w-full p-3 rounded border"
          value={serviceId}
          onChange={(e) => setServiceId(e.target.value)}
        >
          <option value="">Select service</option>

          {services.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name} — {s.duration} min — {s.price} SEK
            </option>
          ))}
        </select>
      </div>

      {/* BARBER SELECT */}
      <div className="mb-6">
        <label className="block mb-2 font-medium">Barber</label>
        <select
          className="w-full p-3 rounded border"
          value={barberId}
          onChange={(e) => setBarberId(e.target.value)}
        >
          <option value="">Select barber</option>

          {barbers.map((b, index) => (
            <option key={b.id ?? index} value={b.id}>
              {b.fullName}
            </option>
          ))}
        </select>
      </div>

      {/* NEXT BUTTON */}
      <button
        onClick={handleNext}
        disabled={!serviceId || !barberId}
        className="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700 disabled:opacity-50"
      >
        Next
      </button>
    </div>
  );
}
