import { useState } from "react";
import { register } from "../api/auth";
import { useNavigate } from "react-router-dom";

export default function Register() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  // ---------------------------
  // VALIDATION HELPERS
  // ---------------------------

  function isValidEmail(email) {
    return /\S+@\S+\.\S+/.test(email);
  }

  function isValidPassword(password) {
    // Minst 8 tegn, 1 stor bokstav, 1 tall, 1 spesialtegn
    return /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/.test(password);
  }

  // ---------------------------
  // HANDLE SUBMIT
  // ---------------------------
  async function onSubmit(e) {
    e.preventDefault();
    setError("");

    // --- CLIENT VALIDATION ---
    if (!email.trim())
      return setError("Email cannot be empty.");

    if (!isValidEmail(email))
      return setError("Invalid email format.");

    if (!password.trim())
      return setError("Password cannot be empty.");

    if (!isValidPassword(password))
      return setError(
        "Password must be at least 8 characters,\ninclude one uppercase letter,\none number,\nand one special character."
      );

    // --- SERVER VALIDATION ---
    try {
      await register(email, password);
      navigate("/login");
    } catch (err) {
      let message = "Registration failed.";

      if (err instanceof Error && err.message) {
        message = err.message;
      }

      setError(message);
    }
  }

  return (
    <div className="container mt-5" style={{ maxWidth: 400 }}>
      <h2>Register</h2>

      {/* ERROR BOX */}
      {error && (
        <div
          className="alert alert-danger"
          style={{ whiteSpace: "pre-line" }}
        >
          {error}
        </div>
      )}

      <form onSubmit={onSubmit}>
        {/* EMAIL */}
        <input
          className="form-control mb-3"
          type="email"
          placeholder="yourmail@emailprovider.com"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        {/* PASSWORD */}
        <input
          className="form-control mb-3"
          type="password"
          placeholder="Password (8+ char, 1 uppercase, 1 number, 1 special)"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        {/* SUBMIT */}
        <button className="btn btn-success w-100">
          Register
        </button>

        <div className="text-center mt-3">
          <span>Already have an account? </span>
          <a href="/login">Login here!</a>
        </div>
      </form>
    </div>
  );
}
