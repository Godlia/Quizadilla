import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { fetchDiscover } from "../api/quizzes";
import QuizCardList from "../components/QuizCardList";

export default function Home() {
  const [quizzes, setQuizzes] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDiscover()
      .then(setQuizzes)
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  const todaysQuiz = useMemo(() => {
    if (!quizzes || quizzes.length === 0) return null;
    const i = Math.floor(Math.random() * quizzes.length);
    return quizzes[i];
  }, [quizzes]);

  const trending = useMemo(() => {
    if (!quizzes) return [];
    // for now, just take the first 4 quizzes like your seed data
    return quizzes.slice(0, 4);
  }, [quizzes]);

  return (
    <div className="custom-background-color">
      {/* Top hero row */}
      <div className="row align-items-start mt-4">
        {/* Left: hero text */}
        <div className="col-lg-6 mb-3">
          <h1 className="text-center text-lg-start">Welcome to Quizzadilla</h1>
          <p className="text-center text-lg-start text-index">
            Your home for quizzes
          </p>
        </div>

        {/* Right: today's random quiz card */}
        <div className="col-lg-6 mb-3">
          {todaysQuiz && (
            <div className="todays-quiz p-3">
              <div className="d-flex justify-content-between align-items-center">
                <h5 className="mb-0">Today&apos;s random quiz</h5>
              </div>

              <div className="mt-2 text-index-quiz">
                <div className="ingredient-title-top">
                  {todaysQuiz.title}
                </div>
                <div className="ingredient-meta-top">
                  {todaysQuiz.description || "No description"}
                </div>
              </div>

              <div className="ingredient-actions-top mt-3 mb-1">
                <Link
                  to={`/quiz/${todaysQuiz.quizId}`}
                  className="btn btn-primary btn-sm me-2"
                >
                  Play
                </Link>
                {/* Preview-knapp kan du fylle senere hvis du vil */}
                <Link
                  to={`/quiz/${todaysQuiz.quizId}`}
                  className="btn btn-outline-secondary btn-sm"
                >
                  Preview
                </Link>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Trending section */}
      <section className="mt-5">
        <h2 className="text-center">Trending</h2>
        <p className="text-center text-index mb-4">
          The most popular quizzes at the moment!
        </p>

        {loading ? (
          <p className="text-center">Loading quizzes…</p>
        ) : (
          <QuizCardList quizzes={trending} />
        )}
      </section>

      {/* Bottom “we have quizzes for everyone” banner */}
      <section className="mt-5 mb-4 text-center">
        <h3>We have quizzes for everyone!</h3>
        <p className="text-index">
          Browse through our extensive library of quizzes or create your own.
        </p>
      </section>
    </div>
  );
}
