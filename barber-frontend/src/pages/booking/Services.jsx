// src/pages/booking/Services.jsx
import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getServices } from "../../api/services";
import { getBarbers } from "../../api/barber";
import { useAuth } from "../../context/AuthContext";

export default function Services() {
  const navigate = useNavigate();
  const { authFetch, loading: authLoading } = useAuth();

  const [services, setServices] = useState([]);
  const [barbers, setBarbers] = useState([]);
  const [serviceId, setServiceId] = useState("");
  const [barberId, setBarberId] = useState("");

  const safeAuthReady = useMemo(() => !authLoading && typeof authFetch === "function", [authLoading, authFetch]);

  useEffect(() => {
    if (!safeAuthReady) return;

    (async () => {
      try {
        const [svc, brs] = await Promise.all([
          getServices(authFetch).catch(() => []),
          getBarbers(authFetch).catch(() => []),
        ]);

        // Hard normalize even if backend is broken
        setServices(Array.isArray(svc) ? svc : []);
        setBarbers(Array.isArray(brs) ? brs : []);
      } catch {
        setServices([]);
        setBarbers([]);
      }
    })();
  }, [safeAuthReady, authFetch]);

  function handleNext() {
    if (!serviceId || !barberId) return;
    navigate("/booking/date", {
      state: { serviceId: Number(serviceId), barberId: Number(barberId) },
    });
  }

  return (
    <div className="page-container">
      <h1>Select Service & Barber</h1>

      {authLoading ? (
        <div>Loading…</div>
      ) : (
        <>
          <div>
            <h2>Service</h2>
            <select value={serviceId} onChange={(e) => setServiceId(e.target.value)}>
              <option value="">Select service</option>
              {Array.isArray(services) &&
                services.map((s) => (
                  <option key={s?.id ?? s?.Id} value={s?.id ?? s?.Id ?? ""}>
                    {s?.name ?? s?.Name ?? "(Unnamed service)"}
                  </option>
                ))}
            </select>
          </div>

          <div>
            <h2>Barber</h2>
            <select value={barberId} onChange={(e) => setBarberId(e.target.value)}>
              <option value="">Select barber</option>
              {Array.isArray(barbers) &&
                barbers.map((b) => (
                  <option key={b?.id ?? b?.Id} value={b?.id ?? b?.Id ?? ""}>
                    {b?.fullName ?? b?.FullName ?? "(Unknown Barber)"}
                  </option>
                ))}
            </select>
          </div>

          <button onClick={handleNext}>Next</button>
        </>
      )}
    </div>
  );
}
