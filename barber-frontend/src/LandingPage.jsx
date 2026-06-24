import "./LandingPage.css";
import { Link } from "react-router-dom";

export default function LandingPage() {
  return (
    <div className="landing">

      {/* NAVBAR */}
      <nav className="landing-nav">
        <div className="logo">Barber System</div>
        <div className="nav-links">
          <Link to="/login">Login</Link>
          <Link to="/register" className="btn-primary">Create Account</Link>
        </div>
      </nav>

      {/* HERO */}
      <section className="hero">
        <h1>Book Your Barber. Fast. Simple.</h1>
        <p>Modern scheduling for barbers and clients — built for speed and simplicity.</p>

        <div className="hero-buttons">
          <Link to="/login" className="btn-primary">Login</Link>
          <Link to="/register" className="btn-secondary">Create Account</Link>
        </div>
      </section>

      {/* FEATURES */}
      <section className="features">
        <h2>Why Choose Our System?</h2>

        <div className="feature-grid">
          <div className="feature-card">
            <h3>Smart Calendar</h3>
            <p>Manage bookings, days off, and availability with ease.</p>
          </div>

          <div className="feature-card">
            <h3>Breaks & Days Off</h3>
            <p>Control your schedule with flexible break management.</p>
          </div>

          <div className="feature-card">
            <h3>Barber Dashboard</h3>
            <p>All your tools in one clean, simple interface.</p>
          </div>

          <div className="feature-card">
            <h3>Fast Booking</h3>
            <p>Clients can book appointments in seconds.</p>
          </div>
        </div>
      </section>

      {/* SCREENSHOTS */}
      <section className="screenshots">
        <h2>Preview</h2>

        <div className="screenshot-grid">
          <div className="screenshot-card">Calendar Preview</div>
          <div className="screenshot-card">Breaks Preview</div>
          <div className="screenshot-card">Login Preview</div>
        </div>
      </section>

      {/* FOOTER */}
      <footer className="landing-footer">
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
