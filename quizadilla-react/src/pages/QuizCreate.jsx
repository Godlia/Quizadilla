import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createQuiz } from "../api/quizzes";

const emptyQuestion = () => ({
  questionText: "",
  correctString: "",
  options: [{ optionText: "" }, { optionText: "" }],
});

export default function QuizCreate() {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [questions, setQuestions] = useState([emptyQuestion()]);
  const navigate = useNavigate();

  // -------------------------
  // UPDATE QUESTIONS
  // -------------------------
  function updateQuestion(index, patch) {
    setQuestions((prev) => {
      const copy = [...prev];
      copy[index] = { ...copy[index], ...patch };
      return copy;
    });
  }

  function updateOption(qIndex, oIndex, value) {
    setQuestions((prev) => {
      const copy = [...prev];
      const opts = [...copy[qIndex].options];
      opts[oIndex] = { ...opts[oIndex], optionText: value };
      copy[qIndex].options = opts;
      return copy;
    });
  }

  // -------------------------
  // ADD / REMOVE QUESTIONS
  // -------------------------
  function addQuestion() {
    setQuestions((prev) => [...prev, emptyQuestion()]);
  }

  function addOption(qIndex) {
    setQuestions((prev) => {
      const copy = [...prev];
      copy[qIndex].options = [...copy[qIndex].options, { optionText: "" }];
      return copy;
    });
  }

  function removeOption(qIndex, oIndex) {
    setQuestions((prev) => {
      const copy = [...prev];
      copy[qIndex].options = copy[qIndex].options.filter((_, i) => i !== oIndex);
      return copy;
    });
  }

  // -------------------------
  // SUBMIT
  // -------------------------
  async function handleSubmit(e) {
    e.preventDefault();

    const payload = {
      title,
      description,
      questions: questions.map((q) => ({
        questionText: q.questionText,
        correctString: q.correctString,
        options: q.options.map((o) => ({
          optionText: o.optionText,
        })),
      })),
    };

    try {
      await createQuiz(payload);
      navigate("/my");
    } catch (err) {
      console.error("CREATE QUIZ ERROR:", err);
      alert("Saving quiz failed:\n" + err.message);
    }
  }

  // -------------------------
  // UI
  // -------------------------
  return (
    <div>
      <h2>Create a New Quiz</h2>

      <form onSubmit={handleSubmit}>
        {/* Title */}
        <div className="mb-3">
          <label className="form-label">Title</label>
          <input
            className="form-control"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>

        {/* Description */}
        <div className="mb-3">
          <label className="form-label">Description</label>
          <textarea
            className="form-control"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>

        <hr />

        {/* Questions */}
        {questions.map((q, qi) => (
          <div key={qi} className="mb-4 border rounded p-3">
            <h5>Question {qi + 1}</h5>

            {/* Question text */}
            <div className="mb-2">
              <label className="form-label">Question text</label>
              <input
                className="form-control"
                value={q.questionText}
                onChange={(e) =>
                  updateQuestion(qi, { questionText: e.target.value })
                }
                required
              />
            </div>

            {/* Correct answer */}
            <div className="mb-2">
              <label className="form-label">Correct answer</label>
              <input
                className="form-control"
                value={q.correctString}
                onChange={(e) =>
                  updateQuestion(qi, { correctString: e.target.value })
                }
                required
              />
            </div>

            {/* Options */}
            <div className="mb-2">
              <label className="form-label">Options</label>

              {q.options.map((o, oi) => (
                <div key={oi} className="input-group mb-1">
                  <input
                    className="form-control"
                    value={o.optionText}
                    onChange={(e) => updateOption(qi, oi, e.target.value)}
                    required
                  />

                  {q.options.length > 2 && (
                    <button
                      type="button"
                      className="btn btn-outline-danger"
                      onClick={() => removeOption(qi, oi)}
                    >
                      ✕
                    </button>
                  )}
                </div>
              ))}

              <button
                type="button"
                className="btn btn-outline-secondary btn-sm mt-1"
                onClick={() => addOption(qi)}
              >
                + Add option
              </button>
            </div>
          </div>
        ))}

        {/* Add question */}
        <button
          type="button"
          className="btn btn-outline-primary mb-3"
          onClick={addQuestion}
        >
          + Add question
        </button>

        {/* Save */}
        <div>
          <button type="submit" className="btn btn-primary">
            Save quiz
          </button>
        </div>
      </form>
    </div>
  );
}
