import { useEffect, useState } from "react";

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  const token = localStorage.getItem("token");

  const loadUsers = async () => {
    try {
      setLoading(true);

      const res = await fetch("http://localhost:5078/api/users", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) throw new Error("Failed to load users");

      const data = await res.json();
      setUsers(data);
    } catch (err) {
      console.error("Failed to load users", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

  const changeRole = async (userId, newRole) => {
    try {
      await fetch(`http://localhost:5078/api/admin/users/${userId}/role`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newRole),
      });

      loadUsers();
    } catch (err) {
      console.error("Failed to update user role", err);
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold">Manage Users</h2>

      {loading && <p>Loading users...</p>}

      {!loading && users.length === 0 && (
        <p className="text-gray-500">No users found.</p>
      )}

      <div className="space-y-4">
        {users.map((u) => (
          <div
            key={u.id}
            className="border p-4 rounded bg-gray-50 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-3"
          >
            <div>
              <div className="font-medium">{u.fullName}</div>
              <div className="text-sm text-gray-600">{u.email}</div>
              <div className="text-sm text-gray-500">Role: {u.role}</div>
            </div>

            <select
              className="border p-2 rounded"
              value={u.role}
              onChange={(e) => changeRole(u.id, e.target.value)}
            >
              <option value="Customer">Customer</option>
              <option value="Barber">Barber</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
        ))}
      </div>
    </div>
  );
}
