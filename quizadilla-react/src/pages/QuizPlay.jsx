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

  useEffect(() => {
    fetchQuiz(id)
      .then(setQuiz)
      .catch(console.error);
  }, [id]);

  if (!quiz) return <p>Loading quiz…</p>;

  const questions = quiz.questions || [];
  const currentQuestion = questions[currentIndex];

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
    for (const q of questions) {
      const qid = q.id;
      const userAnswer = answers[qid];
      if (userAnswer && userAnswer === q.correctString) {
        s++;
      }
    }
    setScore(s);
    setFinished(true);
  }

  if (finished) {
    return (
      <div className="text-center">
        <h2>{quiz.title}</h2>
        <div id="scoreSummary" className="alert alert-info text-center mt-4">
          <div id="scoreText">
            You scored {score} out of {questions.length}
          </div>
        </div>
        <Link to="/discover" className="btn btn-primary mt-3">
          Back to Discover
        </Link>
      </div>
    );
  }

  const options = currentQuestion.options || [];
  const qid = currentQuestion.id;

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
            const oid = `q${qid}-o${o.optionId || index}`;
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
              <button type="button" className="btn btn-success" onClick={finish}>
                Finish
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
