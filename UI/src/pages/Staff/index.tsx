import { useEffect, useState } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button,
  Input,
  Table,
  message,
  Pagination,
  Select,
} from "antd";
import {
  PlusOutlined,
  SearchOutlined,
  SortAscendingOutlined,
  SortDescendingOutlined,
} from "@ant-design/icons";
import { searchStaffs } from "../../apis/staffAPI";
import type {
  Filter,
  SearchRequest,
  SearchResponse,
  StaffResponse,
} from "../../interfaces";
import "./style.scss";

const { Option } = Select;

const Staff = () => {
  const [loading, setLoading] = useState(false);
  const [staffs, setStaffs] = useState<StaffResponse[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  const fetchStaffs = async (page = 1, pageSize = 10, searchValue = "") => {
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
        filters: filters,
        sortBy: sortField
          ? {
              fieldName: sortField,
              ascending: sortDirection,
            }
          : undefined,
        pageIndex: page,
        pageSize: pageSize,
      };

      const response = await searchStaffs(searchRequest);
      if (response.isSuccess) {
        const searchResponse = response as SearchResponse<StaffResponse>;
        // Kiểm tra và lọc các phần tử không hợp lệ
        const validStaffs = searchResponse.data.data.filter(
          (staff) => staff && staff.data
        );
        setStaffs(validStaffs);
        setPagination({
          current: searchResponse.data.currentPage,
          pageSize: searchResponse.data.rowsPerPage,
          total: searchResponse.data.totalRows,
        });
      } else {
        message.error(response.message || "Failed to fetch staffs");
      }
    } catch (error) {
      console.error("Error fetching staffs:", error);
      message.error("Failed to fetch staffs");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStaffs();
  }, []);

  const handleSearch = () => {
    fetchStaffs(1, pagination.pageSize, searchText);
  };

  const handleSortChange = (field: string) => {
    if (sortField === field) {
      setSortDirection(!sortDirection);
    } else {
      setSortField(field);
      setSortDirection(true);
    }
    fetchStaffs(pagination.current, pagination.pageSize, searchText);
  };

  const handleTableChange = (pagination: {
    current: number;
    pageSize: number;
  }) => {
    fetchStaffs(pagination.current, pagination.pageSize, searchText);
  };

  const renderSortIcon = (field: string) => {
    if (sortField !== field) return null;
    return sortDirection ? (
      <SortAscendingOutlined />
    ) : (
      <SortDescendingOutlined />
    );
  };

  const columns = [
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Name")}>
          Name {renderSortIcon("Name")}
        </div>
      ),
      dataIndex: ["data", "name"],
      key: "name",
      render: (record: StaffResponse) => record.data?.name || "-",
    },
    {
      title: (
        <div
          className="column-header"
          onClick={() => handleSortChange("Email")}
        >
          Email {renderSortIcon("Email")}
        </div>
      ),
      dataIndex: ["data", "email"],
      key: "email",
      render: (record: StaffResponse) =>
        record.data?.email || "-",
    },
    {
      title: (
        <div
          className="column-header"
          onClick={() => handleSortChange("PhoneNumber")}
        >
          Phone Number {renderSortIcon("PhoneNumber")}
        </div>
      ),
      dataIndex: ["data", "phoneNumber"],
      key: "phoneNumber",
      render: (record: StaffResponse) =>
        record.data?.phoneNumber || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Role")}>
          Role {renderSortIcon("Role")}
        </div>
      ),
      dataIndex: ["data", "role"],
      key: "role",
      render: (record: StaffResponse) => record.data?.role || "-",
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: unknown, record: StaffResponse) => (
        <div className="action-buttons">
          {record.data && (
            <>
              <Button type="link" onClick={() => console.log("Edit", record)}>
                Edit
              </Button>
              <Button
                type="link"
                danger
                onClick={() => console.log("Delete", record)}
              >
                Delete
              </Button>
            </>
          )}
        </div>
      ),
    },
  ];

  return (
    <MainLayout>
      <div className="staff-page">
        <div className="staff-header">
          <h2>Staff Management</h2>
          <Button type="primary" icon={<PlusOutlined />}>
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
          rowKey={(record) => record.data?.email || Math.random().toString()}
          pagination={false}
          loading={loading}
          onChange={(pagination) =>
            handleTableChange({
              current: pagination.current || 1,
              pageSize: pagination.pageSize || 10
            })
          }
        />

        <div
          className="pagination-container"
          style={{ marginTop: 16, textAlign: "right" }}
        >
          <Pagination
            current={pagination.current}
            pageSize={pagination.pageSize}
            total={pagination.total}
            onChange={(page, pageSize) =>
              fetchStaffs(page, pageSize, searchText)
            }
            showSizeChanger
            showTotal={(total) => `Total ${total} items`}
          />
        </div>
      </div>
    </MainLayout>
  );
};

export default Staff;
