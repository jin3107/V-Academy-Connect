import { type ReactNode, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
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
import "./style.scss";

const { Header, Sider, Content } = Layout;

interface MainLayoutProps {
  children: ReactNode;
}

const MainLayout = ({ children }: MainLayoutProps) => {
  const [collapsed, setCollapsed] = useState(false);
  const [userName, setUserName] = useState("User");
  const navigate = useNavigate();

  useEffect(() => {
    setUserName("Chuong Huynh");
  }, []);

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

  const userMenuItems = [
    {
      key: "profile",
      icon: <UserOutlined />,
      label: "Profile",
    },
    {
      key: "settings",
      icon: <SettingOutlined />,
      label: "Settings",
    },
    {
      key: "logout",
      icon: <LoginOutlined />,
      label: "Logout",
      onClick: handleLogout,
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
          defaultOpenKeys={["dashboard"]}
          onClick={({ key }) => {
            if (key === "dashboard") navigate("/home");
            if (key === "staff") navigate("/staffs");
            if (key === "settings") navigate("/settings");
          }}
          items={[
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
          ]}
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
            >
              <div className="user-info">
                <Avatar icon={<UserOutlined />} />
                {!collapsed && <span className="user-name">{userName}</span>}
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
