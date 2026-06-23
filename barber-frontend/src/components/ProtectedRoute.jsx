import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function ProtectedRoute({ children }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  const publicRoutes = ["/login", "/register"];

  if (loading) return <div>Loading...</div>;

  // If NOT logged in → allow login/register
  if (!user && publicRoutes.includes(location.pathname)) {
    return children;
  }

  // If logged in → block login/register
  if (user && publicRoutes.includes(location.pathname)) {
    if (user.role === "Admin") return <Navigate to="/admin" />;
    if (user.role === "Barber") return <Navigate to="/barber/dashboard" />;
    return <Navigate to="/dashboard" />;
  }

  // If NOT logged in → block protected routes
  if (!user) return <Navigate to="/login" />;

  // Otherwise allow
  return children;
}
