// /pages/Home/index.tsx
import { Typography } from "antd";
import MainLayout from "../../components/MainLayout/MainLayout";
import './style.scss';

const { Title, Text } = Typography;

const Home = () => {
  return (
    <MainLayout>
      <div className="home-page">
        <div className="page-header">
          <Title level={2}>Welcome to Your Dashboard</Title>
          <Text type="secondary">
            This is your personalized dashboard. You can manage your account and settings here.
          </Text>
        </div>

        <div className="dashboard-stats">
          <div className="stat-card">
            <div className="stat-icon">📊</div>
            <div className="stat-content">
              <Title level={4} className="stat-label">Projects</Title>
              <Title level={2} className="stat-number">12</Title>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">📋</div>
            <div className="stat-content">
              <Title level={4} className="stat-label">Tasks</Title>
              <Title level={2} className="stat-number">48</Title>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">✅</div>
            <div className="stat-content">
              <Title level={4} className="stat-label">Completed</Title>
              <Title level={2} className="stat-number">36</Title>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default Home;