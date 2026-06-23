import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function ProtectedRoute({ children, role }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  const publicRoutes = ["/login", "/register"];

  if (loading) return <div>Loading...</div>;

  // Allow login/register when logged out
  if (!user && publicRoutes.includes(location.pathname)) {
    return children;
  }

  // Block login/register when logged in
  if (user && publicRoutes.includes(location.pathname)) {
    if (user.role === "Admin") return <Navigate to="/admin" />;
    if (user.role === "Barber") return <Navigate to="/barber/dashboard" />;
    return <Navigate to="/dashboard" />;
  }

  // Block protected routes when logged out
  if (!user) return <Navigate to="/login" />;

  // ⭐ NEW: Role-based protection
  if (role && user.role !== role) {
    if (user.role === "Barber") return <Navigate to="/barber/dashboard" />;
    if (user.role === "Admin") return <Navigate to="/admin" />;
    return <Navigate to="/dashboard" />;
  }

  return children;
}
