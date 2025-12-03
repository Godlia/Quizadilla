import { useEffect, useState } from "react";
import { fetchDiscover } from "../api/quizzes";
import QuizCardList from "../components/QuizCardList";

export default function Discover() {
  const [quizzes, setQuizzes] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDiscover()
      .then(setQuizzes)
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading quizzes…</p>;

  return (
    <>
      <h2 className="mb-3">Discover Quizzes</h2>
      <p className="text-index mb-4">
        Scroll through tasty quizzes, just like i den gamle versjonen 🍅
      </p>
      <QuizCardList quizzes={quizzes} />
    </>
  );
}
