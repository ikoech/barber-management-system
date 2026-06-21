/*eslint no-unused-vars: "error"*/

import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./pages/Login.jsx";
import Dashboard from "./pages/Dashboard.jsx";
import Services from "./pages/Services.jsx";
import DateSelection from "./pages/DateSelection.jsx";
import TimeSelection from "./pages/TimeSelection.jsx";
import ConfirmBooking from "./pages/ConfirmBooking.jsx";
import "./index.css";

ReactDOM.createRoot(document.getElementById("root")).render(
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/booking/services" element={<Services />} />
            <Route path="/booking/date" element={<DateSelection />} />   
            <Route path="/booking/time" element={<TimeSelection />} /> 
            <Route path="/booking/confirm" element={<ConfirmBooking />} />
        </Routes>
    </BrowserRouter>
);
