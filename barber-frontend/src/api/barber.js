// src/api/barber.js

const API = "http://localhost:5078";

// GET bookings for a barber
export async function getBarberBookings(barberId, authFetch) {
  const res = await authFetch(`${API}/api/bookings/barber/${barberId}`);

  if (!res.ok) {
    console.error("Failed to load bookings:", res.status);
    return [];
  }

  const data = await res.json();
  return Array.isArray(data) ? data : [];
}

// GET days off for a barber
export async function getDaysOff(barberId, authFetch) {
  const res = await authFetch(`${API}/api/daysoff/barber/${barberId}`);

  if (!res.ok) {
    console.error("Failed to load days off:", res.status);
    return [];
  }

  const data = await res.json();
  return Array.isArray(data) ? data : [];
}

// CREATE day off
export async function createDayOff(payload, authFetch) {
  const res = await authFetch(`${API}/api/daysoff`, {
    method: "POST",
    body: JSON.stringify(payload),
  });

  if (!res.ok) throw new Error("Failed to create day off");
  return await res.json();
}

// DELETE day off
export async function deleteDayOff(id, authFetch) {
  const res = await authFetch(`${API}/api/daysoff/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Failed to delete day off");
  return true;
}

// ⭐ FIXED — Get all barbers (safe, normalized, never returns undefined)
export async function getBarbers(authFetch) {
  const res = await authFetch(`${API}/api/barbers`);

  if (!res.ok) {
    console.error("Failed to load barbers:", res.status);
    return [];
  }

  const data = await res.json();

  const list =
    Array.isArray(data)
      ? data
      : Array.isArray(data?.result)
      ? data.result
      : Array.isArray(data?.data)
      ? data.data
      : Array.isArray(data?.items)
      ? data.items
      : [];

  const cleaned = (Array.isArray(list) ? list : []).filter((x) => x && typeof x === "object");

  return cleaned.map((x) => ({
    id: x.id ?? x.Id,
    userId: x.userId ?? x.UserId,
    specialization: x.specialization ?? x.Specialization ?? "",
    isActive: x.isActive ?? x.IsActive ?? true,
    fullName: x.fullName ?? x.FullName ?? x.user?.fullName ?? x.user?.FullName ?? "(Unknown Barber)",
  }));
}
