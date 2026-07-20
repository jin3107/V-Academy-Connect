import { Button, Result } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts";
import "./style.scss";

const Unauthorized = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleGoBack = () => {
    if (user) {
      if (user.role === "SuperAdmin" || user.role === "TenantAdmin") {
        navigate("/home");
      } else if (user.role === "Teacher") {
        navigate("/classes");
      } else {
        navigate("/login");
      }
    } else {
      navigate("/login");
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="unauthorized-page">
      <Result
        status="403"
        title="403"
        subTitle="Sorry, you are not authorized to access this page."
        extra={
          <div className="button-group">
            <Button type="primary" onClick={handleGoBack}>
              Go to Dashboard
            </Button>
            <Button onClick={handleLogout}>
              Logout
            </Button>
          </div>
        }
      />
    </div>
  );
};

export default Unauthorized;
