import { Navigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import type { JSX } from "react";

interface PrivateRouteProps {
  children: JSX.Element;
  allowedRoles?: string[];
}

interface DecodedToken {
  role?: string;
  roles?: string[] | string;
  exp: number;
  [key: string]: unknown;
}

const PrivateRoute = ({ children, allowedRoles }: PrivateRouteProps) => {
  const token = localStorage.getItem("token");

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  try {
    const decoded: DecodedToken = jwtDecode(token);
    console.log("Decoded token:", decoded);

    // Kiểm tra token hết hạn
    if (decoded.exp * 1000 < Date.now()) {
      localStorage.removeItem("token");
      return <Navigate to="/login" replace />;
    }

    // Lấy role từ nhiều field khác nhau
    let userRoles: string[] = [];

    if (decoded.role) userRoles.push(decoded.role);
    if (decoded.roles) {
      if (Array.isArray(decoded.roles)) {
        userRoles = [...userRoles, ...decoded.roles];
      } else {
        userRoles.push(decoded.roles);
      }
    }
    if (decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) {
      const claimRoles = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      if (Array.isArray(claimRoles)) {
        userRoles = [...userRoles, ...claimRoles];
      } else {
        userRoles.push(claimRoles as string);
      }
    }

    // Xoá duplicate và chuẩn hoá chữ thường
    userRoles = [...new Set(userRoles.map(r => r.toLowerCase()))];

    // Kiểm tra quyền
    if (allowedRoles && !allowedRoles.some(r => userRoles.includes(r.toLowerCase()))) {
      return <Navigate to="/unauthorized" replace />;
    }

    return children;
  } catch (err) {
    console.error("Token decode error:", err);
    localStorage.removeItem("token");
    return <Navigate to="/login" replace />;
  }
};

export default PrivateRoute;
