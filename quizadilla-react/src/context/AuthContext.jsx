import { createContext, useContext, useEffect, useState } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [token, setToken] = useState(null);
  const [user, setUser] = useState(null); // optional: decode JWT later

  useEffect(() => {
    const saved = localStorage.getItem("jwt");
    if (saved) setToken(saved);
  }, []);

  function login(jwt) {
    localStorage.setItem("jwt", jwt);
    setToken(jwt);
  }

  function logout() {
    localStorage.removeItem("jwt");
    setToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ token, user, login, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
