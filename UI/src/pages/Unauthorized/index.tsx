import { Button, Result } from "antd";
import { useNavigate } from "react-router-dom";
import "./style.scss";

const Unauthorized = () => {
  const navigate = useNavigate();

  const handleGoBack = () => {
    const userInfoStr = localStorage.getItem("userInfo");
    if (userInfoStr) {
      try {
        const userInfo = JSON.parse(userInfoStr);
        // Redirect based on role
        if (userInfo.role === "SuperAdmin" || userInfo.role === "TenantAdmin") {
          navigate("/home");
        } else if (userInfo.role === "Teacher") {
          navigate("/classes");
        } else {
          navigate("/login");
        }
      } catch {
        navigate("/login");
      }
    } else {
      navigate("/login");
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userInfo");
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
