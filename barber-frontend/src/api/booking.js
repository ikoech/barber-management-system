// src/api/booking.js
const API = "http://localhost:5078";

export async function createBooking(dto, authFetch) {
  const res = await authFetch(`${API}/api/bookings`, {
    method: "POST",
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function getBookingsForUser(userId, authFetch) {
  const res = await authFetch(`${API}/api/bookings/user/${userId}`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function getBookingsForBarber(barberId, authFetch) {
  const res = await authFetch(`${API}/api/bookings/barber/${barberId}`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function cancelBooking(bookingId, authFetch) {
  const res = await authFetch(`${API}/api/bookings/${bookingId}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}
