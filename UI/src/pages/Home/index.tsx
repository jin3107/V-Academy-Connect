import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { logOutAsync } from "../../apis/accountAPI";
import { DashboardOutlined, LoginOutlined, MenuFoldOutlined, MenuUnfoldOutlined, SettingOutlined, UserOutlined } from "@ant-design/icons";
import { Layout, Menu, Button, Typography, Avatar, Dropdown } from "antd";
import './style.scss';

const { Header, Sider, Content } = Layout;
const { Title, Text } = Typography;

const Home = () => {
  const [collapsed, setCollapsed] = useState(false);
  const [userName, setUserName] = useState('User');
  const navigate = useNavigate();

  useEffect(() => {
  // Thông thường, chúng ta sẽ lấy thông tin người dùng từ ngữ cảnh hoặc quản lý trạng thái
  // Hiện tại, chúng ta sẽ chỉ sử dụng một trình giữ chỗ
  setUserName('Chuong Huynh');
  }, []);

  const handleLogout = async () => {
    // Clear any auth tokens or user data
    try {
      await logOutAsync();
    } catch (error) {
      console.log('Logout failed: ', error);
    } finally {
      localStorage.removeItem('token');
      navigate('/login');
    }
  };

  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Profile',
    },
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: 'Settings',
    },
    {
      key: 'logout',
      icon: <LoginOutlined />,
      label: 'Logout',
      onClick: handleLogout,
    },
  ];

  return (
    <Layout className="home-layout">
      <Sider trigger={null} collapsible collapsed={collapsed} className="home-sider">
        <div className="logo">
          <div className="logo-circle"></div>
          {!collapsed && <span className="logo-text">Dashboard</span>}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          defaultOpenKeys={['dashboard']}
          items={[
            {
              key: 'dashboard',
              icon: <DashboardOutlined />,
              label: 'Dashboard',
            },
            {
              key: 'settings',
              icon: <SettingOutlined />,
              label: 'Settings',
            },
          ]}
        />
      </Sider>
      <Layout>
        <Header className="home-header">
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
        <Content className="home-content">
          <div className="content-wraper">
            <Title level={2}>Welcome to Your Dashboard</Title>
            <Text>This is your personalized dashboard. You can manage your account and settings here.</Text>

            <div className="dashboard-stats">
              <div className="stat-card">
                <Title level={4}>Projects</Title>
                <Title level={2}>12</Title>
              </div>

              <div className="stat-card">
                <Title level={4}>Tasks</Title>
                <Title level={2}>48</Title>
              </div>

              <div className="stat-card">
                <Title level={4}>Completed</Title>
                <Title level={2}>36</Title>
              </div>
            </div>
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default Home;