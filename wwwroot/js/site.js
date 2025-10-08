// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    
// Sidebar toggle logic
    const sidebar = document.getElementById("sidebar");
    const menuToggle = document.getElementById("menuToggle");
    const closeSidebar = document.getElementById("closeSidebar");

    menuToggle.addEventListener("click", () => {
      sidebar.classList.toggle("active");
    });
    closeSidebar.addEventListener("click", () => {
      sidebar.classList.remove("active");
    });