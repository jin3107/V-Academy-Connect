import { type ReactNode, useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { logOutAsync } from "../../apis/accountAPI";
import {
  DashboardOutlined,
  LoginOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  SettingOutlined,
  TeamOutlined,
  UserOutlined,
} from "@ant-design/icons";
import { Layout, Menu, Button, Avatar, Dropdown } from "antd";
import type { MenuProps } from 'antd';
import "./style.scss";

const { Header, Sider, Content } = Layout;

interface MainLayoutProps {
  children: ReactNode;
}

const MainLayout = ({ children }: MainLayoutProps) => {
  const [collapsed, setCollapsed] = useState(false);
  const [userName, setUserName] = useState("User");
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    // Thông thường sẽ lấy từ context hoặc API
    setUserName("Chuong Huynh");
  }, []);

  // Xác định menu item được chọn dựa trên current path
  const getSelectedKey = () => {
    const path = location.pathname;
    if (path.includes('/home') || path === '/') return ['dashboard'];
    if (path.includes('/staff')) return ['staff'];
    if (path.includes('/settings')) return ['settings'];
    return ['dashboard'];
  };

  const handleLogout = async () => {
    try {
      await logOutAsync();
    } catch (error) {
      console.log("Logout failed: ", error);
    } finally {
      localStorage.removeItem("token");
      navigate("/login");
    }
  };

  const handleMenuClick = ({ key }: { key: string }) => {
    switch (key) {
      case "dashboard":
        navigate("/home");
        break;
      case "staff":
        navigate("/staffs");
        break;
      case "settings":
        navigate("/settings");
        break;
      default:
        break;
    }
  };

  const userMenuItems: MenuProps['items'] = [
    {
      key: "profile",
      icon: <UserOutlined />,
      label: "Profile",
      onClick: () => navigate("/profile")
    },
    {
      key: "settings",
      icon: <SettingOutlined />,
      label: "Settings",
      onClick: () => navigate("/settings")
    },
    {
      type: 'divider',
    },
    {
      key: "logout",
      icon: <LoginOutlined />,
      label: "Logout",
      onClick: handleLogout,
    },
  ];

  const menuItems: MenuProps['items'] = [
    {
      key: "dashboard",
      icon: <DashboardOutlined />,
      label: "Dashboard",
    },
    {
      key: "staff",
      icon: <TeamOutlined />,
      label: "Staff",
    },
    {
      key: "settings",
      icon: <SettingOutlined />,
      label: "Settings",
    },
  ];

  return (
    <Layout className="main-layout">
      <Sider
        trigger={null}
        collapsible
        collapsed={collapsed}
        className="main-sider"
      >
        <div className="logo">
          <div className="logo-circle"></div>
          {!collapsed && <span className="logo-text">Dashboard</span>}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={getSelectedKey()}
          items={menuItems}
          onClick={handleMenuClick}
        />
      </Sider>
      <Layout>
        <Header className="main-header">
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={() => setCollapsed(!collapsed)}
            className="trigger-button"
          />
          <div className="header-right">
            <Dropdown
              menu={{ items: userMenuItems }}
              placement="bottomRight"
              arrow
              trigger={['click']}
            >
              <div className="user-info">
                <Avatar icon={<UserOutlined />} />
                <span className="user-name">{userName}</span>
              </div>
            </Dropdown>
          </div>
        </Header>
        <Content className="main-content">
          <div className="content-wrapper">{children}</div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;