import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { fetchQuiz, updateQuiz } from "../api/quizzes";

const THEMES = ["tomato", "guac", "cheese", "onion", "chicken", "salsa"];

export default function QuizEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [quiz, setQuiz] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  // LOAD
  useEffect(() => {
    async function load() {
      try {
        const data = await fetchQuiz(id);

        data.questions ||= [];
        data.questions.forEach((q) => {
          q.options ||= [];
          q.correctString ||= "";
        });

        setQuiz(data);
      } catch (e) {
        setError("Failed to load quiz");
      }
      setLoading(false);
    }
    load();
  }, [id]);

  // UPDATE FIELDS
  function updateField(name, value) {
    setQuiz({ ...quiz, [name]: value });
  }

  function updateQuestion(idx, value) {
    const copy = [...quiz.questions];
    copy[idx].questionText = value;
    setQuiz({ ...quiz, questions: copy });
  }

  function updateCorrect(idx, value) {
    const copy = [...quiz.questions];
    copy[idx].correctString = value;
    setQuiz({ ...quiz, questions: copy });
  }

  function updateOption(qi, oi, value) {
    const copy = [...quiz.questions];
    copy[qi].options[oi].optionText = value;
    setQuiz({ ...quiz, questions: copy });
  }

  // ADD / REMOVE
  function addQuestion() {
    setQuiz({
      ...quiz,
      questions: [
        ...quiz.questions,
        { id: 0, questionText: "", correctString: "", options: [] },
      ],
    });
  }

  function removeQuestion(idx) {
    const copy = [...quiz.questions];
    copy.splice(idx, 1);
    setQuiz({ ...quiz, questions: copy });
  }

  function addOption(qi) {
    const copy = [...quiz.questions];
    copy[qi].options.push({ optionId: 0, optionText: "" });
    setQuiz({ ...quiz, questions: copy });
  }

  function removeOption(qi, oi) {
    const copy = [...quiz.questions];
    copy[qi].options.splice(oi, 1);
    setQuiz({ ...quiz, questions: copy });
  }

  // SAVE
  async function save() {
    setSaving(true);
    setError("");

    try {
      const payload = {
        title: quiz.title,
        description: quiz.description,
        theme: quiz.theme, // <---- ADDED
        questions: quiz.questions.map((q) => ({
          id: q.id || 0,
          questionText: q.questionText,
          correctString: q.correctString || "",
          options: q.options.map((o) => ({
            optionId: o.optionId || 0,
            optionText: o.optionText,
          })),
        })),
      };

      await updateQuiz(id, payload);

      alert("Quiz updated!");
      navigate(`/quiz/${id}`);
    } catch (e) {
      console.error(e);
      setError("Failed to save quiz");
    }

    setSaving(false);
  }

  if (loading) return <p>Loading…</p>;
  if (!quiz) return <p>Quiz not found</p>;

  return (
    <div className="container mt-4" style={{ maxWidth: 900 }}>
      <h2>Edit Quiz</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      <label className="form-label mt-3">Title</label>
      <input
        className="form-control"
        value={quiz.title}
        onChange={(e) => updateField("title", e.target.value)}
      />

      <label className="form-label mt-3">Description</label>
      <textarea
        className="form-control"
        rows={3}
        value={quiz.description || ""}
        onChange={(e) => updateField("description", e.target.value)}
      />

      {/* THEME SELECT */}
      <label className="form-label mt-3">Theme</label>
      <select
        className="form-select"
        value={quiz.theme || "tomato"}
        onChange={(e) => updateField("theme", e.target.value)}
      >
        {THEMES.map((t) => (
          <option key={t} value={t}>
            {t}
          </option>
        ))}
      </select>

      <hr />

      <h4>Questions</h4>

      {quiz.questions.map((q, qi) => (
        <div key={qi} className="border rounded p-3 my-3">
          <label>Question {qi + 1}</label>
          <input
            className="form-control mb-2"
            value={q.questionText}
            onChange={(e) => updateQuestion(qi, e.target.value)}
          />

          <label>Correct answer</label>
          <input
            className="form-control mb-2"
            value={q.correctString}
            onChange={(e) => updateCorrect(qi, e.target.value)}
          />

          <label>Options</label>
          {q.options.map((o, oi) => (
            <div key={oi} className="d-flex mb-2">
              <input
                className="form-control me-2"
                value={o.optionText}
                onChange={(e) => updateOption(qi, oi, e.target.value)}
              />
              <button
                className="btn btn-danger"
                onClick={() => removeOption(qi, oi)}
              >
                ×
              </button>
            </div>
          ))}

          <button
            className="btn btn-sm btn-secondary"
            onClick={() => addOption(qi)}
          >
            + Add Option
          </button>

          <hr />

          <button
            className="btn btn-sm btn-outline-danger"
            onClick={() => removeQuestion(qi)}
          >
            Remove Question
          </button>
        </div>
      ))}

      <button className="btn btn-outline-primary mb-4" onClick={addQuestion}>
        + Add Question
      </button>

      <button
        className="btn btn-success btn-lg"
        disabled={saving}
        onClick={save}
      >
        {saving ? "Saving…" : "Save Quiz"}
      </button>
    </div>
  );
}
