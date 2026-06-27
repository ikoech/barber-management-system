// src/api/workingHours.js

import { API_BASE } from "./config";

// GET working hours for a barber
export async function getWorkingHours(barberId, authFetch) {
  if (!barberId && barberId !== 0) return [];

  const res = await authFetch(`${API_BASE}/api/workinghours/${barberId}`);

  if (!res.ok) {
    console.error("Failed to load working hours:", res.status);
    return [];
  }

  const data = await res.json();
  return Array.isArray(data) ? data : [];
}

// CREATE working hours
export async function createWorkingHours(payload, authFetch) {
  const res = await authFetch(`${API_BASE}/api/workinghours`, {
    method: "POST",
    body: JSON.stringify(payload),
  });

  if (!res.ok) throw new Error("Failed to create working hours");
  return await res.json();
}

// DELETE working hours
export async function deleteWorkingHours(id, authFetch) {
  const res = await authFetch(`${API_BASE}/api/workinghours/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Failed to delete working hours");
  return true;
}

// Get availability for booking flow
export async function getWorkingHoursForBarber(barberId, date, serviceId, authFetch) {
  if (barberId === null || barberId === undefined || barberId === "") return {};
  if (serviceId === null || serviceId === undefined || serviceId === "") return {};

  const today = new Date().toISOString().split("T")[0];

  const url = `${API_BASE}/api/workinghours/barber/${barberId}?date=${today}&serviceId=1&stepMinutes=15`;
  const res = await authFetch(url);

  if (!res.ok) {
    console.error("Failed to load working hours:", res.status);
    return {};
  }

  const data = await res.json();
  return data ?? {};
}

export async function getWorkingHoursForBarberCRUD(barberId, authFetch) {
  if (!barberId && barberId !== 0) return [];

  const res = await authFetch(`${API_BASE}/api/workinghours/${barberId}`);

  if (!res.ok) {
    console.error("Failed to load working hours:", res.status);
    return [];
  }

  const data = await res.json();
  return Array.isArray(data) ? data : [];
}
