import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import { Button, Input, Table, message, Select } from "antd";
import { PlusOutlined, SearchOutlined, SortAscendingOutlined, SortDescendingOutlined } from "@ant-design/icons";
import { searchStaffs } from "../../apis/staffAPI";
import type { Filter, SearchRequest, SearchResponse, StaffResponse } from "../../interfaces";
import "./style.scss";

const { Option } = Select;

const Staff = () => {
  const [loading, setLoading] = useState(false);
  const [staffs, setStaffs] = useState<StaffResponse[]>([]);
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

        // console.log("Search request:", searchRequest);

        const response = await searchStaffs(searchRequest);
        // console.log("Full API response:", response);
        
        if (response.isSuccess) {
          const searchResponse = response as SearchResponse<StaffResponse>;
          // console.log("Success response data:", searchResponse.data);
          // console.log("Items in data.data:", searchResponse.data.data);
          
          // Xử lý dữ liệu - có thể là array trực tiếp hoặc có wrapper
          let staffData: StaffResponse[] = [];
          
          if (searchResponse.data && searchResponse.data.data) {
            staffData = searchResponse.data.data.map((item: StaffResponse | { data: StaffResponse }) => {
              // console.log("Processing item:", item);
              if ('data' in item) {
                return item.data;
              }
              else {
                return item;
              }
            });
          }
          
          // console.log("Final staff data:", staffData);
          // console.log("Staff data length:", staffData.length);
          
          setStaffs(staffData);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
          
          // console.log("Pagination info:", {
          //   currentPage: searchResponse.data.currentPage,
          //   rowsPerPage: searchResponse.data.rowsPerPage,
          //   totalRows: searchResponse.data.totalRows
          // });
          
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
      title: (
        <div className="column-header" onClick={() => handleSortChange("Name")}>
          Name {renderSortIcon("Name")}
        </div>
      ),
      dataIndex: "name",
      key: "name",
      render: (_: unknown, record: StaffResponse) => {
        const name = record.name || "-";
        return name;
      },
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
      dataIndex: "email",
      key: "email",
      render: (_: unknown, record: StaffResponse) => {
        const email = record.email || "-";
        return email;
      },
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
      dataIndex: "phoneNumber",
      key: "phoneNumber",
      render: (_: unknown, record: StaffResponse) => {
        const phone = record.phoneNumber || "-";
        return phone;
      },
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Role")}>
          Role {renderSortIcon("Role")}
        </div>
      ),
      dataIndex: "role",
      key: "role",
      render: (_: unknown, record: StaffResponse) => {
        const role = record.role || "-";
        return role;
      },
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: unknown, record: StaffResponse) => (
        <div className="action-buttons">
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
            const key = record.id || record.email || Math.random().toString();
            // console.log("Row key:", key, "for record:", record);
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