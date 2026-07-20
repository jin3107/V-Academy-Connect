import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import { Table, message, Select, Tag } from "antd";
import { SortAscendingOutlined, SortDescendingOutlined } from "@ant-design/icons";
import { searchBackgroundJobs } from "../../apis/backgroundJobAPI";
import { searchTenants } from "../../apis/tenantAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  BackgroundJobResponse, TenantReponse,
} from "../../interfaces";
import "./style.scss";
import dayjs from "dayjs";

const resultColor = (result?: string) => {
  if (!result) return "default";
  if (result === "Error") return "error";
  if (result === "Downgraded to Free") return "warning";
  return "success";
};

const BackgroundJobs = () => {
  const [loading, setLoading] = useState(false);
  const [backgroundJobs, setBackgroundJobs] = useState<BackgroundJobResponse[]>([]);
  const [filterTenantId, setFilterTenantId] = useState<string | undefined>(undefined);
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [tenantOptions, setTenantOptions] = useState<{ label: string; value: string }[]>([]);
  const [tenantsLoading, setTenantsLoading] = useState(false);

  const loadTenants = useCallback(async () => {
    setTenantsLoading(true);
    const response = await searchTenants({ filters: [], pageIndex: 1, pageSize: 500 });
    setTenantsLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load tenants");
      return;
    }
    setTenantOptions(
      (response.data.data.data || []).map((t: TenantReponse) => ({ label: t.name, value: t.id! }))
    );
  }, []);

  useEffect(() => {
    loadTenants();
  }, [loadTenants]);

  const fetchBackgroundJobs = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (filterTenantId) filters.push({ fieldName: "TenantId", value: filterTenantId });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchBackgroundJobs(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<BackgroundJobResponse>;
          setBackgroundJobs(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch background jobs");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch background jobs");
      } finally {
        setLoading(false);
      }
    },
    [filterTenantId, sortField, sortDirection]
  );

  useEffect(() => {
    fetchBackgroundJobs(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchBackgroundJobs]);

  const handleSortChange = (field: string) => {
    if (sortField === field) {
      setSortDirection(!sortDirection);
    } else {
      setSortField(field);
      setSortDirection(true);
    }
  };

  const handleTableChange = (newPagination: { current?: number; pageSize?: number }) => {
    const newPage = newPagination.current || currentPage;
    const newPageSize = newPagination.pageSize || pageSize;
    setCurrentPage(newPage);
    setPageSize(newPageSize);
    fetchBackgroundJobs(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("RunDate")}>
          Run Date {renderSortIcon("RunDate")}
        </div>
      ),
      dataIndex: "runDate",
      key: "runDate",
      render: (_: unknown, r: BackgroundJobResponse) => dayjs(r.runDate).format("YYYY-MM-DD HH:mm"),
    },
    {
      title: "Tenant",
      dataIndex: "tenantName",
      key: "tenantName",
      render: (_: unknown, r: BackgroundJobResponse) => r.tenantName || "-",
    },
    {
      title: "Result",
      dataIndex: "result",
      key: "result",
      render: (_: unknown, r: BackgroundJobResponse) => <Tag color={resultColor(r.result)}>{r.result || "-"}</Tag>,
    },
    {
      title: "Detail",
      dataIndex: "logDetail",
      key: "logDetail",
      render: (_: unknown, r: BackgroundJobResponse) => r.logDetail || "-",
    },
  ];

  return (
    <MainLayout>
      <div className="background-job-page">
        <div className="background-job-header">
          <h2>Subscription Billing Job History</h2>
        </div>

        <div className="background-job-search">
          <div className="search-container">
            <Select
              options={tenantOptions}
              loading={tenantsLoading}
              placeholder="Filter by tenant"
              allowClear
              showSearch
              optionFilterProp="label"
              style={{ width: 260 }}
              value={filterTenantId}
              onChange={(value) => {
                setFilterTenantId(value);
                fetchBackgroundJobs(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="background-job-table"
          columns={columns}
          dataSource={backgroundJobs}
          rowKey={(record) => record.id || `${record.tenantId}-${record.runDate}`}
          pagination={{
            current: currentPage,
            pageSize,
            total,
            showSizeChanger: true,
            showTotal: (t) => `Total ${t} items`,
          }}
          loading={loading}
          onChange={(newPagination) =>
            handleTableChange({
              current: newPagination.current || 1,
              pageSize: newPagination.pageSize || 10,
            })
          }
        />
      </div>
    </MainLayout>
  );
};

export default BackgroundJobs;
