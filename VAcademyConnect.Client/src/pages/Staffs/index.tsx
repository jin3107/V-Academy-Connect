import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import { 
  Button, Input, Table, message, Select,
  Modal, Form, DatePicker, Popconfirm, Descriptions, Spin,
} from "antd";
import {
  LockOutlined, MailOutlined, PhoneOutlined, PlusOutlined, SearchOutlined,
  SortAscendingOutlined, SortDescendingOutlined, UserOutlined,
} from "@ant-design/icons";
import {
  createStaff, searchStaffs, getStaffById,
  updateStaff, deleteStaff,
} from "../../apis/staffAPI";
import type {
  Filter, RoleResponse, SearchRequest, SearchResponse,
  CreateStaffRequest, UpdateStaffRequest, StaffResponse,
} from "../../interfaces";
import "./style.scss";
import { getAllRoles } from "../../apis/roleAPI";
import { Typography } from "antd";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";
import { useAuth } from "../../contexts";
import { TenantType } from "../../commons";

const { Option } = Select;
const { Text } = Typography;

type Mode = "create" | "detail" | "update";

interface StaffData {
  id?: string;
  name: string;
  email: string;
  phoneNumber: string;
  role: string;
}

type BackendStaffResponse = StaffResponse & { createBy?: string };

const Staffs = () => {
  const { user } = useAuth();
  const isCenterTenant = user?.tenantType === TenantType.Center;

  // ===== Table state =====
  const [loading, setLoading] = useState(false);
  const [staffs, setStaffs] = useState<StaffData[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  // ===== Modal / form state =====
  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [form] = Form.useForm<StaffResponse>(); // Form dùng StaffResponse

  // Detail data (hiển thị đẹp ở chế độ detail)
  const [detailData, setDetailData] = useState<StaffResponse | null>(null);

  // ===== Roles =====
  const [roleOptions, setRoleOptions] = useState<{ label: string; value: string }[]>([]);
  const [roleLoading, setRoleLoading] = useState(false);

  const genders = [
    { label: "Male", value: 0 },
    { label: "Female", value: 1 },
  ];

  // ===== Helpers =====
  const toDayjs = (v: string | Dayjs | undefined): Dayjs | undefined => {
    if (!v) return undefined;
    return typeof v === "string" ? dayjs(v) : v;
  };

  const fmtDate = (v?: string | Dayjs, withTime = false) => {
    if (!v) return "-";
    const d = typeof v === "string" ? dayjs(v) : v;
    return d ? d.format(withTime ? "YYYY-MM-DD HH:mm" : "YYYY-MM-DD") : "-";
  };

  const fmtGender = (g?: number) => {
    if (g === 0) return "Male";
    if (g === 1) return "Female";
    return "-";
  };

  // ===== Openers =====
  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
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

  // ===== Loaders =====
  const loadRoles = async () => {
    setRoleLoading(true);
    const response = await getAllRoles();
    setRoleLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load roles");
      return;
    }
    const roles = (response.data || []).filter((r: RoleResponse) => isCenterTenant || r.name !== "Teacher");
    setRoleOptions(roles.map((r: RoleResponse) => ({ label: r.name, value: r.name })));
  };

  const loadDetail = async (id: string) => {
    setDetailLoading(true);
    const response = await getStaffById(id);
    setDetailLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load staff");
      return;
    }

    // Map CreateBy -> createdBy, convert date fields
    const raw = response.data as BackendStaffResponse;
    const mapped: StaffResponse = {
      ...raw,
      createdBy: raw.createdBy ?? raw.createBy, 
      dateOfBirth: toDayjs(raw.dateOfBirth),
      createdOn: toDayjs(raw.createdOn),
    };
    setDetailData(mapped);

    // nếu là Update thì fill vào Form
    if (mode === "update") {
      form.setFieldsValue({
        id: mapped.id,
        name: mapped.name,
        email: mapped.email,
        phoneNumber: mapped.phoneNumber,
        role: mapped.role,
        gender: mapped.gender,
        dateOfBirth: mapped.dateOfBirth,
        address: mapped.address,
        createdOn: mapped.createdOn,
        createdBy: mapped.createdBy,
      } as Partial<StaffResponse>);
    }
  };

  // ===== Submit (Create / Update) =====
  const onFinish = async (values: StaffResponse) => {
    try {
      setSubmitting(true);
      const common = {
        name: values.name,
        email: values.email,
        phoneNumber: values.phoneNumber,
        role: values.role,
        gender: typeof values.gender === "number" ? values.gender : Number(values.gender ?? 0),
        dateOfBirth: values.dateOfBirth ? dayjs(values.dateOfBirth).toISOString() : null,
        address: values.address ?? "",
      };

      if (mode === "create") {
        const payloadCreate: CreateStaffRequest = {
          ...common, 
          password: values.password ?? ""
        } as unknown as CreateStaffRequest;
        const res = await createStaff(payloadCreate);
        if (!res.isSuccess) {
          message.error(res.message || "Create failed");
        } else {
          message.success(res.message || "Created");
          setOpen(false);
          fetchStaffs(currentPage, pageSize, searchText);
        }
      } else if (mode === "update") {
        if (!selectedId) 
          return message.error('Missing staff id');
        
        const payloadUpdate = {
          ...common,
          password: values.password ?? "",
          id: selectedId,
        } as unknown as UpdateStaffRequest;
        const res = await updateStaff(payloadUpdate);
        if (!res.isSuccess) {
          message.error(res.message || "Update failed");
        } else {
          message.success(res.message || "Updated");
          setOpen(false);
          fetchStaffs(currentPage, pageSize, searchText);
        }
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  // ===== Table data loader =====
  const fetchStaffs = useCallback(
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
          sortBy: sortField
            ? {
                fieldName: sortField,
                ascending: sortDirection,
              }
            : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchStaffs(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<StaffData>;
          let staffData: StaffData[] = [];
          if (searchResponse.data && searchResponse.data.data) {
            staffData = searchResponse.data.data.map((item: StaffData | { data: StaffData }) =>
              "data" in item ? item.data : item
            );
          }
          setStaffs(staffData);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch staffs");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch staffs");
      } finally {
        setLoading(false);
      }
    },
    [searchField, sortField, sortDirection]
  );

  useEffect(() => {
    fetchStaffs(currentPage, pageSize, searchText);
  }, [fetchStaffs, currentPage, pageSize, searchText]);

  // ===== Handlers =====
  const handleSearch = () => {
    fetchStaffs(1, pageSize, searchText);
  };

  const handleSortChange = (field: string) => {
    if (sortField === field) {
      setSortDirection(!sortDirection);
    } else {
      setSortField(field);
      setSortDirection(true);
    }
    fetchStaffs(currentPage, pageSize, searchText);
  };

  const handleTableChange = (newPagination: { current?: number; pageSize?: number }) => {
    const newPage = newPagination.current || currentPage;
    const newPageSize = newPagination.pageSize || pageSize;
    setCurrentPage(newPage);
    setPageSize(newPageSize);
    fetchStaffs(newPage, newPageSize, searchText);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  // ===== Columns =====
  const columns = [
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Name")}>
          Name {renderSortIcon("Name")}
        </div>
      ),
      dataIndex: "name",
      key: "name",
      width: 120,
      ellipsis: true,
      render: (_: unknown, r: StaffData) => (
        <Text ellipsis={{ tooltip: r.name }}>{r.name || "-"}</Text>
      ),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Email")}>
          Email {renderSortIcon("Email")}
        </div>
      ),
      dataIndex: "email",
      key: "email",
      width: 110,
      ellipsis: true,
      render: (_: unknown, r: StaffData) => (
        <Text ellipsis={{ tooltip: r.email }}>{r.email || "-"}</Text>
      ),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("PhoneNumber")}>
          Phone Number {renderSortIcon("PhoneNumber")}
        </div>
      ),
      dataIndex: "phoneNumber",
      key: "phoneNumber",
      width: 90,
      ellipsis: true,
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Role")}>
          Role {renderSortIcon("Role")}
        </div>
      ),
      dataIndex: "role",
      key: "role",
      width: 80,
      ellipsis: true,
    },
    {
      title: "Actions",
      key: "actions",
      width: 260,
      fixed: "right" as const,
      render: (_: unknown, record: StaffData) => (
        <div className="action-buttons" style={{ whiteSpace: "nowrap" }}>
          <Button type="link" onClick={() => openDetail(record.id)}>
            Detail
          </Button>
          <Button type="link" onClick={() => openUpdate(record.id)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this staff?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteStaff(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchStaffs(currentPage, pageSize, searchText);
            }}
          >
            <Button type="link" danger>
              Delete
            </Button>
          </Popconfirm>
        </div>
      ),
    },
  ];

  // ===== Render =====
  return (
    <MainLayout>
      <div className="staff-page">
        <div className="staff-header">
          <h2>Staff Management</h2>
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
            Add Staff
          </Button>
        </div>

        <div className="staff-search">
          <div className="search-container">
            <Select
              defaultValue="Name"
              style={{ width: 120 }}
              onChange={(value) => setSearchField(value)}
              value={searchField}
            >
              <Option value="Name">Name</Option>
              <Option value="Email">Email</Option>
              <Option value="PhoneNumber">Phone</Option>
              <Option value="Role">Role</Option>
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
          className="staff-table"
          columns={columns}
          dataSource={staffs}
          rowKey={(record) => record.id || record.email}
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
          title={
            mode === "create"
              ? "Create Staff"
              : mode === "update"
              ? "Update Staff"
              : "Staff Detail"
          }
          open={open}
          onCancel={() => setOpen(false)}
          footer={
            mode === "detail"
              ? [<Button key="close" onClick={() => setOpen(false)}>Close</Button>]
              : [
                  <Button key="cancel" onClick={() => setOpen(false)}>Cancel</Button>,
                  <Button key="submit" type="primary" loading={submitting} onClick={() => form.submit()}>
                    {mode === "create" ? "Submit" : "Save"}
                  </Button>,
                ]
          }
          destroyOnHidden
          afterOpenChange={async (visible) => {
            if (!visible) return;
            if (mode !== "detail") {
              form.resetFields();
              await loadRoles();
            }

            // Load detail cho detail/update
            if ((mode === "detail" || mode === "update") && selectedId) {
              await loadDetail(selectedId);
            }
          }}
        >
          {mode === "detail" ? (
            <Spin spinning={detailLoading}>
              <Descriptions column={1} bordered size="middle">
                <Descriptions.Item label="Name">{detailData?.name ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Email">{detailData?.email ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Phone Number">{detailData?.phoneNumber ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Role">{detailData?.role ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Password">{detailData?.password ?? "-"}</Descriptions.Item>

                {/* Chỉ hiển thị Gender nếu có */}
                {typeof detailData?.gender !== "undefined" && (
                  <Descriptions.Item label="Gender">
                    {fmtGender(detailData?.gender as number)}
                  </Descriptions.Item>
                )}

                <Descriptions.Item label="Date of Birth">
                  {fmtDate(detailData?.dateOfBirth as Dayjs | string)}
                </Descriptions.Item>
                <Descriptions.Item label="Address">{detailData?.address ?? "-"}</Descriptions.Item>
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
                <Input prefix={<span className="input-prefix"><UserOutlined /></span>} />
              </Form.Item>

              <Form.Item 
                name="email" 
                label="Email" 
                rules={[{ required: true, message: "Emailame is required" }, { type: "email" }]}
              >
                <Input prefix={<span className="input-prefix"><MailOutlined /></span>} />
              </Form.Item>

              <Form.Item 
                name="phoneNumber" 
                label="Phone Number" 
                rules={[{ required: true, message: "Phone Number is required" }]}
              >
                <Input prefix={<span className="input-prefix"><PhoneOutlined /></span>} />
              </Form.Item>

              {(mode === "create" || mode === "update") && (
                <Form.Item 
                  name="password" 
                  label="Password" 
                  rules={[{ required: true, min: 6, message: "Password is required" }]}>
                  <Input.Password prefix={<span className="input-prefix"><LockOutlined /></span>} />
                </Form.Item>
              )}

              <Form.Item 
                name="role" 
                label="Role" 
                rules={[{ required: true, message: "Role is required" }]}
              >
                <Select
                  options={roleOptions}
                  loading={roleLoading}
                  placeholder="Select role"
                  showSearch
                  optionFilterProp="label"
                />
              </Form.Item>

              <Form.Item 
                name="gender" 
                label="Gender" 
                rules={[{ required: mode === "create", message: "Gender is required" }]}
              >
                <Select options={genders} />
              </Form.Item>

              <Form.Item 
                name="dateOfBirth" 
                label="Date of Birth" 
                rules={[{ required: true, message: "Date Of Birth is required" }]}
              >
                <DatePicker style={{ width: "100%" }} />
              </Form.Item>

              <Form.Item 
                name="address" 
                label="Address" 
                rules={[{ required: true, message: "Address is required" }]}
              >
                <Input.TextArea rows={2} />
              </Form.Item>
            </Form>
          )}
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Staffs;
