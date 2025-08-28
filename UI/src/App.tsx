import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import LogIn from "./pages/LogIn";
import Register from "./pages/Register";
import Home from "./pages/Home";
import "./App.css";
import VerifyEmail from "./pages/VerifyEmail";
import ChangePassword from "./pages/ChangePassword";
import { PrivateRoute } from "./components";
import Staff from "./pages/Staff";

function App() {
  return (
    <div className="app">
      <div className="content">
        <Router>
          <Routes>
            <Route path="/login" element={<LogIn />} />

            <Route path="/register" element={<Register />} />
            <Route path="/verify-email" element={<VerifyEmail />} />
            <Route path="/change-password" element={<ChangePassword />} />

            {/* <Route path="/home" element={<Home />} /> */}
            <Route 
              path="/home"
              element={
                <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin"]}>
                  <Home />
                </PrivateRoute>
              }
            />

            <Route
              path="/staffs"
              element={
                <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin"]}>
                  <Staff />
                </PrivateRoute>
              } 
            />

            <Route path="/" element={<Navigate to="/login" replace />} />
          </Routes>
        </Router>
      </div>
    </div>
  );
}

export default App;
