import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { getCalendar } from "../../api/calendar";
import "./Calendar.css";

export default function Calendar() {
  const { user, authFetch } = useAuth();

  const [selectedMonth, setSelectedMonth] = useState(() => {
    const now = new Date();
    const m = String(now.getMonth() + 1).padStart(2, "0");
    return `${now.getFullYear()}-${m}`;
  });

  const [calendar, setCalendar] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!user?.barberId) return;

    const load = async () => {
      setLoading(true);
      const data = await getCalendar(user.barberId, selectedMonth, authFetch);
      setCalendar(data);
      setLoading(false);
    };

    load();
  }, [selectedMonth, user?.barberId]);

  const handleMonthChange = (e) => setSelectedMonth(e.target.value);

  if (!calendar) return <p>Loading...</p>;

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
              className={`calendar-cell ${
                day.isDayOff ? "day-off" : ""
              }`}
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
