const API = "http://localhost:5078";

export async function getCalendar(barberId, month, authFetch) {
  // month from <input type="month"> is "YYYY-MM" → backend expects full date
  const normalizedMonth = month.length === 7 ? `${month}-01` : month;

  const res = await authFetch(
    `${API}/api/calendar?barberId=${barberId}&month=${normalizedMonth}`
  );

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    console.error("Calendar API error:", text);
    throw new Error("Failed to load calendar");
  }

  return await res.json();
}
