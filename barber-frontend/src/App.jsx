import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";

import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Services from "./pages/Services";
import DateSelection from "./pages/DateSelection";
import TimeSelection from "./pages/TimeSelection";
import ConfirmBooking from "./pages/ConfirmBooking";
import AdminDashboard from "./pages/admin/AdminDashboard";
import BarberDashboard from "./pages/barber/BarberDashboard";
import DaysOff from "./pages/barber/DaysOff";
import Register from "./pages/Register";
import WorkingHours from "./pages/barber/WorkingHours";
import Breaks from "./pages/barber/Breaks";
import Calendar from "./pages/barber/Calendar";
import LandingPage from "./LandingPage";
import About from "./pages/info/About";
import Terms from "./pages/info/Terms";
import Contact from "./pages/info/Contact";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>

          {/* Redirect root */}
            <Route path="/" element={<LandingPage />} />
            <Route path="/" element={<Navigate to="/login" />} />
            <Route path="/about" element={<About />} />
            <Route path="/terms" element={<Terms />} />
            <Route path="/contact" element={<Contact />} />

          {/* Public */}
          <Route path="/login" element={<ProtectedRoute><Login /></ProtectedRoute>} />
          <Route path="/register" element={<ProtectedRoute><Register /></ProtectedRoute>} />

          {/* CUSTOMER ROUTES */}
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
                <Services />
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
                <TimeSelection />
              </ProtectedRoute>
            }
          />

          <Route
            path="/booking/confirm"
            element={
              <ProtectedRoute role="Customer">
                <ConfirmBooking />
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
          <Route path="/" element={<LandingPage />} />

        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
