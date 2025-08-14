import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import LogIn from "./pages/LogIn";
import Register from "./pages/Register";
import Home from "./pages/Home";
import "./App.css";
import VerifyEmail from "./pages/VerifyEmail";

function App() {
  return (
    <div className="app">
      <div className="content">
        <Router>
          <Routes>
            <Route path="/login" element={<LogIn />} />
            <Route path="/register" element={<Register />} />
            <Route path="/verify-email" element={<VerifyEmail />} />
            <Route path="/home" element={<Home />} />
            <Route path="/" element={<Navigate to="/login" replace />} />
          </Routes>
        </Router>
      </div>
    </div>
  );
}

export default App;
