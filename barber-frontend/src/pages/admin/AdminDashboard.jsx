import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

import AdminOverview from "./AdminOverview";
import AdminBookings from "./AdminBookings";
import AdminServices from "./AdminServices";
import AdminBarbers from "./AdminBarbers";
import AdminUsers from "./AdminUsers";

export default function AdminDashboard() {
  const [tab, setTab] = useState("overview");
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const tabs = [
    { id: "overview", label: "Overview" },
    { id: "bookings", label: "Bookings" },
    { id: "services", label: "Services" },
    { id: "barbers", label: "Barbers" },
    { id: "users", label: "Users" },
  ];

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="p-10 max-w-7xl mx-auto">

      {/* Header */}
      <div className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-semibold">Admin Dashboard</h1>

          <p className="text-gray-600 flex items-center gap-2">
            Logged in as: {user?.email}
            <span className="px-2 py-1 bg-purple-100 text-purple-700 rounded text-xs">
              ADMIN PANEL
            </span>
          </p>
        </div>

        <div className="flex gap-3">
          <button
            onClick={() => navigate("/dashboard")}
            className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
          >
            Back
          </button>

          <button
            onClick={handleLogout}
            className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Sign Out
          </button>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-4 border-b mb-6">
        {tabs.map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`pb-2 px-3 border-b-2 ${
              tab === t.id
                ? "border-purple-600 text-purple-600 font-semibold"
                : "border-transparent text-gray-600 hover:text-black"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {/* Tab Content */}
      {tab === "overview" && <AdminOverview />}
      {tab === "bookings" && <AdminBookings />}
      {tab === "services" && <AdminServices />}
      {tab === "barbers" && <AdminBarbers />}
      {tab === "users" && <AdminUsers />}
    </div>
  );
}
