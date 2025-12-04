// src/api/auth.js

const BASE = "http://localhost:5135/api/auth";

async function api(path, method = "GET", body) {
  const res = await fetch(BASE + path, {
    method,
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
    // Send error object with .response so register.jsx can read it
    const error = new Error("Request failed");
    error.response = res;
    throw error;
  }

  return res.json();
}

// ---------------------------
// AUTH FUNCTIONS
// ---------------------------

export function register(email, password) {
  return api("/register", "POST", { email, password });
}

export function login(email, password) {
  return api("/login", "POST", { email, password });
}

export function logout() {
  return api("/logout", "POST");
}

export function me() {
  return api("/me", "GET");
}

export function changeEmail(newEmail) {
  return api("/change-email", "POST", { newEmail });
}

export function changePassword(currentPassword, newPassword) {
  return api("/change-password", "POST", { currentPassword, newPassword });
}

export function changeUsername(newUsername) {
  return api("/change-username", "POST", { newUsername });
}

export function deleteAccount() {
  return api("/delete-account", "DELETE");
}
