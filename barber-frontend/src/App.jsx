import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";

import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Services from "./pages/Services";
import DateSelection from "./pages/DateSelection";
import TimeSelection from "./pages/TimeSelection";
import ConfirmBooking from "./pages/ConfirmBooking";
import AdminDashboard from "./pages/AdminDashboard";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login />} />

          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />

          <Route
            path="/booking/services"
            element={
              <ProtectedRoute>
                <Services />
              </ProtectedRoute>
            }
          />

          <Route
            path="/booking/date"
            element={
              <ProtectedRoute>
                <DateSelection />
              </ProtectedRoute>
            }
          />

          <Route
            path="/booking/time"
            element={
              <ProtectedRoute>
                <TimeSelection />
              </ProtectedRoute>
            }
          />

          <Route
            path="/booking/confirm"
            element={
              <ProtectedRoute>
                <ConfirmBooking />
              </ProtectedRoute>
            }
          />

          <Route
            path="/admin"
            element={
              <ProtectedRoute>
                <AdminDashboard />
              </ProtectedRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
