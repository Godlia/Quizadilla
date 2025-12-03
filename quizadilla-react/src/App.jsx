import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Home from "./pages/Home";
import Discover from "./pages/Discover";
import MyQuizzes from "./pages/MyQuizzes";
import SearchResults from "./pages/SearchResults";
import QuizPlay from "./pages/QuizPlay";
import QuizCreate from "./pages/QuizCreate";

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/discover" element={<Discover />} />
          <Route path="/my" element={<MyQuizzes />} />
          <Route path="/search" element={<SearchResults />} />
          <Route path="/quiz/:id" element={<QuizPlay />} />
          <Route path="/create" element={<QuizCreate />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}
