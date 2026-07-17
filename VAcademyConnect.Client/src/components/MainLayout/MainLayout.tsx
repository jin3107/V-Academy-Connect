import { type ReactNode, useState, useMemo, useCallback } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { logOutAsync } from "../../apis/accountAPI";
import { useAuth } from "../../contexts";
import {
  BookOutlined,
  ClusterOutlined,
  DashboardOutlined, LoginOutlined, MenuFoldOutlined, MenuUnfoldOutlined,
  ScheduleOutlined, ShoppingOutlined, TeamOutlined, UserOutlined,
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

  // Lấy thông tin user từ context
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
    // if (path.startsWith("/students")) return ["students"];
    return ["dashboard"];
  }, [location.pathname]);

  // Menu items based on role
  const getMenuItems = useMemo(() => {
    const role = userRole;

    // SuperAdmin: Can see everything
    if (role === "SuperAdmin") {
      return [
        { key: "dashboard", icon: <DashboardOutlined />, label: "Dashboard" },
        { key: "staffs", icon: <TeamOutlined />, label: "Staffs" },
        { key: "subscriptions", icon: <ShoppingOutlined />, label: "Subscriptions" },
        { key: "tenants", icon: <ClusterOutlined />, label: "Tenants" },
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" }
      ];
    }

    // TenantAdmin: Can see Staffs and Classes
    if (role === "TenantAdmin") {
      return [
        { key: "dashboard", icon: <DashboardOutlined />, label: "Dashboard" },
        { key: "staffs", icon: <TeamOutlined />, label: "Staffs" },
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" }
      ];
    }

    // Teacher: Can see Classes and Attendance (Students commented for future)
    if (role === "Teacher") {
      return [
        { key: "classes", icon: <BookOutlined />, label: "Classes" },
        { key: "attendances", icon: <ScheduleOutlined />, label: "Attendance" },
        // { key: "students", icon: <UserOutlined />, label: "Students" } // TODO: Implement later
      ];
    }

    // Default: No menu items for users without roles
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
      // Vẫn logout ở client side ngay cả khi API call thất bại
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
    // else if (key === "students") navigate("/students"); // TODO: Implement later
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
          <div className="logo-circle"></div>
          {!collapsed && <span className="logo-text">Dashboard</span>}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={selectedKey}
          items={getMenuItems}
          onClick={handleMenuClick}
        />
      </Sider>

      {/* Khối phải tịnh tiến theo bề rộng Sider */}
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

        {/* Content tự cuộn, không làm Sider cuộn */}
        <Content className="main-content" style={{ overflow: "auto" }}>
          <div className="content-wrapper">{children}</div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
