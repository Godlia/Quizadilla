import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";

export default function Navbar() {
  const [needle, setNeedle] = useState("");
  const navigate = useNavigate();

  function onSearchSubmit(e) {
    e.preventDefault();
    if (!needle.trim()) return;
    navigate(`/search?needle=${encodeURIComponent(needle.trim())}`);
  }

  return (
    <nav className="navbar navbar-expand-lg navbar-light custom-navbar-footer">
      <div className="container-fluid">
        <Link className="navbar-brand fw-bold" to="/">
          Quizadilla
        </Link>

        <div className="collapse navbar-collapse show">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0 nav-mid">
            <li className="nav-item">
              <Link className="nav-link" to="/discover">
                Discover
              </Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/my">
                My Quizzes
              </Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/create">
                Create
              </Link>
            </li>
          </ul>

          <form className="d-flex search-form" onSubmit={onSearchSubmit}>
            <div className="input-group">
              <input
                type="search"
                className="form-control"
                placeholder="Search"
                value={needle}
                onChange={(e) => setNeedle(e.target.value)}
              />
              <button className="btn btn-outline-secondary clear-btn" type="submit">
                <i className="bi bi-search" />
              </button>
            </div>
          </form>
        </div>
      </div>
    </nav>
  );
}
