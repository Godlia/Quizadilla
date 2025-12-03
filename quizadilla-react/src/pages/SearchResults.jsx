import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { searchQuizzes } from "../api/quizzes";
import QuizCardList from "../components/QuizCardList";

function useQuery() {
  const { search } = useLocation();
  return new URLSearchParams(search);
}

export default function SearchResults() {
  const query = useQuery();
  const needle = query.get("needle") || "";
  const [quizzes, setQuizzes] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!needle) return;

    setLoading(true);
    searchQuizzes(needle)
      .then(setQuizzes)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [needle]);

  if (!needle) return <p>Use the search box in the navbar to find quizzes.</p>;
  if (loading) return <p>Searching for “{needle}”…</p>;

  return (
    <>
      <h2 className="mb-3">Results for “{needle}”</h2>
      <QuizCardList quizzes={quizzes} />
    </>
  );
}
