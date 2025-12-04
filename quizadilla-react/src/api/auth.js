const API_URL = "http://localhost:5135/api/auth"; // endre port hvis nødvendig

async function handle(res) {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  // some responses (logout) return no body
  try {
    return await res.json();
  } catch {
    return {};
  }
}

export async function login(email, password) {
  const res = await fetch(`${API_URL}/login`, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  return handle(res);
}

export async function register(email, password) {
  const res = await fetch(`${API_URL}/register`, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  return handle(res);
}

export async function logout() {
  const res = await fetch(`${API_URL}/logout`, {
    method: "POST",
    credentials: "include",
  });
  return handle(res);
}

export async function me() {
  const res = await fetch(`${API_URL}/me`, {
    credentials: "include",
  });
  return handle(res);
}
export async function changeEmail(newEmail) {
  const res = await fetch(`${API_URL}/change-email`, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ newEmail })
  });
  return handle(res);
}

export async function changeUsername(newUsername) {
  const res = await fetch(`${API_URL}/change-username`, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ newUsername })
  });
  return handle(res);
}

export async function changePassword(currentPassword, newPassword) {
  const res = await fetch(`${API_URL}/change-password`, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ currentPassword, newPassword })
  });
  return handle(res);
}
export async function deleteAccount() {
  const res = await fetch(`${API_URL}/delete-account`, {
    method: "DELETE",
    credentials: "include"
  });
  return handle(res);
}


