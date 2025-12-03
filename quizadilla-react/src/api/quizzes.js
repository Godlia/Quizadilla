const API_BASE = "http://localhost:5135/api/quiz"; // JUSTER port hvis din backend bruker en annen port

async function handle(res) {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json();
}

export async function fetchDiscover() {
  const res = await fetch(`${API_BASE}`);
  return handle(res);
}

export async function fetchMyQuizzes() {
  const res = await fetch(`${API_BASE}/my`, {
    credentials: "include",
  });
  return handle(res);
}

export async function fetchQuiz(id) {
  const res = await fetch(`${API_BASE}/${id}`);
  return handle(res);
}

export async function searchQuizzes(needle) {
  const res = await fetch(
    `${API_BASE}/search?needle=${encodeURIComponent(needle)}`
  );
  return handle(res);
}

export async function createQuiz(quiz) {
  const res = await fetch(API_BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(quiz),
    credentials: "include",
  });

  if (!res.ok) {
    const text = await res.text();
    console.error("Create quiz failed:", res.status, text);
    throw new Error(text || `Create failed (${res.status})`);
  }

  return res.json();
}


export async function updateQuiz(id, quiz) {
  const res = await fetch(`${API_BASE}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(quiz),
    credentials: "include",
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
}

export async function deleteQuiz(id) {
  const res = await fetch(`${API_BASE}/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
}
