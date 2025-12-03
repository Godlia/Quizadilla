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
  const [theme, setTheme] = useState("tomato");
  const [questions, setQuestions] = useState([emptyQuestion()]);
  const navigate = useNavigate();

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

  async function handleSubmit(e) {
  e.preventDefault();

  const quiz = {
    title,
    description,
    theme,
    questions,
  };

  try {
    await createQuiz(quiz);
    navigate("/my");
  } catch (err) {
    console.error(err);
    alert("Saving quiz failed:\n" + err.message);
  }
}


  return (
    <div>
      <h2>Create a New Quiz</h2>

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Title</label>
          <input
            className="form-control"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Description</label>
          <textarea
            className="form-control"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Theme</label>
          <select
            className="form-select"
            value={theme}
            onChange={(e) => setTheme(e.target.value)}
          >
            <option value="tomato">Tomato</option>
            <option value="guac">Guac</option>
            <option value="cheese">Cheese</option>
            <option value="onion">Onion</option>
            <option value="chicken">Chicken</option>
            <option value="salsa">Salsa</option>
          </select>
        </div>

        <hr />

        {questions.map((q, qi) => (
          <div key={qi} className="mb-4 border rounded p-3">
            <h5>Question {qi + 1}</h5>

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

        <button
          type="button"
          className="btn btn-outline-primary mb-3"
          onClick={addQuestion}
        >
          + Add question
        </button>

        <div>
          <button type="submit" className="btn btn-primary">
            Save quiz
          </button>
        </div>
      </form>
    </div>
  );
}
