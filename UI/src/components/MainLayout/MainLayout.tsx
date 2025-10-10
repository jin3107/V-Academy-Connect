import { type ReactNode, useEffect, useState, useMemo } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { logOutAsync } from "../../apis/accountAPI";
import {
  ClusterOutlined,
  DashboardOutlined, LoginOutlined, MenuFoldOutlined, MenuUnfoldOutlined,
  SettingOutlined, ShoppingOutlined, TeamOutlined, UserOutlined,
} from "@ant-design/icons";
import { Layout, Menu, Button, Avatar, Dropdown } from "antd";
import type { MenuProps } from "antd";
import "./style.scss";

const { Header, Sider, Content } = Layout;

interface MainLayoutProps { children: ReactNode; }

const MainLayout = ({ children }: MainLayoutProps) => {
  const [collapsed, setCollapsed] = useState(false);
  const [userName, setUserName] = useState("User");
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => { setUserName("Chuong Huynh"); }, []);

  const selectedKey = useMemo(() => {
    const path = location.pathname;
    if (path === "/" || path.startsWith("/home")) return ["dashboard"];
    if (path.startsWith("/staffs")) return ["staffs"];
    if (path.startsWith("/settings")) return ["settings"];
    if (path.startsWith("/subscriptions")) return ["subscriptions"]
    return ["dashboard"];
  }, [location.pathname]);

  const handleLogout = async () => {
    try { await logOutAsync(); } catch (err) {
      console.log("Logout failed: ", err);
    } finally {
      localStorage.removeItem("token");
      navigate("/login");
    }
  };

  const handleMenuClick: MenuProps["onClick"] = ({ key }) => {
    if (key === "dashboard") navigate("/home");
    else if (key === "staffs") navigate("/staffs");
    else if (key === "subscriptions") navigate("/subscriptions");
    else if (key === "tenants") navigate("/tenants");
    else if (key === "settings") navigate("/settings");
  };

  const userMenuItems: MenuProps["items"] = [
    { key: "profile",  icon: <UserOutlined/>,    label: "Profile",  onClick: () => navigate("/profile") },
    { key: "settings", icon: <SettingOutlined/>, label: "Settings", onClick: () => navigate("/settings") },
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
          items={[
            { key: "dashboard", icon: <DashboardOutlined />, label: "Dashboard" },
            { key: "staffs",     icon: <TeamOutlined />,      label: "Staffs" },
            { key: "subscriptions",     icon: <ShoppingOutlined />,      label: "Subscriptions" },
            { key: "tenants",     icon: <ClusterOutlined />,      label: "Tenants" },
            { key: "settings",  icon: <SettingOutlined />,   label: "Settings" },
          ]}
          onClick={handleMenuClick}
        />
      </Sider>

      {/* Khối phải tịnh tiến theo bề rộng Sider */}
      <Layout style={{ marginLeft: siderWidth, transition: "margin-left .2s ease" }}>
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
