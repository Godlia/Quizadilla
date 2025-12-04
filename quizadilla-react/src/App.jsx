import React from "react";                      // <- MANGLET HOS DEG
import { Routes, Route, Navigate } from "react-router-dom";

import Layout from "./components/Layout";
import Home from "./pages/Home";
import Discover from "./pages/Discover";
import MyQuizzes from "./pages/MyQuizzes";
import SearchResults from "./pages/SearchResults";
import QuizPlay from "./pages/QuizPlay";
import QuizCreate from "./pages/QuizCreate";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Account from "./pages/Account";
import QuizEdit from "./pages/QuizEdit";

import { useAuth } from "./context/AuthContext";

export default function App() {
  const { isAuthenticated, loading } = useAuth();

  
  if (loading) {
    return (
      <div style={{ padding: "2rem", textAlign: "center" }}>
        Loading...
      </div>
    );
  }

  return (
    <Layout>
      <Routes>
        {}
        <Route path="/" element={<Home />} />
        <Route path="/discover" element={<Discover />} />
        <Route path="/search" element={<SearchResults />} />
        <Route path="/quiz/:id" element={<QuizPlay />} />

        {}
        <Route
          path="/my"
          element={
            isAuthenticated ? <MyQuizzes /> : <Navigate to="/login" replace />
          }
        />
        <Route
          path="/create"
          element={
            isAuthenticated ? <QuizCreate /> : <Navigate to="/login" replace />
          }
        />
        <Route
          path="/quiz/:id/edit"
          element={
            isAuthenticated ? <QuizEdit /> : <Navigate to="/login" replace />
          }
        />

        {}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/account" element={<Account />} />
      </Routes>
    </Layout>
  );
}
