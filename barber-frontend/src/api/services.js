const API = "http://localhost:5078";

// Get all services
export async function getServices(authFetch) {
  const res = await authFetch(`${API}/api/services`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

// Get service by ID
export async function getServiceById(id, authFetch) {
  const res = await authFetch(`${API}/api/services/${id}`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}
