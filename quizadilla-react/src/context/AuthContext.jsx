import { createContext, useContext, useEffect, useState } from "react";
import { me, logout as logoutApi } from "../api/auth";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // runs on refresh
  useEffect(() => {
    async function loadUser() {
      try {
        const data = await me();
        setUser({ email: data.email });
      } catch {
        setUser(null);
      } finally {
        setLoading(false);
      }
    }
    loadUser();
  }, []);

  async function loginUser(email) {
    setUser({ email });   // <---- THE FIX
  }

  async function logout() {
    await logoutApi();
    setUser(null);
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        loading,
        loginUser,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
