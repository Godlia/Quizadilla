import { useState } from "react";
import { register } from "../api/auth";
import { useNavigate } from "react-router-dom";

export default function Register() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");  
  const [error, setError] = useState("");

  async function onSubmit(e) {
    e.preventDefault();
    setError("");

    try {
      await register(email, password);
      navigate("/");
    } catch (e) {
      setError("Registration failed");
    }
  }

  return (
    <div className="container mt-5" style={{ maxWidth: 400 }}>
      <h2>Register</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      <form onSubmit={onSubmit}>
        <input
          className="form-control mb-3"
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <input
          className="form-control mb-3"
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button className="btn btn-success w-100">Register</button>
        
        <div className="text-center mt-3">
            <span>Already have an account? </span>
            <a href="/login">Login here!</a>
        </div>
      </form>
    </div>
  );
}
