import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "./App.css";
import { PrivateRoute } from "./components";
import { AuthProvider } from "./contexts";
import {
  Landing, LogIn, Register, ChangePassword, VerifyEmail, Staffs,
  Home, Profile, Subscriptions, Tenants, Classes, Attendances, LessonEvaluations, Scores, ScoreStatistics, TuitionFees, Payments,
  BackgroundJobs, Unauthorized
 } from "./pages";

function App() {
  return (
    <div className="app">
      <div className="content">
        <Router>
          <AuthProvider>
            <Routes>
              <Route path="/" element={<Landing />} />
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
                path="/profile"
                element={
                  <PrivateRoute>
                    <Profile />
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

              <Route 
                path="/tenants"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin"]}>
                    <Tenants />
                  </PrivateRoute>
                }
              />

              <Route
                path="/classes"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin", "Teacher"]}>
                    <Classes />
                  </PrivateRoute>
                }
              />

              <Route
                path="/attendances"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <Attendances />
                  </PrivateRoute>
                }
              />

              <Route
                path="/lesson-evaluations"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <LessonEvaluations />
                  </PrivateRoute>
                }
              />

              <Route
                path="/scores"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <Scores />
                  </PrivateRoute>
                }
              />

              <Route
                path="/score-statistics"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <ScoreStatistics />
                  </PrivateRoute>
                }
              />

              <Route
                path="/tuition-fees"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <TuitionFees />
                  </PrivateRoute>
                }
              />

              <Route
                path="/payments"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin", "Admin", "TenantAdmin", "Teacher"]}>
                    <Payments />
                  </PrivateRoute>
                }
              />

              <Route
                path="/background-jobs"
                element={
                  <PrivateRoute allowedRoles={["SuperAdmin"]}>
                    <BackgroundJobs />
                  </PrivateRoute>
                }
              />

              {/* TODO: Students page for Teachers */}
              {/* <Route 
                path="/students"
                element={
                  <PrivateRoute allowedRoles={["Teacher"]}>
                    <Students />
                  </PrivateRoute>
                }
              /> */}

              <Route path="/unauthorized" element={<Unauthorized />} />
            </Routes>
          </AuthProvider>
        </Router>
      </div>
    </div>
  );
}

export default App;
