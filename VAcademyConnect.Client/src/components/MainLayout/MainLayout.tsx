import { type ReactNode, useState, useMemo, useCallback } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { logOutAsync } from "../../apis/accountAPI";
import { useAuth } from "../../contexts";
import {
  BarChartOutlined, BookOutlined,
  ClusterOutlined, CheckCircleOutlined, HistoryOutlined,
  DashboardOutlined, DollarOutlined, LoginOutlined, MenuFoldOutlined, MenuUnfoldOutlined,
  ScheduleOutlined, ShoppingOutlined, SolutionOutlined, StarOutlined, TeamOutlined, UserOutlined,
} from "@ant-design/icons";
import { Layout, Menu, Button, Avatar, Dropdown, message } from "antd";
import type { MenuProps } from "antd";
import "./style.scss";

const { Header, Sider, Content } = Layout;

interface MainLayoutProps { children: ReactNode; }

const MainLayout = ({ children }: MainLayoutProps) => {
  const [collapsed, setCollapsed] = useState(false);
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const userName = user?.userName || user?.email?.split('@')[0] || user?.email || "User";
  const userRole = user?.role || "";

  const selectedKey = useMemo(() => {
    const path = location.pathname;
    if (path === "/" || path.startsWith("/home")) return ["dashboard"];
    if (path.startsWith("/staffs")) return ["staffs"];
    if (path.startsWith("/classes")) return ["classes"];
    if (path.startsWith("/subscriptions")) return ["subscriptions"];
    if (path.startsWith("/tenants")) return ["tenants"];
    if (path.startsWith("/attendances")) return ["attendances"];
    if (path.startsWith("/lesson-evaluations")) return ["lesson-evaluations"];
    if (path.startsWith("/score-statistics")) return ["score-statistics"];
    if (path.startsWith("/tuition-fees")) return ["tuition-fees"];
    if (path.startsWith("/payments")) return ["payments"];
    if (path.startsWith("/background-jobs")) return ["background-jobs"];
    if (path.startsWith("/scores")) return ["scores"];
    return ["dashboard"];
  }, [location.pathname]);

  const getMenuItems = useMemo(() => {
    const role = userRole;

    if (role === "SuperAdmin") {
      return [
        { key: "dashboard", icon: <DashboardOutlined />, label: "Dashboard" },
        { key: "staffs", icon: <TeamOutlined />, label: "Staffs" },
        { key: "subscriptions", icon: <ShoppingOutlined />, label: "Subscriptions" },
        { key: "tenants", icon: <ClusterOutlined />, label: "Tenants" },
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" },
        { key: "lesson-evaluations", icon: <StarOutlined />, label: "Lesson Evaluation" },
        { key: "scores", icon: <SolutionOutlined />, label: "Scores" },
        { key: "score-statistics", icon: <BarChartOutlined />, label: "Score Statistics" },
        { key: "tuition-fees", icon: <DollarOutlined />, label: "Tuition Fees" },
        { key: "payments", icon: <CheckCircleOutlined />, label: "Payments" },
        { key: "background-jobs", icon: <HistoryOutlined />, label: "Billing Job History" }
      ];
    }

    if (role === "TenantAdmin") {
      return [
        { key: "dashboard", icon: <DashboardOutlined />, label: "Dashboard" },
        { key: "staffs", icon: <TeamOutlined />, label: "Staffs" },
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" },
        { key: "lesson-evaluations", icon: <StarOutlined />, label: "Lesson Evaluation" },
        { key: "scores", icon: <SolutionOutlined />, label: "Scores" },
        { key: "score-statistics", icon: <BarChartOutlined />, label: "Score Statistics" },
        { key: "tuition-fees", icon: <DollarOutlined />, label: "Tuition Fees" },
        { key: "payments", icon: <CheckCircleOutlined />, label: "Payments" }
      ];
    }

    if (role === "Teacher") {
      return [
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" },
        { key: "lesson-evaluations", icon: <StarOutlined />, label: "Lesson Evaluation" },
        { key: "scores", icon: <SolutionOutlined />, label: "Scores" },
        { key: "score-statistics", icon: <BarChartOutlined />, label: "Score Statistics" },
        { key: "tuition-fees", icon: <DollarOutlined />, label: "Tuition Fees" },
        { key: "payments", icon: <CheckCircleOutlined />, label: "Payments" },
      ];
    }

    return [];
  }, [userRole]);

  const handleLogout = useCallback(async () => {
    try {
      await logOutAsync();
      logout();
      message.success('Logged out successfully');
      navigate("/login");
    } catch (err) {
      console.error("Logout failed: ", err);
      logout();
      navigate("/login");
    }
  }, [logout, navigate]);

  const handleMenuClick: MenuProps["onClick"] = useCallback(({ key }: { key: string }) => {
    if (key === "dashboard") navigate("/home");
    else if (key === "staffs") navigate("/staffs");
    else if (key === "subscriptions") navigate("/subscriptions");
    else if (key === "tenants") navigate("/tenants");
    else if (key === "classes") navigate("/classes");
    else if (key === "attendances") navigate("/attendances");
    else if (key === "lesson-evaluations") navigate("/lesson-evaluations");
    else if (key === "scores") navigate("/scores");
    else if (key === "score-statistics") navigate("/score-statistics");
    else if (key === "tuition-fees") navigate("/tuition-fees");
    else if (key === "payments") navigate("/payments");
    else if (key === "background-jobs") navigate("/background-jobs");
  }, [navigate]);

  const userMenuItems: MenuProps["items"] = [
    { key: "profile",  icon: <UserOutlined/>,    label: "Profile",  onClick: () => navigate("/profile") },
    { type: "divider" },
    { key: "logout",   icon: <LoginOutlined/>,   label: "Logout",   onClick: handleLogout },
  ];

  const siderWidth = collapsed ? 80 : 220;

  return (
    <Layout className="main-layout">
      <Sider
        trigger={null}
        collapsible
        collapsed={collapsed}
        width={220}
        className="main-sider"
        style={{
          position: "fixed",
          left: 0, top: 0, bottom: 0,
          height: "100vh",
          overflowY: "auto",
        }}
      >
        <div className="logo">
          <div className="logo-mark">V</div>
          {!collapsed && <span className="logo-text">V-Academy</span>}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={selectedKey}
          items={getMenuItems}
          onClick={handleMenuClick}
        />
      </Sider>

      <Layout style={{
        marginLeft: siderWidth, 
        transition: "margin-left 0.2s cubic-bezier(0.4, 0, 0.2, 1)",
        willChange: "margin-left"
      }}>
        <Header className="main-header">
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined/> : <MenuFoldOutlined/>}
            onClick={() => setCollapsed(!collapsed)}
            className="trigger-button"
          />
          <div className="header-right">
            <Dropdown menu={{ items: userMenuItems }} placement="bottomRight" arrow trigger={["click"]}>
              <div className="user-info">
                <Avatar icon={<UserOutlined />} />
                <span className="user-name">{userName}</span>
              </div>
            </Dropdown>
          </div>
        </Header>

        <Content className="main-content" style={{ overflow: "auto" }}>
          <div className="content-wrapper">{children}</div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
