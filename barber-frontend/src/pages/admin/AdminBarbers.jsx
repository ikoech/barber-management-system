import { useEffect, useState } from "react";

export default function AdminBarbers() {
  const [barbers, setBarbers] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  const [newBarber, setNewBarber] = useState({
    userId: "",
    specialization: "",
  });

  const [editingBarber, setEditingBarber] = useState(null);

  const token = localStorage.getItem("token");

  // Load barbers + users
  const loadData = async () => {
    try {
      setLoading(true);

        const [barbersRes, usersRes] = await Promise.all([
        fetch("http://localhost:5078/api/barbers", {
            headers: { Authorization: `Bearer ${token}` },
        }),
        fetch("http://localhost:5078/api/users", {
            headers: { Authorization: `Bearer ${token}` },
        }),
        ]);
        
      setBarbers(await barbersRes.json());
      setUsers(await usersRes.json());
    } catch (err) {
      console.error("Failed to load barbers", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // Create new barber
  const createBarber = async () => {
    try {
      await fetch("http://localhost:5078/api/barbers", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newBarber),
      });

      // Update user role to Barber
      await fetch(
        `http://localhost:5078/api/admin/users/${newBarber.userId}/role`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify("Barber"),
        }
      );

      setNewBarber({ userId: "", specialization: "" });
      loadData();
    } catch (err) {
      console.error("Failed to create barber", err);
    }
  };

  // Update barber
  const updateBarber = async () => {
    try {
      await fetch(
        `http://localhost:5078/api/barbers/${editingBarber.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            specialization: editingBarber.specialization,
          }),
        }
      );

      setEditingBarber(null);
      loadData();
    } catch (err) {
      console.error("Failed to update barber", err);
    }
  };

  // Delete barber
  const deleteBarber = async (id) => {
    if (!confirm("Are you sure you want to remove this barber?")) return;

    try {
      await fetch(`http://localhost:5078/api/barbers/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      loadData();
    } catch (err) {
      console.error("Failed to delete barber", err);
    }
  };

  return (
    <div className="space-y-10">

      {/* Add Barber */}
      <div className="p-6 bg-white border rounded-lg shadow-sm">
        <h2 className="text-xl font-semibold mb-4">Add New Barber</h2>

        <div className="grid md:grid-cols-2 gap-4">
          <select
            className="border p-2 rounded"
            value={newBarber.userId}
            onChange={(e) =>
              setNewBarber({ ...newBarber, userId: e.target.value })
            }
          >
            <option value="">Select User</option>
            {users
              .filter((u) => u.role === "Customer")
              .map((u) => (
                <option key={u.id} value={u.id}>
                  {u.fullName} ({u.email})
                </option>
              ))}
          </select>

          <input
            placeholder="Specialization"
            className="border p-2 rounded"
            value={newBarber.specialization}
            onChange={(e) =>
              setNewBarber({
                ...newBarber,
                specialization: e.target.value,
              })
            }
          />
        </div>

        <button
          onClick={createBarber}
          className="mt-4 bg-purple-600 text-white px-4 py-2 rounded"
        >
          Add Barber
        </button>
      </div>

      {/* Barbers List */}
      <div className="p-6 bg-white border rounded-lg shadow-sm">
        <h2 className="text-xl font-semibold mb-4">All Barbers</h2>

        {loading && <p>Loading barbers...</p>}

        {!loading && barbers.length === 0 && (
          <p className="text-gray-500">No barbers found.</p>
        )}

        <div className="space-y-4">
          {barbers.map((b) => (
            <div
              key={b.id}
              className="border p-4 rounded bg-gray-50 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-3"
            >
              {/* Display Mode */}
              {!editingBarber || editingBarber.id !== b.id ? (
                <>
                  <div>
                    <div className="font-medium">{b.name}</div>
                    <div className="text-sm text-gray-600">{b.email}</div>
                    <div className="text-sm text-gray-600">
                      Specialization: {b.specialization}
                    </div>
                  </div>

                  <div className="flex gap-3">
                    <button
                      onClick={() => setEditingBarber(b)}
                      className="px-3 py-1 bg-blue-500 text-white rounded"
                    >
                      Edit
                    </button>

                    <button
                      onClick={() => deleteBarber(b.id)}
                      className="px-3 py-1 bg-red-500 text-white rounded"
                    >
                      Delete
                    </button>
                  </div>
                </>
              ) : (
                /* Edit Mode */
                <div className="w-full space-y-3">
                  <input
                    className="border p-2 rounded w-full"
                    value={editingBarber.specialization}
                    onChange={(e) =>
                      setEditingBarber({
                        ...editingBarber,
                        specialization: e.target.value,
                      })
                    }
                  />

                  <div className="flex gap-3">
                    <button
                      onClick={updateBarber}
                      className="px-3 py-1 bg-green-600 text-white rounded"
                    >
                      Save
                    </button>

                    <button
                      onClick={() => setEditingBarber(null)}
                      className="px-3 py-1 bg-gray-400 text-white rounded"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
