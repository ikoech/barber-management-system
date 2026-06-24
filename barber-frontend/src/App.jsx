// src/App.jsx
import { Routes, Route } from "react-router-dom";
import { useAuth } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";

import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";

import Services from "./pages/booking/Services";
import DateSelection from "./pages/booking/DateSelection";
import TimeSelection from "./pages/booking/TimeSelection";
import ConfirmBooking from "./pages/booking/ConfirmBooking";
import CustomerBookings from "./pages/booking/CustomerBookings";

import AdminDashboard from "./pages/admin/AdminDashboard";

import BarberDashboard from "./pages/barber/BarberDashboard";
import DaysOff from "./pages/barber/DaysOff";
import WorkingHours from "./pages/barber/WorkingHours";
import Breaks from "./pages/barber/Breaks";
import Calendar from "./pages/barber/Calendar";

import LandingPage from "./LandingPage";
import About from "./pages/info/About";
import Terms from "./pages/info/Terms";
import Contact from "./pages/info/Contact";

export default function App() {
  const { authFetch, currentUser } = useAuth();

  return (
    <div className="app-theme">
      <Routes>
        {/* PUBLIC */}
        <Route path="/" element={<LandingPage />} />
        <Route path="/about" element={<About />} />
        <Route path="/terms" element={<Terms />} />
        <Route path="/contact" element={<Contact />} />

        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* CUSTOMER */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute role="Customer">
              <Dashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/booking/services"
          element={
            <ProtectedRoute role="Customer">
              <Services authFetch={authFetch} currentUser={currentUser} />
            </ProtectedRoute>
          }
        />

        <Route
          path="/booking/date"
          element={
            <ProtectedRoute role="Customer">
              <DateSelection />
            </ProtectedRoute>
          }
        />

        <Route
          path="/booking/time"
          element={
            <ProtectedRoute role="Customer">
              <TimeSelection authFetch={authFetch} />
            </ProtectedRoute>
          }
        />

        <Route
          path="/booking/confirm"
          element={
            <ProtectedRoute role="Customer">
              <ConfirmBooking
                authFetch={authFetch}
                currentUser={currentUser}
              />
            </ProtectedRoute>
          }
        />

        <Route
          path="/bookings/my"
          element={
            <ProtectedRoute role="Customer">
              <CustomerBookings
                authFetch={authFetch}
                currentUser={currentUser}
              />
            </ProtectedRoute>
          }
        />

        {/* ADMIN */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute role="Admin">
              <AdminDashboard />
            </ProtectedRoute>
          }
        />

        {/* BARBER */}
        <Route
          path="/barber/dashboard"
          element={
            <ProtectedRoute role="Barber">
              <BarberDashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/barber/daysoff"
          element={
            <ProtectedRoute role="Barber">
              <DaysOff />
            </ProtectedRoute>
          }
        />

        <Route
          path="/barber/working-hours"
          element={
            <ProtectedRoute role="Barber">
              <WorkingHours />
            </ProtectedRoute>
          }
        />

        <Route
          path="/barber/breaks"
          element={
            <ProtectedRoute role="Barber">
              <Breaks />
            </ProtectedRoute>
          }
        />

        <Route
          path="/barber/calendar"
          element={
            <ProtectedRoute role="Barber">
              <Calendar />
            </ProtectedRoute>
          }
        />
      </Routes>
    </div>
  );
}
