import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import { Button, Input, Table, message, Select } from "antd";
import { PlusOutlined, SearchOutlined, SortAscendingOutlined, SortDescendingOutlined } from "@ant-design/icons";
import { searchStaffs } from "../../apis/staffAPI";
import type { Filter, SearchRequest, SearchResponse } from "../../interfaces";
import "./style.scss";

const { Option } = Select;
import { Typography } from "antd";
const { Text } = Typography;

interface StaffData {
  id?: string;
  name: string;
  email: string;
  phoneNumber: string;
  role: string;
}

const Staff = () => {
  const [loading, setLoading] = useState(false);
  const [staffs, setStaffs] = useState<StaffData[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

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
          const searchResponse = response as unknown as SearchResponse<StaffData>;
          let staffData: StaffData[] = [];
          if (searchResponse.data && searchResponse.data.data) {
            staffData = searchResponse.data.data.map((item: StaffData | { data: StaffData }) => {
              if ('data' in item) {
                return item.data;
              }
              else {
                return item;
              }
            });
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

  const handleTableChange = (newPagination: {
    current?: number;
    pageSize?: number;
  }) => {
    const newPage = newPagination.current || currentPage;
    const newPageSize = newPagination.pageSize || pageSize;
    setCurrentPage(newPage);
    setPageSize(newPageSize);
    fetchStaffs(newPage, newPageSize, searchText);
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
      title: <div className="column-header" onClick={() => handleSortChange("Name")}>Name {renderSortIcon("Name")}</div>,
      dataIndex: "name",
      key: "name",
      width: 220,
      ellipsis: true,
      render: (_: unknown, r: StaffData) => <Text ellipsis={{ tooltip: r.name }}>{r.name || "-"}</Text>,
    },
    {
      title: <div className="column-header" onClick={() => handleSortChange("Email")}>Email {renderSortIcon("Email")}</div>,
      dataIndex: "email",
      key: "email",
      width: 260,
      ellipsis: true,
      render: (_: unknown, r: StaffData) => <Text ellipsis={{ tooltip: r.email }}>{r.email || "-"}</Text>,
    },
    {
      title: <div className="column-header" onClick={() => handleSortChange("PhoneNumber")}>Phone Number {renderSortIcon("PhoneNumber")}</div>,
      dataIndex: "phoneNumber",
      key: "phoneNumber",
      width: 160,
      ellipsis: true,
    },
    {
      title: <div className="column-header" onClick={() => handleSortChange("Role")}>Role {renderSortIcon("Role")}</div>,
      dataIndex: "role",
      key: "role",
      width: 140,
      ellipsis: true,
    },
    {
      title: "Actions",
      key: "actions",
      width: 200,
      fixed: "right" as const,
      render: (_: unknown, record: StaffData) => (
        <div className="action-buttons" style={{ whiteSpace: "nowrap" }}>
          <Button type="link" onClick={() => console.log("Detail", record)}>Detail</Button>
          <Button type="link" onClick={() => console.log("Edit", record)}>Edit</Button>
          <Button type="link" danger onClick={() => console.log("Delete", record)}>Delete</Button>
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
          rowKey={(record) => {
            const key = record.id || record.email
            return key;
          }}
          pagination={{
            current: currentPage,
            pageSize: pageSize,
            total: total,
            showSizeChanger: true,
            showTotal: (total) => `Total ${total} items`,
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

export default Staff;