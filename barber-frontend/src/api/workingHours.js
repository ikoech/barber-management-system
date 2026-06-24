// src/api/workingHours.js

const API = "http://localhost:5078";

// GET working hours for a barber
export async function getWorkingHours(barberId, authFetch) {
  if (!barberId && barberId !== 0) return [];

  const res = await authFetch(`${API}/api/workinghours/barber/${barberId}`);

  if (!res.ok) {
    // Never break booking UI on 403/404. Return empty slots.
    console.error("Failed to load working hours:", res.status);
    return [];
  }

  const data = await res.json();
  return Array.isArray(data) ? data : [];
}

// CREATE working hours
export async function createWorkingHours(payload, authFetch) {
  const res = await authFetch(`${API}/api/workinghours`, {
    method: "POST",
    body: JSON.stringify(payload),
  });

  if (!res.ok) throw new Error("Failed to create working hours");
  return await res.json();
}

// DELETE working hours
export async function deleteWorkingHours(id, authFetch) {
  const res = await authFetch(`${API}/api/workinghours/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Failed to delete working hours");
  return true;
}
// ⭐ NEW — Get availability for a specific barber + date (required by booking flow)
export async function getWorkingHoursForBarber(barberId, date, serviceId, authFetch) {
  if (barberId === null || barberId === undefined || barberId === "") return {};
  if (serviceId === null || serviceId === undefined || serviceId === "") return {};

  const res = await authFetch(
    `${API}/api/workinghours/barber/${barberId}?date=${encodeURIComponent(date ?? "")}&serviceId=${encodeURIComponent(serviceId)}`
  );

  if (!res.ok) {
    console.error("Failed to load working hours:", res.status);
    return {};
  }

  const data = await res.json();
  return data ?? {};
}



