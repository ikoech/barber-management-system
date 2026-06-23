import { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Extract user fields from JWT
  const mapUserFromToken = (decoded) => ({
    id: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
    email: decoded.email,
    role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
    fullName: decoded.fullName ?? null,
    barberId: decoded.barberId ? Number(decoded.barberId) : null
  });

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        setUser(mapUserFromToken(decoded));
      } catch {
        localStorage.removeItem("token");
      }
    }
    setLoading(false);
  }, []);

  const login = (token) => {
    localStorage.setItem("token", token);
    const decoded = jwtDecode(token);
    setUser(mapUserFromToken(decoded));
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  const authFetch = async (url, options = {}) => {
    const token = localStorage.getItem("token");

    const headers = {
      ...(options.headers || {}),
      Authorization: token ? `Bearer ${token}` : "",
      "Content-Type": "application/json",
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
