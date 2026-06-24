import "./LandingPage.css";
import { Link } from "react-router-dom";
import DarkModeToggle from "./components/DarkModeToggle";

export default function LandingPage() {
  return (
    <div className="landing fade-in">

      {/* NAVBAR */}
      <nav className="landing-nav">
        <div className="logo">Barber System</div>
        <div className="nav-links">
          <Link to="/login">Login</Link>
          <Link to="/register" className="btn-primary">Create Account</Link>
          <DarkModeToggle />
        </div>
      </nav>

      {/* HERO */}
      <section className="hero slide-up">
        <h1>💈 Book Your Barber. Fast. Simple.</h1>
        <p>Modern scheduling for barbers and clients — built for speed and simplicity.</p>

        <div className="hero-buttons">
          <Link to="/login" className="btn-primary">Login</Link>
          <Link to="/register" className="btn-secondary">Create Account</Link>
          <Link to="/booking/services" className="px-6 py-3 bg-purple-600 text-white rounded-lg shadow hover:bg-purple-700 transition">Start Booking</Link>
        </div>
      </section>

      {/* FEATURES */}
      <section className="features slide-up">
        <h2>Why Choose Our System?</h2>

        <div className="feature-grid">
          <div className="feature-card">Smart Calendar</div>
          <div className="feature-card">Breaks & Days Off</div>
          <div className="feature-card">Barber Dashboard</div>
          <div className="feature-card">Fast Booking</div>
        </div>
      </section>

      {/* SCREENSHOTS */}
      <section className="screenshots slide-up">
        <h2>Preview</h2>

        <div className="screenshot-grid">
          <div className="screenshot-card">Calendar Preview</div>
          <div className="screenshot-card">Breaks Preview</div>
          <div className="screenshot-card">Login Preview</div>
        </div>
      </section>

      {/* FOOTER */}
      <footer className="landing-footer fade-in">
        <p>© {new Date().getFullYear()} Barber System</p>
        <div className="footer-links">
          <Link to="/about">About</Link>
          <Link to="/terms">Terms</Link>
          <Link to="/contact">Contact</Link>
        </div>
      </footer>

    </div>
  );
}
