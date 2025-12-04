import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const navigate = useNavigate();
  const { user, isAuthenticated, logout } = useAuth();
  const [needle, setNeedle] = useState("");

 
  function onSearchSubmit(e) {
    e.preventDefault();
    if (needle.trim() === "") return;
    navigate(`/search?needle=${encodeURIComponent(needle)}`);
  }

useEffect(() => {
  const sidebar = document.getElementById("sidebar");
  const menuToggle = document.getElementById("menuToggle");
  const closeSidebar = document.getElementById("closeSidebar");

  const quizDropdownBtn = document.getElementById("quizDropdownBtn");
  const quizSubmenu = document.getElementById("quizSubmenu");

  const accountDropdownBtn = document.getElementById("accountDropdownBtn");
  const accountSubmenu = document.getElementById("accountSubmenu");

  function toggleSidebar() {
    sidebar.classList.toggle("active");
  }
  
  function close() {
    sidebar.classList.remove("active");
  }

  function toggleQuiz() {
    quizSubmenu.classList.toggle("d-none");
  }

  function toggleAccount() {
    accountSubmenu.classList.toggle("d-none");
  }

  menuToggle?.addEventListener("click", toggleSidebar);
  closeSidebar?.addEventListener("click", close);

  quizDropdownBtn?.addEventListener("click", toggleQuiz);
  accountDropdownBtn?.addEventListener("click", toggleAccount);

  return () => {
    menuToggle?.removeEventListener("click", toggleSidebar);
    closeSidebar?.removeEventListener("click", close);

    quizDropdownBtn?.removeEventListener("click", toggleQuiz);
    accountDropdownBtn?.removeEventListener("click", toggleAccount);
  };
}, []);


  return (
    <header>
      {}
      <nav className="navbar navbar-light custom-navbar-footer border-bottom box-shadow mb-3">
        <div className="container-fluid d-flex align-items-center justify-content-between">

          {}
          <Link className="navbar-brand d-flex align-items-center" to="/">
            <img src="/img/ChatGPT-Logo.svg" alt="logo" width="35" className="me-2" />
            <span className="fw-bold">Quizadilla</span>
          </Link>

          {}
          <div className="flex-grow-1 d-flex justify-content-center">
            <ul className="navbar-nav flex-row nav-mid" style={{ fontSize: "1.2rem", fontWeight: 400 }}>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/">Home</Link>
              </li>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/discover">Discover</Link>
              </li>
              <li className="nav-item mx-2">
                <Link className="nav-link text-dark" to="/create">Create</Link>
              </li>
            </ul>
          </div>

          {}
          <div className="right-content d-flex align-items-center">
            
            {}
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
                  <i className="bi bi-search"></i>
                </button>
              </div>
            </form>

            {} 
            <div className="dropdown">
              <button
                className="text-dark fs-4 person-icon dropdown-toggle btn btn-link"
                id="accountDropdown"
                data-bs-toggle="dropdown"
                >
                <i className="bi bi-person" />
              </button>

            <ul className="dropdown-menu dropdown-menu-start show-left mt-2 custom-navbar-footer">
              {!isAuthenticated ? (
              <>
                <li><Link className="dropdown-item" to="/login">Login</Link></li>
                <li><Link className="dropdown-item" to="/register">Register</Link></li>
              </>
              ) : (
              <>
              <li className="dropdown-item disabled">Hello {user?.email}</li>
              <li><Link className="dropdown-item" to="/account">Settings</Link></li>
              <li><button className="dropdown-item" onClick={logout}>Logout</button></li>
              </>
              )}
          </ul>
      </div>



            {} 
            <button id="menuToggle" className="navbar-toggler" type="button">
              <span className="navbar-toggler-icon"></span>
            </button>

          </div>
        </div>
      </nav>

      {}
      <div id="sidebar" className="sidebar custom-navbar-footer">

        <div className="sidebar-header">
          <h5 className="mb-0 text-dark">
            Menudilla <img src="/img/ChatGPT-Logo.svg" alt="logo" width="35" className="ms-1" />
          </h5>
          <button className="btn-close" id="closeSidebar"></button>
        </div>

        {}
        <form className="d-flex search-form2 me-2" onSubmit={onSearchSubmit}>
          <div className="input-group">
            <input
              type="search"
              className="form-control"
              placeholder="Search"
              value={needle}
              onChange={(e) => setNeedle(e.target.value)}
            />
            <button className="btn btn-outline-secondary clear-btn" type="submit">
              <i className="bi bi-search"></i>
            </button>
          </div>
        </form>

        <hr className="search-form2" />

        {} 
        <Link to="/">Home</Link>
        <Link to="/privacy">Privacy</Link>
        <Link to="/support">Support</Link>

        {}
        <div className="sidebar-dropdown sidebar-acc">
          <button id="accountDropdownBtn" className="sidebar-link d-flex w-100 align-items-center justify-content-between">
            <span>{isAuthenticated ? "My Account" : "Login/Register"}</span>
            <i className="bi bi-chevron-down ms-2" />
          </button>

          <div id="accountSubmenu" className="sidebar-submenu d-none ps-3">
            {!isAuthenticated ? (
              <>
                <Link className="sidebar-item d-block" to="/login">Login</Link>
                <Link className="sidebar-item d-block" to="/register">Register</Link>
              </>
            ) : (
              <>
                <Link className="sidebar-item d-block" to="/account">My Account/Settings</Link>
                <button className="sidebar-item btn btn-link d-block text-start" onClick={logout}>Logout</button>
              </>
            )}
          </div>
        </div>

        <hr />

        {}
        <div className="sidebar-dropdown">
          <button id="quizDropdownBtn" className="sidebar-link d-flex w-100 align-items-center justify-content-between">
            <span>Quizzes</span>
            <i className="bi bi-chevron-down ms-2"></i>
          </button>

          <div id="quizSubmenu" className="sidebar-submenu d-none ps-3">
            <Link className="sidebar-item d-block" to="/discover">Discover Quizzes</Link>
            <Link className="sidebar-item d-block" to="/create">Create Quiz</Link>
            <Link className="sidebar-item d-block" to="/categories">Categories</Link>
            <Link className="sidebar-item d-block" to="/popular">Most Popular</Link>
            {isAuthenticated && (
              <Link className="sidebar-item d-block" to="/my">My Quizzes</Link>
            )}
          </div>
        </div>

      </div>
    </header>
  );
}
