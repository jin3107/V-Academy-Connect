import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, DatePicker, Popconfirm, Switch,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined,
} from "@ant-design/icons";
import {
  createAttendance, searchAttendances, updateAttendance, deleteAttendance,
} from "../../apis/attendanceAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  AttendanceRequest, AttendanceResponse, ClassResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

interface AttendanceFormValues {
  classId: string;
  studentId: string;
  date: Dayjs;
  isPresent: boolean;
  countAsPaid: boolean;
}

const Attendances = () => {
  const [loading, setLoading] = useState(false);
  const [attendances, setAttendances] = useState<AttendanceResponse[]>([]);
  const [filterClassId, setFilterClassId] = useState<string | undefined>(undefined);
  const [filterDate, setFilterDate] = useState<Dayjs | null>(null);
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [form] = Form.useForm<AttendanceFormValues>();
  const [formClassId, setFormClassId] = useState<string | undefined>(undefined);

  const [classOptions, setClassOptions] = useState<{ label: string; value: string }[]>([]);
  const [studentOptions, setStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [rosterLoading, setRosterLoading] = useState(false);

  const loadClasses = useCallback(async () => {
    const response = await searchClasses({ filters: [], pageIndex: 1, pageSize: 200 });
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load classes");
      return;
    }
    setClassOptions(
      (response.data.data.data || []).map((c: ClassResponse) => ({ label: c.name, value: c.id! }))
    );
  }, []);

  const loadRoster = useCallback(async (classId: string) => {
    setRosterLoading(true);
    const response = await searchClassStudents({
      filters: [{ fieldName: "ClassId", value: classId }],
      pageIndex: 1,
      pageSize: 200,
    });
    setRosterLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load class roster");
      setStudentOptions([]);
      return;
    }
    setStudentOptions(
      (response.data.data.data || []).map((cs: ClassStudentResponse) => ({
        label: cs.studentName || cs.email || cs.studentId || "",
        value: cs.studentId!,
      }))
    );
  }, []);

  useEffect(() => {
    loadClasses();
  }, [loadClasses]);

  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setFormClassId(undefined);
    setStudentOptions([]);
    setOpen(true);
  };

  const openUpdate = (record: AttendanceResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setFormClassId(record.classId);
    setOpen(true);
  };

  const onFinish = async (values: AttendanceFormValues) => {
    try {
      setSubmitting(true);
      const payload: AttendanceRequest = {
        id: mode === "update" ? selectedId : undefined,
        classId: values.classId,
        studentId: values.studentId,
        date: values.date.toISOString(),
        isPresent: values.isPresent,
        countAsPaid: values.countAsPaid,
      } as AttendanceRequest;

      const res = mode === "create" ? await createAttendance(payload) : await updateAttendance(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchAttendances(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const fetchAttendances = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (filterClassId) filters.push({ fieldName: "ClassId", value: filterClassId });
        if (filterDate) filters.push({ fieldName: "Date", value: filterDate.toISOString() });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchAttendances(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<AttendanceResponse>;
          setAttendances(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch attendances");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch attendances");
      } finally {
        setLoading(false);
      }
    },
    [filterClassId, filterDate, sortField, sortDirection]
  );

  useEffect(() => {
    fetchAttendances(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchAttendances]);

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
    fetchAttendances(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: "Class",
      dataIndex: "className",
      key: "className",
      render: (_: unknown, r: AttendanceResponse) => r.className || "-",
    },
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: AttendanceResponse) => r.studentName || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Date")}>
          Date {renderSortIcon("Date")}
        </div>
      ),
      dataIndex: "date",
      key: "date",
      render: (_: unknown, r: AttendanceResponse) => (r.date ? dayjs(r.date).format("YYYY-MM-DD") : "-"),
    },
    {
      title: "Present",
      dataIndex: "isPresent",
      key: "isPresent",
      render: (_: unknown, r: AttendanceResponse) => (r.isPresent ? "Yes" : "No"),
    },
    {
      title: "Counts As Paid",
      dataIndex: "countAsPaid",
      key: "countAsPaid",
      render: (_: unknown, r: AttendanceResponse) => (r.countAsPaid ? "Yes" : "No"),
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: AttendanceResponse) => (
        <div className="action-buttons">
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this attendance record?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteAttendance(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchAttendances(currentPage, pageSize);
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

  return (
    <MainLayout>
      <div className="attendance-page">
        <div className="attendance-header">
          <h2>Attendance</h2>
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
            Record Attendance
          </Button>
        </div>

        <div className="attendance-search">
          <div className="search-container">
            <Select
              placeholder="Filter by class"
              allowClear
              style={{ minWidth: 220 }}
              options={classOptions}
              value={filterClassId}
              onChange={(value) => {
                setFilterClassId(value);
                fetchAttendances(1, pageSize);
              }}
              showSearch
              optionFilterProp="label"
            />
            <DatePicker
              placeholder="Filter by date"
              value={filterDate}
              onChange={(value) => {
                setFilterDate(value);
                fetchAttendances(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="attendance-table"
          columns={columns}
          dataSource={attendances}
          rowKey={(record) => record.id || `${record.classId}-${record.studentId}-${record.date}`}
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
          title={mode === "create" ? "Record Attendance" : "Update Attendance"}
          open={open}
          onCancel={() => setOpen(false)}
          footer={[
            <Button key="cancel" onClick={() => setOpen(false)}>Cancel</Button>,
            <Button key="submit" type="primary" loading={submitting} onClick={() => form.submit()}>
              {mode === "create" ? "Submit" : "Save"}
            </Button>,
          ]}
          destroyOnHidden
          afterOpenChange={(visible) => {
            if (!visible) return;
            form.resetFields();
            if (formClassId) loadRoster(formClassId);
          }}
        >
          <Form
            form={form}
            layout="vertical"
            onFinish={onFinish}
            preserve={false}
            initialValues={{
              classId: formClassId,
              date: dayjs(),
              isPresent: true,
              countAsPaid: true,
            }}
          >
            <Form.Item
              name="classId"
              label="Class"
              rules={[{ required: true, message: "Class is required" }]}
            >
              <Select
                options={classOptions}
                placeholder="Select class"
                showSearch
                optionFilterProp="label"
                onChange={(value) => {
                  setFormClassId(value);
                  form.setFieldValue("studentId", undefined);
                  loadRoster(value);
                }}
              />
            </Form.Item>

            <Form.Item
              name="studentId"
              label="Student"
              rules={[{ required: true, message: "Student is required" }]}
            >
              <Select
                options={studentOptions}
                loading={rosterLoading}
                placeholder={formClassId ? "Select student" : "Select a class first"}
                disabled={!formClassId}
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Form.Item
              name="date"
              label="Date"
              rules={[{ required: true, message: "Date is required" }]}
            >
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>

            <Form.Item name="isPresent" label="Present" valuePropName="checked">
              <Switch />
            </Form.Item>

            <Form.Item name="countAsPaid" label="Counts As Paid" valuePropName="checked">
              <Switch />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Attendances;
