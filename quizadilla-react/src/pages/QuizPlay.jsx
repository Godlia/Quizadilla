import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { fetchQuiz } from "../api/quizzes";

export default function QuizPlay() {
  const { id } = useParams();

  const [quiz, setQuiz] = useState(null);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [answers, setAnswers] = useState({}); 
  const [finished, setFinished] = useState(false);
  const [score, setScore] = useState(0);
  const [results, setResults] = useState([]); 

  useEffect(() => {
    fetchQuiz(id)
      .then(setQuiz)
      .catch(console.error);
  }, [id]);

  if (!quiz) return <p>Loading quiz…</p>;

  const questions = quiz.questions || [];
  const currentQuestion = questions[currentIndex];
  if (!currentQuestion) return <p>No questions in this quiz.</p>;

 
  function getQuestionId(q) {
    return q.id ?? q.questionId ?? q.Id;
  }

  function chooseAnswer(questionId, optionText) {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: optionText,
    }));
  }

  function next() {
    if (currentIndex < questions.length - 1) {
      setCurrentIndex((i) => i + 1);
    }
  }

  function prev() {
    if (currentIndex > 0) {
      setCurrentIndex((i) => i - 1);
    }
  }

  function finish() {
    let s = 0;
    const detailed = questions.map((q) => {
      const qid = getQuestionId(q);
      const userAnswer = answers[qid] ?? null;
      const correctAnswer = q.correctString ?? "";
      const isCorrect = userAnswer != null && userAnswer === correctAnswer;

      if (isCorrect) s++;

      return {
        id: qid,
        questionText: q.questionText,
        userAnswer,
        correctAnswer,
        isCorrect,
      };
    });

    setScore(s);
    setResults(detailed);
    setFinished(true);
  }

  if (finished) {
    return (
      <div>
        <h2>{quiz.title}</h2>
        <p className="text-muted">{quiz.description}</p>

        <div id="scoreSummary" className="alert alert-info text-center mt-4">
          <div id="scoreText">
            You scored <strong>{score}</strong> out of{" "}
            <strong>{questions.length}</strong>
          </div>
        </div>

        <h3 className="mt-4 mb-3">Review your answers</h3>

        <div className="quiz-review-list">
          {results.map((r, index) => (
            <div
              key={r.id ?? index}
              className={
                "quiz-result-card mb-3 p-3 rounded " +
                (r.isCorrect ? "quiz-correct" : "quiz-wrong")
              }
            >
              <div className="fw-bold mb-1">
                Question {index + 1}{" "}
                {r.isCorrect ? (
                  <span className="text-success">(Correct)</span>
                ) : (
                  <span className="text-danger">(Wrong)</span>
                )}
              </div>
              <div className="mb-2">{r.questionText}</div>

              <div className="mb-1">
                <strong>Your answer:</strong>{" "}
                {r.userAnswer ?? <em>No answer selected</em>}
              </div>
              {!r.isCorrect && (
                <div className="mb-1">
                  <strong>Correct answer:</strong> {r.correctAnswer || "—"}
                </div>
              )}
            </div>
          ))}
        </div>

        <div className="mt-4">
          <Link to="/discover" className="btn btn-primary me-2">
            Back to Discover
          </Link>
          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={() => {
              setAnswers({});
              setFinished(false);
              setCurrentIndex(0);
              setScore(0);
              setResults([]);
            }}
          >
            Retake quiz
          </button>
        </div>
      </div>
    );
  }

  const options = currentQuestion.options || [];
  const qid = getQuestionId(currentQuestion);

  return (
    <div>
      <h2>{quiz.title}</h2>
      <p className="text-muted">{quiz.description}</p>

      <div className="card mt-4">
        <div className="card-body">
          <h5>
            Question {currentIndex + 1} of {questions.length}
          </h5>
          <p className="mt-3">{currentQuestion.questionText}</p>

          {options.map((o, index) => {
            const oid = `q${qid}-o${o.optionId ?? index}`;
            const checked = answers[qid] === o.optionText;

            return (
              <div className="form-check" key={oid}>
                <input
                  className="form-check-input"
                  type="radio"
                  name={`q-${qid}`}
                  id={oid}
                  checked={checked}
                  onChange={() => chooseAnswer(qid, o.optionText)}
                />
                <label className="form-check-label" htmlFor={oid}>
                  {o.optionText}
                </label>
              </div>
            );
          })}

          <div className="mt-3 d-flex justify-content-between">
            <button
              type="button"
              className="btn btn-outline-secondary"
              disabled={currentIndex === 0}
              onClick={prev}
            >
              Previous
            </button>
            {currentIndex < questions.length - 1 ? (
              <button type="button" className="btn btn-primary" onClick={next}>
                Next
              </button>
            ) : (
              <button
                type="button"
                className="btn btn-success"
                onClick={finish}
              >
                Finish
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
