import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { getCalendar } from "../../api/calendar";

export default function Calendar() {
  const { user, authFetch } = useAuth();

  const [selectedMonth, setSelectedMonth] = useState(() => {
    const now = new Date();
    const m = String(now.getMonth() + 1).padStart(2, "0");
    return `${now.getFullYear()}-${m}`; // "YYYY-MM"
  });

  const [calendar, setCalendar] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!user?.barberId) return;

    const load = async () => {
      try {
        setLoading(true);
        setError("");
        const data = await getCalendar(user.barberId, selectedMonth, authFetch);
        setCalendar(data);
      } catch (err) {
        console.error(err);
        setError("Could not load calendar.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [user?.barberId, selectedMonth, authFetch]);

  const handleMonthChange = (e) => {
    setSelectedMonth(e.target.value); // "YYYY-MM"
  };

  if (!user) return <p>Loading user...</p>;

  return (
    <div className="page">
      <div className="page-header">
        <button onClick={() => window.history.back()}>&larr; Back</button>
        <h1>Calendar</h1>
      </div>

      <div style={{ marginBottom: "1rem" }}>
        <label>
          Month:{" "}
          <input
            type="month"
            value={selectedMonth}
            onChange={handleMonthChange}
          />
        </label>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      {!loading && !error && calendar && (
        <div className="calendar-grid">
          {calendar.days.map((day) => (
            <div
              key={day.date}
              className={`calendar-day ${
                day.isDayOff ? "calendar-day--off" : ""
              }`}
            >
              <div className="calendar-day-header">
                {new Date(day.date).getDate()}
                {day.isDayOff && <span className="badge">Day off</span>}
              </div>

              {!day.isDayOff && day.bookings.length > 0 && (
                <ul className="calendar-bookings">
                  {day.bookings.map((b) => (
                    <li key={b.id}>
                      {new Date(b.start).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}{" "}
                      -{" "}
                      {new Date(b.end).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </li>
                  ))}
                </ul>
              )}

              {!day.isDayOff && day.bookings.length === 0 && (
                <div className="calendar-empty">No bookings</div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
