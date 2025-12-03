import { Link } from "react-router-dom";

export default function QuizCardList({ quizzes, showOwnerActions = false, onDelete }) {
  if (!quizzes || quizzes.length === 0) {
    return <p>No quizzes found.</p>;
  }

  return (
    <div className="ingredient-scroller">
      {quizzes.map((quiz) => {
        const theme = quiz.theme && quiz.theme.trim() !== "" ? quiz.theme : "tomato";
        const description = quiz.description ?? "No description";
        const display = description.length > 80
          ? description.substring(0, 80) + "…"
          : description;

        return (
          <div key={quiz.quizId} className={`ingredient-card card theme-${theme}`}>
            <div className="ingredient-top"></div>

            <div className="card-body">
              <h5 className="ingredient-title">{quiz.title}</h5>
              <div className="ingredient-meta">{display}</div>

              <div className="ingredient-actions d-flex justify-content-between mt-3">
                <Link to={`/quiz/${quiz.quizId}`} className="btn btn-primary btn-sm">
                  Play
                </Link>

                {showOwnerActions && (
                  <div className="d-flex gap-2">
                    <Link
                      to={`/edit/${quiz.quizId}`}
                      className="btn btn-outline-secondary btn-sm"
                    >
                      Edit
                    </Link>
                    {onDelete && (
                      <button
                        type="button"
                        className="btn btn-outline-danger btn-sm"
                        onClick={() => onDelete(quiz.quizId)}
                      >
                        Delete
                      </button>
                    )}
                  </div>
                )}
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}
