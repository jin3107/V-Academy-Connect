import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import "./App.css";
import { PrivateRoute } from "./components";
import { 
  LogIn, Register, ChangePassword, VerifyEmail, Staffs,
  Home, Subscriptions
 } from "./pages";

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
                  <Staffs />
                </PrivateRoute>
              } 
            />

            <Route 
              path="/subscriptions"
              element={
                <PrivateRoute allowedRoles={["SuperAdmin"]}>
                  <Subscriptions />
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
