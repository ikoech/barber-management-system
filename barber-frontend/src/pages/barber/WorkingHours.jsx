import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import {
  getWorkingHours,
  createWorkingHours,
  deleteWorkingHours,
} from "../../api/workingHours";

export default function WorkingHours() {
  const { user, authFetch } = useAuth();
  const [hours, setHours] = useState([]);
  const [dayOfWeek, setDayOfWeek] = useState("Monday");
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user?.barberId) return;
    loadHours();
  }, [user]);

  async function loadHours() {
    try {
      setLoading(true);
      const data = await getWorkingHours(user.barberId, authFetch);
      setHours(data);
    } catch (err) {
      console.error(err);
      setError("Failed to load working hours.");
    } finally {
      setLoading(false);
    }
  }

  async function handleAdd() {
    if (!start || !end) {
      setError("Start and end times are required.");
      return;
    }

    try {
      setError("");

      await createWorkingHours(
        {
          BarberId: user.barberId,
          DayOfWeek: dayOfWeek,
          StartTime: `${start}:00`, // "09:00:00"
          EndTime: `${end}:00`,     // "15:00:00"
        },
        authFetch
      );

      setStart("");
      setEnd("");
      loadHours();
    } catch (err) {
      console.error("Backend error:", err);
      setError(err.message || "Failed to create working hours.");
    }
  }

  async function handleDelete(id) {
    try {
      setError("");
      await deleteWorkingHours(id, authFetch);
      loadHours();
    } catch (err) {
      console.error(err);
      setError("Failed to delete working hours.");
    }
  }

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Working Hours</h1>

      <button
        onClick={() => window.history.back()}
        className="mb-4 bg-gray-300 px-4 py-2 rounded hover:bg-gray-400"
      >
        ← Back
      </button>

      {error && <p className="text-red-600 mb-3">{error}</p>}

      <div className="bg-gray-100 p-4 rounded mb-6">
        <h3 className="font-semibold mb-2">Add Working Hours</h3>

        <select
          className="border p-2 rounded mr-2"
          value={dayOfWeek}
          onChange={(e) => setDayOfWeek(e.target.value)}
        >
          {[
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday",
          ].map((d) => (
            <option key={d} value={d}>
              {d}
            </option>
          ))}
        </select>

        <input
          type="time"
          className="border p-2 rounded mr-2"
          value={start}
          onChange={(e) => setStart(e.target.value)}
        />

        <input
          type="time"
          className="border p-2 rounded mr-2"
          value={end}
          onChange={(e) => setEnd(e.target.value)}
        />

        <button
          onClick={handleAdd}
          className="bg-green-600 text-white px-4 py-2 rounded"
        >
          Add
        </button>
      </div>

      <h3 className="text-xl font-semibold mb-3">Your Working Hours</h3>

      {loading ? (
        <p>Loading...</p>
      ) : hours.length === 0 ? (
        <p>No working hours set.</p>
      ) : (
        <ul className="space-y-3">
          {hours.map((h) => (
            <li
              key={h.id}
              className="bg-gray-100 p-3 rounded flex justify-between items-center"
            >
              <span>
                <strong>{h.dayOfWeek}</strong> — {h.startTime} to {h.endTime}
              </span>

              <button
                onClick={() => handleDelete(h.id)}
                className="text-red-600 hover:underline"
              >
                Delete
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
