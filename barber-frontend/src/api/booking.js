// src/api/booking.js
const API = "http://localhost:5078";

export async function createBooking(dto, authFetch) {
  // Hard normalize payload to avoid backend crashes from undefined/null.
  const payload = {
    userId: Number(dto?.userId),
    serviceId: Number(dto?.serviceId),
    barberId: Number(dto?.barberId),
    start: dto?.start,
  };

  const res = await authFetch(`${API}/api/bookings`, {
    method: "POST",
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    // backend returns { message }
    try {
      const data = await res.json();
      throw new Error(data?.message || "Failed to create booking");
    } catch {
      const text = await res.text();
      throw new Error(text || "Failed to create booking");
    }
  }


  const data = await res.json();
  return data;
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
