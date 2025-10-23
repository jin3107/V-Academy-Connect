// import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
// import "./App.css";
// import { PrivateRoute } from "./components";
// import { 
//   LogIn, Register, ChangePassword, VerifyEmail, Staffs,
//   Home, Subscriptions
//  } from "./pages";
// import Tenant from "./pages/Tenant";

// function App() {
//   return (
//     <div className="app">
//       <div className="content">
//         <Router>
//           <Routes>
//             <Route path="/login" element={<LogIn />} />

//             <Route path="/register" element={<Register />} />
//             <Route path="/verify-email" element={<VerifyEmail />} />
//             <Route path="/change-password" element={<ChangePassword />} />

//             <Route 
//               path="/home"
//               element={
//                 <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin"]}>
//                   <Home />
//                 </PrivateRoute>
//               }
//             />

//             <Route
//               path="/staffs"
//               element={
//                 <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin"]}>
//                   <Staffs />
//                 </PrivateRoute>
//               } 
//             />

//             <Route 
//               path="/subscriptions"
//               element={
//                 <PrivateRoute allowedRoles={["SuperAdmin"]}>
//                   <Subscriptions />
//                 </PrivateRoute>
//               }
//             />

//             <Route 
//               path="/tenants"
//               element={
//                 <PrivateRoute allowedRoles={["SuperAdmin"]}>
//                   <Tenant />
//                 </PrivateRoute>
//               }
//             />

//             <Route path="/" element={<Navigate to="/login" replace />} />
//           </Routes>
//         </Router>
//       </div>
//     </div>
//   );
// }

// export default App;
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import "./App.css";
import { PrivateRoute } from "./components";
import { 
  LogIn, Register, ChangePassword, VerifyEmail, Staffs,
  Home, Subscriptions, Tenants, Unauthorized
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

            <Route 
              path="/tenants"
              element={
                <PrivateRoute allowedRoles={["SuperAdmin"]}>
                  <Tenants />
                </PrivateRoute>
              }
            />

            {/* <Route 
              path="/classes"
              element={
                <PrivateRoute allowedRoles={["SuperAdmin", "TenantAdmin", "Teacher"]}>
                  <Class />
                </PrivateRoute>
              }
            /> */}

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

            <Route path="/" element={<Navigate to="/login" replace />} />
          </Routes>
        </Router>
      </div>
    </div>
  );
}

export default App;
