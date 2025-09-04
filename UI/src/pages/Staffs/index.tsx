import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import { Button, Input, Table, message, Select, Modal, Form, DatePicker } from "antd";
import {
  LockOutlined,
  MailOutlined,
  PhoneOutlined,
  PlusOutlined,
  SearchOutlined,
  SortAscendingOutlined,
  SortDescendingOutlined,
  UserOutlined,
} from "@ant-design/icons";
import { createStaff, searchStaffs } from "../../apis/staffAPI";
import type {
  Filter,
  RoleResponse,
  SearchRequest,
  SearchResponse,
  StaffRequest,
} from "../../interfaces";
import "./style.scss";


const { Option } = Select;
import { Typography } from "antd";
import { getAllRoles } from "../../apis/roleAPI";
const { Text } = Typography;

interface StaffData {
  id?: string;
  name: string;
  email: string;
  phoneNumber: string;
  role: string;
}

const Staffs = () => {
  const [loading, setLoading] = useState(false);
  const [staffs, setStaffs] = useState<StaffData[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [form] = Form.useForm<StaffRequest>();

  const [roleOptions, setRoleOptions] = useState<{ label: string; value: string }[]>([]);
  const [roleLoading, setRoleLoading] = useState(false);

  const genders = [
    { label: "Male", value: 0 },
    { label: "Female", value: 1 },
  ];

  const openCreate = () => setOpen(true);

  const loadRoles = async () => {
    setRoleLoading(true);
    const response = await getAllRoles();
    setRoleLoading(false);
    if (!response.isSuccess) return message.error(response.message || "Failed to load roles");
    setRoleOptions(
      (response.data || []).map((r: RoleResponse) => ({ label: r.name, value: r.name }))
    );
  };

  const onCreate = async (values: StaffRequest) => {
    try {
      setSubmitting(true);
      const payload: StaffRequest = {
        ...values,
        dateOfBirth: values.dateOfBirth.toISOString(), // Dayjs -> ISO string
        gender: Number(values.gender),
      } as unknown as StaffRequest; // đảm bảo khớp kiểu FE -> BE

      const response = await createStaff(payload);
      if (!response.isSuccess) {
        message.error(response.message || "Create failed");
        return;
      }
      message.success(response.message || "Created");
      setOpen(false);
      fetchStaffs(currentPage, pageSize, searchText);
    } catch (err) {
      console.error("Create failed: ", err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const fetchStaffs = useCallback(
    async (page = 1, pageSize = 10, searchValue = "") => {
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
          pageSize,
        };

        const response = await searchStaffs(searchRequest);

        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<StaffData>;
          let staffData: StaffData[] = [];
          if (searchResponse.data && searchResponse.data.data) {
            staffData = searchResponse.data.data.map(
              (item: StaffData | { data: StaffData }) => {
                if ("data" in item) return item.data;
                return item;
              }
            );
          }

          setStaffs(staffData);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          console.error("API Error:", response.message);
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

  const renderSortIcon = (field: string) => {
    if (sortField !== field) return null;
    return sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />;
  };

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
      width: 100,
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
      width: 70,
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
      width: 50,
      ellipsis: true,
    },
    {
      title: "Actions",
      key: "actions",
      width: 150,
      fixed: "right" as const,
      render: (_: unknown, record: StaffData) => (
        <div className="action-buttons" style={{ whiteSpace: "nowrap" }}>
          <Button type="link" onClick={() => console.log("Detail", record)}>
            Detail
          </Button>
          <Button type="link" onClick={() => console.log("Edit", record)}>
            Edit
          </Button>
          <Button type="link" danger onClick={() => console.log("Delete", record)}>
            Delete
          </Button>
        </div>
      ),
    },
  ];

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
          title="Create Staff"
          open={open}
          onCancel={() => setOpen(false)}
          footer={[
            <Button key="cancel" onClick={() => setOpen(false)}>
              Cancel
            </Button>,
            <Button
              key="submit"
              type="primary"
              loading={submitting}
              onClick={() => form.submit()}
            >
              Submit
            </Button>,
          ]}
          destroyOnHidden
          afterOpenChange={(visible) => {
            if (visible) {
              form.resetFields(); // Form đã mount
              loadRoles();
            }
          }}
        >
          <Form form={form} layout="vertical" onFinish={onCreate} preserve={false}>
            <Form.Item name="name" label="Name" rules={[{ required: true }]}>
              <Input prefix={<span className="input-prefix"><UserOutlined /></span>} />
            </Form.Item>

            <Form.Item name="email" label="Email" rules={[{ required: true }, { type: "email" }]}>
              <Input prefix={<span className="input-prefix"><MailOutlined /></span>} />
            </Form.Item>

            <Form.Item name="phoneNumber" label="Phone Number" rules={[{ required: true }]}>
              <Input prefix={<span className="input-prefix"><PhoneOutlined /></span>} />
            </Form.Item>

            <Form.Item name="password" label="Password" rules={[{ required: true, min: 6 }]}>
              <Input.Password prefix={<span className="input-prefix"><LockOutlined /></span>} />
            </Form.Item>

            <Form.Item name="role" label="Role" rules={[{ required: true }]}>
              <Select
                options={roleOptions}
                loading={roleLoading}
                placeholder="Select role"
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Form.Item name="gender" label="Gender" rules={[{ required: true }]}>
              <Select options={genders} />
            </Form.Item>

            <Form.Item name="dateOfBirth" label="Date of Birth" rules={[{ required: true }]}>
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>

            <Form.Item name="address" label="Address" rules={[{ required: true }]}>
              <Input.TextArea rows={2} />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Staffs;
