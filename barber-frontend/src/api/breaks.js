const API = "http://localhost:5078";

export async function getBreaks(barberId, authFetch) {
  const res = await authFetch(`${API}/api/breaks/barber/${barberId}`);
  if (!res.ok) {
    console.error("Failed to load breaks:", res.status);
    return [];
  }
  return await res.json();
}

export async function createBreak(payload, authFetch) {
  const res = await authFetch(`${API}/api/breaks`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
  if (!res.ok) throw new Error("Failed to create break");
  return await res.json();
}

export async function deleteBreak(id, authFetch) {
  const res = await authFetch(`${API}/api/breaks/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete break");
  return true;
}
