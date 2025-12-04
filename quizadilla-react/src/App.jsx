import { Routes, Route } from "react-router-dom";
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

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/discover" element={<Discover />} />
        <Route path="/my" element={<MyQuizzes />} />
        <Route path="/search" element={<SearchResults />} />
        <Route path="/quiz/:id" element={<QuizPlay />} />
        <Route path="/create" element={<QuizCreate />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/account" element={<Account />} />
        <Route path="/quiz/:id/edit" element={<QuizEdit />} />

      </Routes>
    </Layout>
  );
}
