import Navbar from "./Navbar";

export default function Layout({ children }) {
  return (
    <div className="d-flex flex-column min-vh-100">
      <Navbar />
      <main role="main" className="flex-grow-1 container mt-4 main-content">
        {children}
      </main>
      <footer className="border-top footer custom-navbar-footer">
        <div className="container text-center">
          &copy; {new Date().getFullYear()} - Quizadilla
        </div>
      </footer>
    </div>
  );
}
