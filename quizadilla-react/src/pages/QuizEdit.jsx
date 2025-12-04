import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { fetchQuiz, updateQuiz } from "../api/quizzes";

export default function QuizEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [quiz, setQuiz] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  // LOAD QUIZ
  useEffect(() => {
    async function load() {
      try {
        const data = await fetchQuiz(id);

        // Ensure arrays
        data.questions = data.questions || [];
        data.questions.forEach(q => {
          q.options = q.options || [];
        });

        setQuiz(data);
      } catch (err) {
        setError("Failed to load quiz");
      }
      setLoading(false);
    }
    load();
  }, [id]);

  // ----------------------------
  // UPDATE FIELDS
  // ----------------------------
  function updateTitle(t) {
    setQuiz({ ...quiz, title: t });
  }

  function updateDescription(d) {
    setQuiz({ ...quiz, description: d });
  }

  function updateQuestion(idx, t) {
    const q = [...quiz.questions];
    q[idx].questionText = t;
    setQuiz({ ...quiz, questions: q });
  }

  function updateOption(qi, oi, t) {
    const q = [...quiz.questions];
    q[qi].options[oi].optionText = t;
    setQuiz({ ...quiz, questions: q });
  }

  // ----------------------------
  // ADD / REMOVE
  // ----------------------------
  function addQuestion() {
    setQuiz({
      ...quiz,
      questions: [
        ...quiz.questions,
        { id: 0, questionText: "", correctString: "", options: [] }
      ]
    });
  }

  function removeQuestion(idx) {
    const q = [...quiz.questions];
    q.splice(idx, 1);
    setQuiz({ ...quiz, questions: q });
  }

  function addOption(qi) {
    const q = [...quiz.questions];
    q[qi].options.push({ optionId: 0, optionText: "" });
    setQuiz({ ...quiz, questions: q });
  }

  function removeOption(qi, oi) {
    const q = [...quiz.questions];
    q[qi].options.splice(oi, 1);
    setQuiz({ ...quiz, questions: q });
  }

  // ----------------------------
  // SAVE
  // ----------------------------
  async function save() {
    setSaving(true);
    setError("");

    try {
      const payload = {
        title: quiz.title,
        description: quiz.description,
        questions: quiz.questions.map(q => ({
          id: q.id || 0,
          questionText: q.questionText,
          correctString: q.correctString || "",
          options: q.options.map(o => ({
            optionId: o.optionId || 0,
            optionText: o.optionText
          }))
        }))
      };

      await updateQuiz(id, payload);

      alert("Quiz updated!");
      navigate(`/quiz/${id}`);
    } catch (err) {
      console.error(err);
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
        onChange={(e) => updateTitle(e.target.value)}
      />

      <label className="form-label mt-3">Description</label>
      <textarea
        className="form-control"
        rows={3}
        value={quiz.description || ""}
        onChange={(e) => updateDescription(e.target.value)}
      />

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

          <label>Options</label>
          {q.options.map((o, oi) => (
            <div key={oi} className="d-flex mb-2">
              <input
                className="form-control me-2"
                value={o.optionText}
                onChange={(e) => updateOption(qi, oi, e.target.value)}
              />
              <button className="btn btn-danger" onClick={() => removeOption(qi, oi)}>×</button>
            </div>
          ))}

          <button className="btn btn-sm btn-secondary" onClick={() => addOption(qi)}>
            + Add Option
          </button>

          <hr />

          <button className="btn btn-sm btn-outline-danger" onClick={() => removeQuestion(qi)}>
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
