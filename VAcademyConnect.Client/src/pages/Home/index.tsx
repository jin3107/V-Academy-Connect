import { useEffect, useMemo, useState, type ReactNode } from "react";
import { Typography, Spin } from "antd";
import {
  BookOutlined, CheckCircleOutlined, ClusterOutlined, ShoppingOutlined, TeamOutlined,
} from "@ant-design/icons";
import MainLayout from "../../components/MainLayout/MainLayout";
import { useAuth } from "../../contexts";
import { searchTenants } from "../../apis/tenantAPI";
import { searchSubscriptions } from "../../apis/subscriptionAPI";
import { searchStaffs } from "../../apis/staffAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchPayments } from "../../apis/paymentAPI";
import type { AppResponse } from "../../helpers";
import type { SearchResponse } from "../../interfaces";
import './style.scss';

const { Title, Text } = Typography;

interface StatDef {
  key: string;
  label: string;
  icon: ReactNode;
  fetchCount: () => Promise<AppResponse<unknown>>;
}

const SUPER_ADMIN_STATS: StatDef[] = [
  { key: "tenants", label: "Tenants", icon: <ClusterOutlined />, fetchCount: () => searchTenants({ pageIndex: 1, pageSize: 1 }) },
  { key: "subscriptions", label: "Subscriptions", icon: <ShoppingOutlined />, fetchCount: () => searchSubscriptions({ pageIndex: 1, pageSize: 1 }) },
  { key: "staffs", label: "Staffs", icon: <TeamOutlined />, fetchCount: () => searchStaffs({ pageIndex: 1, pageSize: 1 }) },
];

const TENANT_ADMIN_STATS: StatDef[] = [
  { key: "staffs", label: "Staffs", icon: <TeamOutlined />, fetchCount: () => searchStaffs({ pageIndex: 1, pageSize: 1 }) },
  { key: "classes", label: "Classes", icon: <BookOutlined />, fetchCount: () => searchClasses({ pageIndex: 1, pageSize: 1 }) },
  { key: "payments", label: "Payments", icon: <CheckCircleOutlined />, fetchCount: () => searchPayments({ pageIndex: 1, pageSize: 1 }) },
];

const Home = () => {
  const { user } = useAuth();
  const [counts, setCounts] = useState<Record<string, number | null>>({});
  const [loading, setLoading] = useState(true);

  const statDefs = useMemo<StatDef[]>(() => {
    if (user?.role === "SuperAdmin") return SUPER_ADMIN_STATS;
    if (user?.role === "TenantAdmin") return TENANT_ADMIN_STATS;
    return [];
  }, [user?.role]);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);

    Promise.all(
      statDefs.map(async (def) => {
        try {
          const response = await def.fetchCount();
          if (!response.isSuccess) return [def.key, null] as const;
          const paginated = response.data as unknown as SearchResponse<unknown>["data"];
          return [def.key, paginated?.totalRows ?? 0] as const;
        } catch (error) {
          console.error(`Failed to load stat "${def.key}": `, error);
          return [def.key, null] as const;
        }
      })
    ).then((entries) => {
      if (!cancelled) {
        setCounts(Object.fromEntries(entries));
        setLoading(false);
      }
    });

    return () => { cancelled = true; };
  }, [statDefs]);

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
          {statDefs.map((def) => (
            <div className="stat-card" key={def.key}>
              <div className="stat-icon">{def.icon}</div>
              <div className="stat-content">
                <Title level={4} className="stat-label">{def.label}</Title>
                {loading ? (
                  <Spin size="small" />
                ) : (
                  <Title level={2} className="stat-number">{counts[def.key] ?? "—"}</Title>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </MainLayout>
  );
};

export default Home;
