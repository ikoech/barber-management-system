import { useEffect, useMemo, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { getBreaks, createBreak, deleteBreak } from "../../api/breaks";

export default function Breaks() {
  const { user, authFetch } = useAuth();

  const [breaks, setBreaks] = useState([]);
  const [dayOfWeek, setDayOfWeek] = useState("Monday");
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const barberId = useMemo(() => {
    const n = Number(user?.barberId);
    return Number.isFinite(n) && n > 0 ? n : null;
  }, [user?.barberId]);

  useEffect(() => {
    if (!barberId) return;
    loadBreaks();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [barberId]);

  async function loadBreaks() {
    if (!barberId) return;
    setLoading(true);
    setError("");
    try {
      const data = await getBreaks(barberId, authFetch);
      setBreaks(Array.isArray(data) ? data : []);
    } catch (e) {
      console.error(e);
      setError("Failed to load breaks.");
    } finally {
      setLoading(false);
    }
  }

  async function handleAdd() {
    if (!user || !barberId) {
      setError("Your session has expired. Please log in again.");
      if (typeof window !== "undefined") window.location.href = "/login";
      return;
    }

    if (!start || !end) {
      setError("Start and end are required.");
      return;
    }

    const startDate = new Date(start);
    const endDate = new Date(end);

    if (Number.isNaN(startDate.getTime()) || Number.isNaN(endDate.getTime())) {
      setError("Invalid start/end date.");
      return;
    }

    if (endDate <= startDate) {
      setError("End must be after Start.");
      return;
    }

    try {
      setError("");
      setLoading(true);

      await createBreak(
        {
          barberId,
          dayOfWeek,
          start: startDate,
          end: endDate,
        },
        authFetch
      );

      setStart("");
      setEnd("");
      await loadBreaks();
    } catch (err) {
      console.error("Backend error:", err);
      setError(err?.message || "Failed to create break.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    try {
      setError("");
      await deleteBreak(id, authFetch);
      await loadBreaks();
    } catch (err) {
      console.error(err);
      setError("Failed to delete break.");
    }
  }

  if (!user || !barberId) {
    // Hard requirement: do not render barber pages when barberId invalid.
    return (
      <div className="container mx-auto p-6">
        <p className="text-red-600 mb-3">Your session has expired. Please log in again.</p>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Breaks</h1>

      <button
        onClick={() => window.history.back()}
        className="mb-4 bg-gray-300 px-4 py-2 rounded hover:bg-gray-400"
      >
        ← Back
      </button>

      {error && <p className="text-red-600 mb-3">{error}</p>}

      <div className="bg-gray-100 p-4 rounded mb-6">
        <h3 className="font-semibold mb-2">Add Break</h3>

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
          type="datetime-local"
          className="border p-2 rounded mr-2"
          value={start}
          onChange={(e) => setStart(e.target.value)}
        />

        <input
          type="datetime-local"
          className="border p-2 rounded mr-2"
          value={end}
          onChange={(e) => setEnd(e.target.value)}
        />

        <button
          onClick={handleAdd}
          className="bg-purple-600 text-white px-4 py-2 rounded"
        >
          Add
        </button>
      </div>

      <h3 className="text-xl font-semibold mb-3">Your Breaks</h3>

      {loading ? (
        <p>Loading...</p>
      ) : breaks.length === 0 ? (
        <p>No breaks set.</p>
      ) : (
        <ul className="space-y-3">
          {breaks.map((b) => (
            <li
              key={b.id}
              className="bg-gray-100 p-3 rounded flex justify-between items-center"
            >
              <span>
                <strong>{b.dayOfWeek}</strong> — {new Date(b.start).toLocaleString("sv-SE")} to{" "}
                {new Date(b.end).toLocaleString("sv-SE")}
              </span>

              <button
                onClick={() => handleDelete(b.id)}
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

