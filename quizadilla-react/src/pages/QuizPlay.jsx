import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { fetchQuiz } from "../api/quizzes";

export default function QuizPlay() {
  const { id } = useParams();

  const [quiz, setQuiz] = useState(null);
  const [currentIndex, setCurrentIndex] = useState(0);
  // answers: map questionId -> array of selected optionIds
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

  // toggle selection for a question's option (supports multiple correct answers)
  function toggleAnswer(questionId, optionId) {
    setAnswers((prev) => {
      const prevArr = Array.isArray(prev[questionId]) ? [...prev[questionId]] : [];
      const exists = prevArr.indexOf(optionId);
      if (exists === -1) prevArr.push(optionId);
      else prevArr.splice(exists, 1);
      return {
        ...prev,
        [questionId]: prevArr,
      };
    });
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

  function arraysEqualAsSets(a = [], b = []) {
    if (a.length !== b.length) return false;
    const sa = new Set(a);
    return b.every((x) => sa.has(x));
  }

  function finish() {
    let s = 0;
    const detailed = questions.map((q) => {
      const qid = getQuestionId(q);
      const userSelected = answers[qid] ?? [];
      // normalize option ids (they may be numbers or strings)
      const selectedIds = userSelected.map((x) => String(x));

      const opts = q.options || [];
      const correctOpts = opts.filter((o, idx) => o.isCorrect).map((o, idx) => String(o.optionId ?? idx));
      const isCorrect = arraysEqualAsSets(selectedIds, correctOpts);

      if (isCorrect) s++;

      const userAnswerText = opts
        .filter((o, idx) => selectedIds.includes(String(o.optionId ?? idx)))
        .map((o) => o.optionText);

      const correctAnswerText = opts.filter((o, idx) => correctOpts.includes(String(o.optionId ?? idx))).map((o) => o.optionText);

      return {
        id: qid,
        questionText: q.questionText,
        userAnswer: userAnswerText,
        correctAnswer: correctAnswerText,
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
                {r.userAnswer && r.userAnswer.length ? r.userAnswer.join(", ") : <em>No answer selected</em>}
              </div>
              {!r.isCorrect && (
                <div className="mb-1">
                  <strong>Correct answer:</strong> {r.correctAnswer.length ? r.correctAnswer.join(", ") : "—"}
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
            const optId = o.optionId ?? index;
            const inputId = `q${qid}-o${optId}`;
            const checked = Array.isArray(answers[qid]) && answers[qid].includes(optId);

            return (
              <div className="form-check" key={inputId}>
                <input
                  className="form-check-input"
                  type="checkbox"
                  name={`q-${qid}`}
                  id={inputId}
                  checked={checked}
                  onChange={() => toggleAnswer(qid, optId)}
                />
                <label className="form-check-label" htmlFor={inputId}>
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
