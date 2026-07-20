import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "antd";
import {
  BookOutlined, CheckCircleOutlined, DollarOutlined, MenuOutlined,
  ScheduleOutlined, SolutionOutlined, CloseOutlined,
} from "@ant-design/icons";
import "./style.scss";

const FEATURES = [
  {
    icon: <BookOutlined />,
    title: "Class Management",
    description: "Organize classes, schedules and rosters in one place, with teacher and student assignment built in.",
  },
  {
    icon: <ScheduleOutlined />,
    title: "Attendance Tracking",
    description: "Record attendance per session and keep a full history for every class and student.",
  },
  {
    icon: <SolutionOutlined />,
    title: "Scores & Evaluation",
    description: "Track lesson evaluations, scores and progress statistics across every class.",
  },
  {
    icon: <DollarOutlined />,
    title: "Tuition & Billing",
    description: "Manage tuition fees, payments and automated billing cycles for every tenant.",
  },
];

const NAV_LINKS = [
  { href: "#home", label: "Home" },
  { href: "#features", label: "Features" },
  { href: "#about", label: "About" },
  { href: "#contact", label: "Contact" },
];

const Landing = () => {
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className="landing">
      <header className="landing-nav">
        <div className="landing-nav-inner">
          <a href="#home" className="landing-logo">
            <div className="logo-mark">V</div>
            <span className="logo-text">V-Academy Connect</span>
          </a>

          <nav className={`landing-nav-links ${menuOpen ? "open" : ""}`}>
            {NAV_LINKS.map((link) => (
              <a key={link.href} href={link.href} onClick={() => setMenuOpen(false)}>
                {link.label}
              </a>
            ))}
            <div className="landing-nav-actions mobile-only">
              <Button onClick={() => navigate("/login")}>Log In</Button>
              <Button type="primary" onClick={() => navigate("/register")}>Get Started</Button>
            </div>
          </nav>

          <div className="landing-nav-actions desktop-only">
            <Button onClick={() => navigate("/login")}>Log In</Button>
            <Button type="primary" onClick={() => navigate("/register")}>Get Started</Button>
          </div>

          <button
            className="landing-nav-toggle"
            aria-label="Toggle menu"
            onClick={() => setMenuOpen((open) => !open)}
          >
            {menuOpen ? <CloseOutlined /> : <MenuOutlined />}
          </button>
        </div>
      </header>

      <section id="home" className="landing-hero">
        <div className="landing-hero-content">
          <span className="landing-eyebrow">Education management, simplified</span>
          <h1>Run your academy without the spreadsheets</h1>
          <p>
            V-Academy Connect brings classes, attendance, scores and tuition billing
            together in one platform — built for tutoring centers and academies of any size.
          </p>
          <div className="landing-hero-actions">
            <Button type="primary" size="large" onClick={() => navigate("/register")}>
              Get Started Free
            </Button>
            <Button size="large" onClick={() => navigate("/login")}>
              Log In
            </Button>
          </div>
        </div>
      </section>

      <section id="features" className="landing-features">
        <div className="landing-section-header">
          <span className="landing-eyebrow">Features</span>
          <h2>Everything your academy needs</h2>
        </div>
        <div className="landing-features-grid">
          {FEATURES.map((feature) => (
            <div key={feature.title} className="landing-feature-card">
              <div className="feature-icon">{feature.icon}</div>
              <h3>{feature.title}</h3>
              <p>{feature.description}</p>
            </div>
          ))}
        </div>
      </section>

      <section id="about" className="landing-about">
        <div className="landing-about-content">
          <span className="landing-eyebrow">About</span>
          <h2>Built for multi-tenant academies</h2>
          <p>
            V-Academy Connect is a multi-tenant platform: each academy manages its own staff,
            classes and students independently, while subscription and billing are handled
            centrally with automated recurring jobs — so admins spend less time on operations
            and more time teaching.
          </p>
          <ul className="landing-about-list">
            <li><CheckCircleOutlined /> Role-based access for Admins, Tenant Admins and Teachers</li>
            <li><CheckCircleOutlined /> Automated subscription billing with Quartz.NET jobs</li>
            <li><CheckCircleOutlined /> Full audit trail for payments and billing history</li>
          </ul>
        </div>
      </section>

      <section id="contact" className="landing-contact">
        <div className="landing-section-header">
          <span className="landing-eyebrow">Contact</span>
          <h2>Get in touch</h2>
          <p>Have a question about onboarding your academy? Reach out and we'll get back to you.</p>
        </div>
        <div className="landing-contact-cards">
          <div className="landing-contact-card">
            <h3>Email</h3>
            <a href="mailto:support@vacademyconnect.com">support@vacademyconnect.com</a>
          </div>
          <div className="landing-contact-card">
            <h3>Already a member?</h3>
            <Link to="/login">Log in to your account</Link>
          </div>
        </div>
      </section>

      <footer className="landing-footer">
        <span>&copy; {new Date().getFullYear()} V-Academy Connect. All rights reserved.</span>
      </footer>
    </div>
  );
};

export default Landing;
