// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    
//NAVBAR SCRIPT 
const sidebar = document.getElementById("sidebar");
const menuToggle = document.getElementById("menuToggle");
const closeSidebar = document.getElementById("closeSidebar");
const body = document.body;

menuToggle?.addEventListener("click", () => {
  const isActive = sidebar.classList.toggle("active");
  body.classList.toggle("sidebar-open", isActive);

  // 🔹 Skjul hamburger når sidebaren er aktiv
  menuToggle.style.visibility = isActive ? "hidden" : "visible";
});

closeSidebar?.addEventListener("click", () => {
  sidebar.classList.remove("active");
  body.classList.remove("sidebar-open");
  menuToggle.style.visibility = "visible"; // vis igjen
});


