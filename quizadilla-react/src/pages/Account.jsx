import { useState } from "react";
import { changeEmail, changePassword, deleteAccount } from "../api/auth";
import { useAuth } from "../context/AuthContext";

export default function Account() {
  const { user, loginUser } = useAuth();

  
  const [newEmail, setNewEmail] = useState("");
  const [newUsername, setNewUsername] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const [msg, setMsg] = useState("");
  const [err, setErr] = useState("");

 

  function isValidEmail(email) {
    return /\S+@\S+\.\S+/.test(email);
  }

  function isValidPassword(password) {
    return /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/.test(password);
  }

  function showError(message) {
    setErr(message);
    setMsg("");
  }

  function showSuccess(message) {
    setMsg(message);
    setErr("");
  }


  async function handleEmail() {
    setMsg(""); setErr("");

    if (!newEmail.trim()) return showError("Email cannot be empty.");
    if (!isValidEmail(newEmail)) return showError("Invalid email format.");

    try {
      const result = await changeEmail(newEmail);
      loginUser(result.email);
      showSuccess("Email updated!");
    } catch (e) {
      showError(e.message);
    }
  }

 
  async function handleUsername() {
    setMsg(""); setErr("");

    if (!newUsername.trim()) return showError("Username cannot be empty.");
    if (newUsername.length < 3)
      return showError("Username must be at least 3 characters long.");

    try {
      await changeUsername(newUsername);
      showSuccess("Username updated!");
    } catch (e) {
      showError(e.message);
    }
  }

 
  async function handlePassword() {
    setMsg(""); setErr("");

    if (!currentPassword.trim() || !newPassword.trim())
      return showError("Both password fields must be filled.");

    if (newPassword === currentPassword)
      return showError("New password cannot be the same as the current one.");

    if (!isValidPassword(newPassword))
      return showError(
        "Password must be at least 8 characters, contain one uppercase letter, one number and one special character."
      );

    try {
      await changePassword(currentPassword, newPassword);
      showSuccess("Password updated!");
    } catch (e) {
      showError(e.message);
    }
  }

 
  async function handleDelete() {
    setMsg(""); setErr("");

    if (!confirm("Are you sure? This cannot be undone.")) return;

    try {
      await deleteAccount();
      window.location.href = "/";
    } catch (e) {
      showError(e.message);
    }
  }


  return (
    <div className="container mt-5" style={{ maxWidth: 600 }}>
      <h2>My Account</h2>
      <p className="text-muted">Logged in as: {user?.email}</p>

      {err && <div className="alert alert-danger">{err}</div>}
      {msg && <div className="alert alert-success">{msg}</div>}

      <hr />

      {}
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

      {}
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

      {}
      <h4 className="text-danger">Danger Zone</h4>
      <p>This will permanently delete your account and all associated data.</p>

      <button className="btn btn-danger" onClick={handleDelete}>
        Delete My Account
      </button>
    </div>
  );
}
