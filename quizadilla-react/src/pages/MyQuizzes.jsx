import { useEffect, useState } from "react";
import { fetchMyQuizzes, deleteQuiz } from "../api/quizzes";
import QuizCardList from "../components/QuizCardList";

export default function MyQuizzes() {
  const [quizzes, setQuizzes] = useState([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      const data = await fetchMyQuizzes();
      setQuizzes(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function handleDelete(id) {
    if (!window.confirm("Delete this quiz?")) return;
    await deleteQuiz(id);
    await load();
  }

  if (loading) return <p>Loading your quizzes…</p>;

  return (
    <>
      <h2 className="mb-3">My Quizzes</h2>
      <QuizCardList quizzes={quizzes} showOwnerActions onDelete={handleDelete} />
    </>
  );
}
