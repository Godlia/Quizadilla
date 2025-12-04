const API = "http://localhost:5135/api/quiz";

// GET all quizzes (discover page)
export async function fetchDiscover() {
  const res = await fetch(API, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to load quizzes");
  return await res.json();
}

// SEARCH quizzes
export async function searchQuizzes(needle) {
  const res = await fetch(`${API}/search?needle=${encodeURIComponent(needle)}`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to search quizzes");
  return await res.json();
}

// GET single quiz
export async function fetchQuiz(id) {
  const res = await fetch(`${API}/${id}`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to load quiz");
  return await res.json();
}

// GET my quizzes
export async function fetchMyQuizzes() {
  const res = await fetch(`${API}/my`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to load quizzes");
  return await res.json();
}

// DELETE quiz
export async function deleteQuiz(id) {
  const res = await fetch(`${API}/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to delete");
}

// UPDATE quiz
export async function updateQuiz(id, quiz) {
  const res = await fetch(`${API}/${id}`, {
    method: "PUT",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(quiz)
  });
  if (!res.ok) throw new Error("Failed to update quiz");
  return await res.json();
}

// CREATE quiz
export async function createQuiz(dto) {
  const res = await fetch(API, {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto)
  });
  if (!res.ok) throw new Error("Failed to create quiz");
  return await res.json();
}
