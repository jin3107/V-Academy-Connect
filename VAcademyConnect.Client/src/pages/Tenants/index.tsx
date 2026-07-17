import MainLayout from '../../components/MainLayout';
import './style.scss'
import { 
  Button, Input, Table, message, Select, 
  Modal, Form, Descriptions, Spin, Tag 
} from "antd";
import { 
  SearchOutlined, SortAscendingOutlined, 
  SortDescendingOutlined, ReloadOutlined, 
  ClusterOutlined,
  MailOutlined,
  PhoneOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  ExclamationCircleOutlined
} from "@ant-design/icons";
import type { Filter, SearchRequest, SearchResponse, 
  TenantRequest, TenantReponse
} from "../../interfaces";
import { Status } from "../../commons/enums";
import { Typography } from "antd";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";
import { useState, useCallback, useEffect } from 'react';
import { getTenantById, searchTenants, updateTenant, renewStandardSubscription } from '../../apis/tenantAPI';

const { Option } = Select;
const { Text } = Typography;

type Mode = "detail" | "update";

interface TenantData {
  id?: string;
  name: string;
  contactEmail: string;
  contactPhone: string;
  status: string | number;
  statusName?: string;
  subscriptionStatus?: {
    hasActiveSubscription: boolean;
    isExpiringSoon: boolean;
    isExpired: boolean;
    expirationDate?: string | Date;
    subscriptionPlanName: string;
    status: string;
  };
}

type BackendTenantResponse = TenantReponse & { 
  createBy?: string;
  subscriptionStatus?: {
    hasActiveSubscription: boolean;
    isExpiringSoon: boolean;
    isExpired: boolean;
    expirationDate?: string | Date;
    subscriptionPlanName: string;
    status: string;
  };
};

const Tenants = () => {
  const [loading, setLoading] = useState(false);
  const [tenants, setTenants] = useState<TenantData[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("detail");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [form] = Form.useForm<TenantReponse>();
  const [detailData, setDetailData] = useState<TenantReponse | null>(null);

  const statusOptions = [
  {
    label: (
      <>
        <CheckCircleOutlined style={{ color: "#52c41a", marginRight: 6 }} />
        Active
      </>
    ),
    value: Status.Active,
    },
    {
      label: (
        <>
          <CloseCircleOutlined style={{ color: "#f5222d", marginRight: 6 }} />
          Suspended
        </>
      ),
      value: Status.Suspended,
    },
    {
      label: (
        <>
          <ExclamationCircleOutlined style={{ color: "#faad14", marginRight: 6 }} />
          Inactive
        </>
      ),
      value: Status.Inactive,
    },
  ];

  const toDayjs = (v: string | Dayjs | Date | undefined): Dayjs | undefined => {
    if (!v) return undefined;
    return dayjs(v);
  };

  const fmtDate = (v?: string | Dayjs | Date, withTime = false) => {
    if (!v) return "-";
    const d = dayjs(v);
    return d.isValid() ? d.format(withTime ? "YYYY-MM-DD HH:mm" : "YYYY-MM-DD") : "-";
  };

  const fmtStatus = (statusValue?: Status | number) => {
    if (statusValue === Status.Active || statusValue === 0) return "Active";
    if (statusValue === Status.Suspended || statusValue === 1) return "Suspended";
    if (statusValue === Status.Inactive || statusValue === 2) return "Inactive";
    return "-";
  };

  const getStatusColor = (statusValue?: Status | number) => {
    if (statusValue === Status.Active || statusValue === 0) return "success";
    if (statusValue === Status.Suspended || statusValue === 1) return "error";
    if (statusValue === Status.Inactive || statusValue === 2) return "warning";
    return "default";
  };

  const getStatusIcon = (statusValue?: Status | number) => {
    if (statusValue === Status.Active || statusValue === 0) return <CheckCircleOutlined />;
    if (statusValue === Status.Suspended || statusValue === 1) return <CloseCircleOutlined />;
    if (statusValue === Status.Inactive || statusValue === 2) return <ExclamationCircleOutlined />;
    return undefined;
  };

  const openDetail = (id?: string) => {
    if (!id) return;
    setMode("detail");
    setSelectedId(id);
    setOpen(true);
  };

  const openUpdate = (id?: string) => {
    if (!id) return;
    setMode("update");
    setSelectedId(id);
    setOpen(true);
  };

  const loadDetail = async (id: string) => {
    setDetailLoading(true);
    const response = await getTenantById(id);
    setDetailLoading(false);

    if (!response.isSuccess) {
      message.error(response.message || "Failed to load tenant");
      return;
    }

    const raw = response.data as BackendTenantResponse;
    const subscription = raw.subscriptionStatus || raw.tenantSubscriptionResponse;
    const mapped: TenantReponse = {
      ...raw,
      tenantSubscriptionResponse: subscription,
      subscriptionStatus: subscription,
      createdBy: raw.createdBy ?? raw.createBy,
      createdOn: toDayjs(raw.createdOn),
    };

    setDetailData(mapped);

    if (mode === "update") {
      form.setFieldsValue({
        id: mapped.id,
        name: mapped.name,
        contactEmail: mapped.contactEmail,
        contactPhone: mapped.contactPhone,
        status: mapped.status,
        createdOn: mapped.createdOn,
        createdBy: mapped.createdBy,
      } as Partial<TenantReponse>);
    }
  };

  const onFinish = async (values: TenantReponse) => {
    try {
      setSubmitting(true);
      const common = {
        name: values.name,
        contactEmail: values.contactEmail,
        contactPhone: values.contactPhone,
        status: values.status,
      };

      if (mode === "update") {
        if (!selectedId) return message.error('Missing tenant id');
        const payloadUpdate = {
          ...common,
          id: selectedId,
        } as unknown as TenantRequest;

        const response = await updateTenant(payloadUpdate);
        if (!response.isSuccess) {
          message.error(response.message || 'Update failed');
        } else {
          message.success(response.message || 'Updated');
          setOpen(false);
          fetchTenants(currentPage, pageSize, searchText);
        }
      }
    } catch (err) {
      console.log(err);
      message.error('Unexpected error');
    } finally {
      setSubmitting(false);
    }
  };

  const handleRenewSubscription = async (tenantId: string) => {
    try {
      const response = await renewStandardSubscription(tenantId);
      if (!response.isSuccess) {
        message.error(response.message || 'Renew subscription failed');
      } else {
        message.success(response.message || 'Subscription renewed successfully');
        fetchTenants(currentPage, pageSize, searchText);
      }
    } catch (err) {
      console.log(err);
      message.error('Unexpected error');
    }
  };

  const fetchTenants = useCallback(
    async (page = 1, pageSizeArg = 10, searchValue = "") => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (searchValue) {
          filters.push({
            fieldName: searchField,
            value: searchValue,
            operation: "Contains",
          });
        }

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? {
            fieldName: sortField,
            ascending: sortDirection,
          } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchTenants(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<TenantData>;
          let tenantData: TenantData[] = [];
          if (searchResponse.data && searchResponse.data.data) {
            tenantData = searchResponse.data.data.map((item: TenantData | { data: TenantData }) => {
              const tenant = "data" in item ? item.data : item;
              return tenant;
            });
          }

          setTenants(tenantData);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch tenants");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch tenants");
      } finally {
        setLoading(false);
      }
    },
    [searchField, sortField, sortDirection]
  );

  useEffect(() => {
    fetchTenants(currentPage, pageSize, searchText);
  }, [fetchTenants, currentPage, pageSize, searchText])

  const handleSearch = () => {
    fetchTenants(1, pageSize, searchText);
  };

  const handleSortChange = (field: string) => {
    if (sortField === field) {
      setSortDirection(!sortDirection);
    } else {
      setSortField(field);
      setSortDirection(true);
    }
    fetchTenants(currentPage, pageSize, searchText);
  };

  const handleTableChange = (newPagination: { current?: number; pageSize?: number }) => {
    const newPage = newPagination.current || currentPage;
    const newPageSize = newPagination.pageSize || pageSize;
    setCurrentPage(newPage);
    setPageSize(newPageSize);
    fetchTenants(newPage, newPageSize, searchText);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;
    const columns = [
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Name")}>
          Name {renderSortIcon("Name")}
        </div>
      ),
      dataIndex: "name",
      key: "name",
      width: 150,
      ellipsis: true,
      render: (_: unknown, r: TenantData) => (
        <Text ellipsis={{ tooltip: r.name }}>{r.name || "-"}</Text>
      ),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("ContactEmail")}>
          Contact Email {renderSortIcon("ContactEmail")}
        </div>
      ),
      dataIndex: "contactEmail",
      key: "contactEmail",
      width: 180,
      ellipsis: true,
      render: (_: unknown, r: TenantData) => (
        <Text ellipsis={{ tooltip: r.contactEmail }}>{r.contactEmail || "-"}</Text>
      ),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("ContactPhone")}>
          Contact Phone {renderSortIcon("ContactPhone")}
        </div>
      ),
      dataIndex: "contactPhone",
      key: "contactPhone",
      width: 130,
      ellipsis: true,
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Status")}>
          Status {renderSortIcon("Status")}
        </div>
      ),
      dataIndex: "status",
      key: "status",
      width: 100,
      ellipsis: true,
      render: (_: unknown, record: TenantData) => {
        const statusText = record.statusName || fmtStatus(Number(record.status));
        return (
          <Tag icon={getStatusIcon(Number(record.status))} color={getStatusColor(Number(record.status))}>
            {statusText || "-"}
          </Tag>
        );
      },
    },
    {
      title: "Expiry Date",
      dataIndex: ["subscriptionStatus", "expirationDate"],
      key: "expirationDate",
      width: 120,
      ellipsis: true,
      render: (v: string | Date | null | undefined) => fmtDate(v ?? undefined),
    },
    {
      title: "Actions",
      key: "actions",
      width: 300,
      fixed: "right" as const,
      render: (_: unknown, record: TenantData) => (
        <div className="action-buttons" style={{ whiteSpace: "nowrap" }}>
          <Button type="link" onClick={() => openDetail(record.id)}>
            Detail
          </Button>
          <Button type="link" onClick={() => openUpdate(record.id)}>
            Edit
          </Button>
          <Button 
            type="link" 
            icon={<ReloadOutlined />}
            onClick={() => {
              if (record.id) {
                Modal.confirm({
                  title: 'Renew Subscription',
                  content: 'Are you sure you want to renew the subscription for this tenant?',
                  okText: 'Yes',
                  cancelText: 'No',
                  onOk: () => handleRenewSubscription(record.id!),
                });
              }
            }}
          >
            Renew
          </Button>
        </div>
      ),
    },
  ];

  return (
    <MainLayout>
      <div className='tenant-page'>
        <div className='tenant-header'>
          <h2>Tenant Management</h2>
        </div>

        <div className="tenant-search">
          <div className="search-container">
            <Select
              defaultValue="Name"
              style={{ width: 120 }}
              onChange={(value) => setSearchField(value)}
              value={searchField}
            >
              <Option value="Name">Name</Option>
              <Option value="ContactEmail">Email</Option>
              <Option value="ContactPhone">Phone</Option>
            </Select>
            <Input
              placeholder={`Search by ${searchField.toLowerCase()}`}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              onPressEnter={handleSearch}
              suffix={<SearchOutlined onClick={handleSearch} />}
              style={{ width: 300 }}
            />
            <Button type="primary" onClick={handleSearch}>
              Search
            </Button>
          </div>
        </div>

        <Table
          className="tenant-table"
          columns={columns}
          dataSource={tenants}
          rowKey={(record) => record.id || record.name}
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

        <Modal
          title={mode === "update" ? "Update Tenant" : "Tenant Detail"}
          open={open}
          onCancel={() => setOpen(false)}
          footer={
            mode === "detail"
              ? [<Button key="close" onClick={() => setOpen(false)}>Close</Button>]
              : [
                  <Button key="cancel" onClick={() => setOpen(false)}>Cancel</Button>,
                  <Button key="submit" type="primary" loading={submitting} onClick={() => form.submit()}>
                    Save
                  </Button>,
                ]
          }
          destroyOnHidden
          afterOpenChange={async (visible) => {
            if (!visible) return;

            if (mode !== "detail") {
              form.resetFields();
            }

            if ((mode === "detail" || mode === "update") && selectedId) {
              await loadDetail(selectedId);
            }
          }}
        >
          {mode === "detail" ? (
            <Spin spinning={detailLoading}>
              <Descriptions column={1} bordered size="middle">
                <Descriptions.Item label="Name">{detailData?.name ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Contact Email">{detailData?.contactEmail ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Contact Phone">{detailData?.contactPhone ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Status">
                  <Tag icon={getStatusIcon(detailData?.status)} color={getStatusColor(detailData?.status)}>
                    {detailData?.statusName || fmtStatus(detailData?.status)}
                  </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Has Active Subscription">
                  {detailData?.tenantSubscriptionResponse?.hasActiveSubscription ? "Yes" : "No"}
                </Descriptions.Item>
                <Descriptions.Item label="Is Expiring Soon">
                  {detailData?.tenantSubscriptionResponse?.isExpiringSoon ? "Yes" : "No"}
                </Descriptions.Item>
                <Descriptions.Item label="Is Expired">
                  {detailData?.tenantSubscriptionResponse?.isExpired ? "Yes" : "No"}
                </Descriptions.Item>
                <Descriptions.Item label="Expiration Date">
                  {fmtDate(detailData?.tenantSubscriptionResponse?.expirationDate)}
                </Descriptions.Item>
                <Descriptions.Item label="Created On">
                  {fmtDate(detailData?.createdOn as Dayjs | string, true)}
                </Descriptions.Item>
                <Descriptions.Item label="Created By">
                  {detailData?.createdBy ?? "-"}
                </Descriptions.Item>
              </Descriptions>
            </Spin>
          ) : (
            <Form form={form} layout="vertical" onFinish={onFinish} preserve={false}>
              <Form.Item
                name="name"
                label="Name"
                rules={[{ required: true, message: "Name is required" }]}
              >
                <Input prefix={<span className="input-prefix"><ClusterOutlined /></span>} />
              </Form.Item>

              <Form.Item
                name="contactEmail"
                label="Contact Email"
                rules={[
                  { required: true, message: "Contact Email is required" },
                  { type: "email", message: "Please enter a valid email" }
                ]}
              >
                <Input prefix={<span className="input-prefix"><MailOutlined /></span>} />
              </Form.Item>

              <Form.Item
                name="contactPhone"
                label="Contact Phone"
                rules={[{ required: true, message: "Contact Phone is required" }]}
              >
                <Input prefix={<span className="input-prefix"><PhoneOutlined /></span>} />
              </Form.Item>

              <Form.Item
                name="status"
                label="Status"
                rules={[{ required: true, message: "Status is required" }]}
              >
                <Select options={statusOptions} />
              </Form.Item>
            </Form>
          )}
        </Modal>
      </div>
    </MainLayout>
  )
}

export default Tenants;