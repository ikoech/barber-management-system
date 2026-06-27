import { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext();

function normalizeToNumberOrNull(v) {
  if (v === null || v === undefined || v === "") return null;
  const n = Number(v);
  return Number.isNaN(n) ? null : n;
}

function mapUserFromToken(decoded) {
  return {
    id:
      normalizeToNumberOrNull(decoded?.id) ??
      normalizeToNumberOrNull(decoded?.sub) ??
      normalizeToNumberOrNull(
        decoded?.[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ] ?? null
      ),
    email: decoded?.email ?? null,
    role:
      decoded?.role ??
      decoded?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
      null,
    fullName: decoded?.fullName ?? null,
    barberId: normalizeToNumberOrNull(decoded?.barberId),
  };
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Load token on startup
  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      setUser(null);
      setLoading(false);
      return;
    }

    try {
      const decoded = jwtDecode(token);
      const mapped = mapUserFromToken(decoded);

      // Only barbers require a valid barberId
      if (mapped.role === "Barber" && !mapped.barberId) {
        console.error("Invalid barberId for Barber user");
        localStorage.removeItem("token");
        setUser(null);
        setLoading(false);
        return;
      }

      setUser(mapped);
    } catch (err) {
      console.error("JWT decode failed", err);
      localStorage.removeItem("token");
      setUser(null);
    }

    setLoading(false);
  }, []);

  // Login
  const login = (token) => {
    localStorage.setItem("token", token);

    try {
      const decoded = jwtDecode(token);
      const mapped = mapUserFromToken(decoded);

      if (mapped.role === "Barber" && !mapped.barberId) {
        console.error("Invalid barberId for Barber user");
        localStorage.removeItem("token");
        setUser(null);
        return;
      }

      setUser(mapped);
    } catch (err) {
      console.error("JWT decode failed on login", err);
      localStorage.removeItem("token");
      setUser(null);
    }
  };

  // Logout
  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  // Authenticated fetch
  const authFetch = async (url, options = {}) => {
    const token = localStorage.getItem("token");

    const headers = {
      ...(options.headers || {}),
      Authorization: token ? `Bearer ${token}` : "",
      "Content-Type": options.body
        ? "application/json"
        : options.headers?.["Content-Type"] ?? "application/json",
    };

    return fetch(url, { ...options, headers });
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, authFetch, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
