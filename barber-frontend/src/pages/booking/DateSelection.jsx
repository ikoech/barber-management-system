// src/pages/booking/DateSelection.jsx
import { useLocation, useNavigate } from "react-router-dom";
import { useState } from "react";

export default function DateSelection() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const [date, setDate] = useState("");

  if (!state) return <div className="page-container">Missing booking context.</div>;

  function handleNext() {
    if (!date) return;
    navigate("/booking/time", { state: { ...state, date } });
  }

  return (
    <div className="page-container">
      <h1>Select Date</h1>
      <input
        type="date"
        value={date}
        onChange={e => setDate(e.target.value)}
      />
      <button onClick={handleNext}>Next</button>
    </div>
  );
}
