import { useState } from "react";
import { changeEmail, changePassword, changeUsername, deleteAccount } from "../api/auth";
import { useAuth } from "../context/AuthContext";

export default function Account() {
  const { user, loginUser } = useAuth();

  // STATES
  const [newEmail, setNewEmail] = useState("");
  const [newUsername, setNewUsername] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const [msg, setMsg] = useState("");
  const [err, setErr] = useState("");

  async function handleEmail() {
    setMsg(""); setErr("");
    try {
      const result = await changeEmail(newEmail);
      loginUser(result.email);
      setMsg("Email updated!");
    } catch (e) {
      setErr("Failed: " + e.message);
    }
  }

  async function handleUsername() {
    setMsg(""); setErr("");
    try {
      await changeUsername(newUsername);
      setMsg("Username updated!");
    } catch (e) {
      setErr("Failed: " + e.message);
    }
  }

  async function handlePassword() {
    setMsg(""); setErr("");
    try {
      await changePassword(currentPassword, newPassword);
      setMsg("Password updated!");
    } catch (e) {
      setErr("Failed: " + e.message);
    }
  }

  return (
    <div className="container mt-5" style={{ maxWidth: 600 }}>
      <h2>My Account</h2>
      <p className="text-muted">Logged in as: {user?.email}</p>

      {err && <div className="alert alert-danger">{err}</div>}
      {msg && <div className="alert alert-success">{msg}</div>}

      <hr />

      {/* Change Email */}
      <h4>Change Email</h4>
      <input
        className="form-control mb-2"
        type="email"
        placeholder="New email"
        value={newEmail}
        onChange={(e) => setNewEmail(e.target.value)}
      />
      <button className="btn btn-primary mb-4" onClick={handleEmail}>
        Update Email
      </button>

      <hr />

      {/* Change Username */}
      <h4>Change Username - (Working but not really working)</h4>
      <input
        className="form-control mb-2"
        type="text"
        placeholder="New username"
        value={newUsername}
        onChange={(e) => setNewUsername(e.target.value)}
      />
      <button className="btn btn-primary mb-4" onClick={handleUsername}>
        Update Username
      </button>

      <hr />

      {/* Change Password */}
      <h4>Change Password</h4>
      <input
        className="form-control mb-2"
        type="password"
        placeholder="Current password"
        value={currentPassword}
        onChange={(e) => setCurrentPassword(e.target.value)}
      />
      <input
        className="form-control mb-2"
        type="password"
        placeholder="New password"
        value={newPassword}
        onChange={(e) => setNewPassword(e.target.value)}
      />
      <button className="btn btn-primary" onClick={handlePassword}>
        Change Password
      </button>
      <hr />

<h4 className="text-danger">Danger Zone</h4>
<p>This will permanently delete your account and all associated data.</p>

<button
  className="btn btn-danger"
  onClick={async () => {
    if (!confirm("Are you sure you want to delete your account? This cannot be undone.")) return;

    setMsg(""); setErr("");

    try {
      await deleteAccount();
      window.location.href = "/";
    } catch (e) {
      setErr("Failed: " + e.message);
    }
  }}
>
  Delete My Account
</button>

    </div>
  );
}
