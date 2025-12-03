import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";

export default function Navbar() {
  const [needle, setNeedle] = useState("");
  const navigate = useNavigate();

  // Attach sidebar + dropdown behaviour (ported from site.js)
  useEffect(() => {
    const sidebar = document.getElementById("sidebar");
    const menuToggle = document.getElementById("menuToggle");
    const closeSidebar = document.getElementById("closeSidebar");
    const quizDropdownBtn = document.getElementById("quizDropdownBtn");
    const quizDropdown = document.querySelector(".sidebar-dropdown");
    const quizSubmenu = document.querySelector(".sidebar-submenu");
    const body = document.body;

    function toggleSidebar() {
      if (!sidebar || !menuToggle) return;
      const isActive = sidebar.classList.toggle("active");
      body.classList.toggle("sidebar-open", isActive);
      menuToggle.style.visibility = isActive ? "hidden" : "visible";
    }

    function close() {
      if (!sidebar || !menuToggle) return;
      sidebar.classList.remove("active");
      body.classList.remove("sidebar-open");
      menuToggle.style.visibility = "visible";
    }

    function toggleQuizDropdown() {
      if (!quizSubmenu || !quizDropdown) return;
      quizSubmenu.classList.toggle("d-none");
      quizDropdown.classList.toggle("open");
    }

    menuToggle?.addEventListener("click", toggleSidebar);
    closeSidebar?.addEventListener("click", close);
    quizDropdownBtn?.addEventListener("click", toggleQuizDropdown);

    return () => {
      menuToggle?.removeEventListener("click", toggleSidebar);
      closeSidebar?.removeEventListener("click", close);
      quizDropdownBtn?.removeEventListener("click", toggleQuizDropdown);
    };
  }, []);

  function onSearchSubmit(e) {
    e.preventDefault();
    const term = needle.trim();
    if (!term) return;
    navigate(`/search?needle=${encodeURIComponent(term)}`);
  }

  return (
    <header>
      <nav className="navbar navbar-expand-lg navbar-light custom-navbar-footer">
        <div className="container-fluid">
          {/* Brand with logo */}
          <Link className="navbar-brand d-flex align-items-center me-2" to="/">
            <img
              src="/img/ChatGPT-Logo.svg"   
              alt="Quizadilla logo"
              width="32"
              height="32"
              className="me-2"
            />
            <span className="fw-bold">Quizadilla</span>
          </Link>

          {/* Centered nav links */}
          <div className="flex-grow-1 d-flex justify-content-center">
            <ul className="navbar-nav flex-row nav-mid" style={{ fontSize: "1.1rem" }}>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/">
                  Home
                </Link>
              </li>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/discover">
                  Discover Quizzes
                </Link>
              </li>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/create">
                  Create Quiz
                </Link>
              </li>
            </ul>
          </div>

          {/* Right side: search + person + burger */}
          <div className="right-content d-flex align-items-center">
            {/* Search */}
            <form className="d-flex search-form me-2" onSubmit={onSearchSubmit}>
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

            {/* Person icon – entry point for future login */}
            <button
              type="button"
              className="text-dark fs-4 person-icon btn btn-link"
              title="Log in"
            >
              <i className="bi bi-person" />
            </button>

            {/* Hamburger */}
            <button id="menuToggle" className="navbar-toggler ms-1" type="button">
              <span className="navbar-toggler-icon"></span>
            </button>
          </div>
        </div>
      </nav>

      {/* Right sidebar (hamburger menu) */}
      <div id="sidebar" className="sidebar custom-navbar-footer">
        <div className="sidebar-header">
          <h5 className="mb-0 text-dark">
            Menudilla{" "}
            <img src="/img/logo.svg" alt="Logo" width="24" className="ms-1" />
          </h5>
          <button id="closeSidebar" className="btn btn-link text-dark">
            <i className="bi bi-x-lg"></i>
          </button>
        </div>

        <div className="sidebar-body">
          <Link className="sidebar-link" to="/">
            Home
          </Link>
          <Link className="sidebar-link" to="/discover">
            Discover quizzes
          </Link>
          <Link className="sidebar-link" to="/create">
            Create quiz
          </Link>

          <hr />

          <div className="sidebar-dropdown">
            <button
              id="quizDropdownBtn"
              className="sidebar-link d-flex w-100 align-items-center justify-content-between"
              type="button"
            >
              <span>Quizzes</span>
              <i className="bi bi-chevron-down" />
            </button>
            <div id="quizSubmenu" className="sidebar-submenu d-none">
              <Link className="sidebar-link ps-4" to="/discover">
                Discover quizzes
              </Link>
              <Link className="sidebar-link ps-4" to="/my">
                My quizzes
              </Link>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
}
