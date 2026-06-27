import { useEffect, useMemo, useRef, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { getCalendar } from "../../api/calendar";
import "./Calendar.css";

export default function Calendar() {
  const { user, authFetch, loading: authLoading } = useAuth();

  const [selectedMonth, setSelectedMonth] = useState(() => {
    const now = new Date();
    const m = String(now.getMonth() + 1).padStart(2, "0");
    return `${now.getFullYear()}-${m}`;
  });

  const barberId = useMemo(() => {
    const n = Number(user?.barberId);
    return Number.isFinite(n) && n > 0 ? n : null;
  }, [user?.barberId]);

  const monthToFullDate = (yyyyMm) => {
    if (typeof yyyyMm !== "string") return null;
    const m = yyyyMm.trim();
    if (!/^\d{4}-\d{2}$/.test(m)) return null;
    return `${m}-01`;
  };

  const [calendar, setCalendar] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const inFlight = useRef(false);

  useEffect(() => {
    if (authLoading) return;
    if (!user) return;
    if (!barberId) return;

    const fullDate = monthToFullDate(selectedMonth);
    if (!fullDate) {
      setCalendar(null);
      setError("Invalid selected month.");
      return;
    }

    if (inFlight.current) return;
    inFlight.current = true;

    setLoading(true);
    setError("");

    (async () => {
      try {
        const data = await getCalendar(barberId, fullDate, authFetch);
        setCalendar(data);
      } catch (e) {
        console.error(e);
        setCalendar(null);
        setError(e?.message || "Failed to load calendar.");
      } finally {
        setLoading(false);
        inFlight.current = false;
      }
    })();
  }, [authLoading, selectedMonth, barberId, user, authFetch]);

  const handleMonthChange = (e) => setSelectedMonth(e.target.value);

  if (authLoading) return <p>Loading...</p>;
  if (!user) return <p>Please log in again</p>;
  if (!barberId) {
    if (typeof window !== "undefined") window.location.href = "/login";
    return <p className="text-red-600 mb-3">Your session has expired. Please log in again.</p>;
  }

  if (error) return <p className="text-red-600 mb-3">{error}</p>;
  if (loading || !calendar) return <p>Loading...</p>;

  return (
    <div className="calendar-page">
      <div className="calendar-header">
        <button onClick={() => window.history.back()}>&larr; Back</button>
        <h1>Calendar</h1>

        <input
          type="month"
          value={selectedMonth}
          onChange={handleMonthChange}
          className="month-picker"
        />
      </div>

      <div className="calendar-grid">
        {calendar.days.map((day) => {
          const date = new Date(day.date);
          const dayNum = date.getDate();

          return (
            <div
              key={day.date}
              className={`calendar-cell ${day.isDayOff ? "day-off" : ""}`}
            >
              <div className="calendar-cell-header">
                <span className="day-number">{dayNum}</span>
                {day.isDayOff && <span className="badge-off">Day Off</span>}
              </div>

              {!day.isDayOff && day.bookings.length > 0 && (
                <div className="booking-list">
                  {day.bookings.map((b) => (
                    <div key={b.id} className="booking-item">
                      {new Date(b.start).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                      {" - "}
                      {new Date(b.end).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </div>
                  ))}
                </div>
              )}

              {!day.isDayOff && day.bookings.length === 0 && (
                <div className="no-bookings">No bookings</div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

