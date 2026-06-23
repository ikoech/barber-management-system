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
import BarberDashboard from "./pages/BarberDashboard";
import Register from "./pages/Register";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>

          {/* Redirect root to login */}
          <Route path="/" element={<Navigate to="/login" />} />

          {/* Public routes */}
          <Route path="/login" element={<ProtectedRoute><Login /></ProtectedRoute>} />
          <Route path="/register" element={<ProtectedRoute><Register /></ProtectedRoute>} />

          {/* Customer */}
          <Route path="/dashboard" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />

          {/* Booking */}
          <Route path="/booking/services" element={<ProtectedRoute><Services /></ProtectedRoute>} />
          <Route path="/booking/date" element={<ProtectedRoute><DateSelection /></ProtectedRoute>} />
          <Route path="/booking/time" element={<ProtectedRoute><TimeSelection /></ProtectedRoute>} />
          <Route path="/booking/confirm" element={<ProtectedRoute><ConfirmBooking /></ProtectedRoute>} />

          {/* Admin */}
          <Route path="/admin" element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />

          {/* Barber */}
          <Route path="/barber/dashboard" element={<ProtectedRoute><BarberDashboard /></ProtectedRoute>} />

        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
