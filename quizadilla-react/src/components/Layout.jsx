import Navbar from "./Navbar";

export default function Layout({ children }) {
  return (
    <div className="d-flex flex-column min-vh-100">
      <Navbar />
      <main role="main" className="flex-grow-1 container mt-4">
        {children}
      </main>
      <footer className="border-top footer text-muted">
        <div className="container">
          &copy; {new Date().getFullYear()} - Quizadilla (React)
        </div>
      </footer>
    </div>
  );
}
