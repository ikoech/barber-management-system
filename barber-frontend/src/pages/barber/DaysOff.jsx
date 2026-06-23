import { useEffect, useState } from "react";
import { getDaysOff, createDayOff, deleteDayOff } from "../../api/barber";
import { useAuth } from "../../context/AuthContext";

export default function DaysOff() {
  const { user, authFetch } = useAuth();
  const [daysOff, setDaysOff] = useState([]);
  const [date, setDate] = useState("");
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!user?.barberId) return;
    loadDaysOff();
  }, [user]);

  async function loadDaysOff() {
    try {
      setLoading(true);
      setError("");

      const data = await getDaysOff(user.barberId, authFetch);
      setDaysOff(data);
    } catch (err) {
      console.error(err);
      setError("Failed to load days off.");
    } finally {
      setLoading(false);
    }
  }

  async function handleAdd() {
    if (!date) {
      setError("Please select a date.");
      return;
    }

    try {
      setError("");

      await createDayOff(
        {
          barberId: user.barberId,
          date,
          reason,
        },
        authFetch
      );

      setDate("");
      setReason("");
      loadDaysOff();
    } catch (err) {
      setError(err.message || "Failed to add day off.");
    }
  }

  async function handleDelete(id) {
    try {
      setError("");
      await deleteDayOff(id, authFetch);
      loadDaysOff();
    } catch (err) {
      setError("Failed to delete day off.");
    }
  }

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Manage Days Off</h1>

    <button
      onClick={() => window.history.back()}
      className="mb-4 bg-gray-300 px-4 py-2 rounded hover:bg-gray-400"
    >
      ← Back
    </button>

      {error && <p className="text-red-600 mb-3">{error}</p>}

      <div className="bg-gray-100 p-4 rounded mb-6">
        <h3 className="font-semibold mb-2">Add Day Off</h3>

        <input
          type="date"
          className="border p-2 rounded mr-2"
          value={date}
          onChange={(e) => setDate(e.target.value)}
        />

        <input
          type="text"
          className="border p-2 rounded mr-2"
          placeholder="Reason (optional)"
          value={reason}
          onChange={(e) => setReason(e.target.value)}
        />

        <button
          onClick={handleAdd}
          className="bg-blue-600 text-white px-4 py-2 rounded"
        >
          Add
        </button>
      </div>

      <h3 className="text-xl font-semibold mb-3">Your Days Off</h3>

      {loading ? (
        <p>Loading...</p>
      ) : daysOff.length === 0 ? (
        <p>You have no days off.</p>
      ) : (
        <ul className="space-y-3">
          {daysOff.map((d) => (
            <li
              key={d.id}
              className="bg-gray-100 p-3 rounded flex justify-between items-center"
            >
              <span>
                <strong>{new Date(d.date).toLocaleDateString("sv-SE")}</strong>{" "}
                — {d.reason || "No reason"}
              </span>

              <button
                onClick={() => handleDelete(d.id)}
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
