import { Navigate } from "react-router-dom";
import type { JSX } from "react";
import { useAuth } from "../../contexts";

interface PrivateRouteProps {
  children: JSX.Element;
  allowedRoles?: string[];
}

const PrivateRoute = ({ children, allowedRoles }: PrivateRouteProps) => {
  const { isAuthenticated, user, getUserRoles } = useAuth();

  // Kiểm tra xem user đã đăng nhập chưa
  if (!isAuthenticated || !user) {
    return <Navigate to="/login" replace />;
  }

  // Kiểm tra quyền truy cập
  if (allowedRoles && allowedRoles.length > 0) {
    const userRoles = getUserRoles();
    const hasPermission = allowedRoles.some(role => 
      userRoles.includes(role.toLowerCase())
    );

    if (!hasPermission) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  return children;
};

export default PrivateRoute;
