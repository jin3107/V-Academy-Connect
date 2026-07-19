import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Input, Table, message, Select,
  Modal, Form, InputNumber, Popconfirm, TimePicker, Tag, List, Empty, Divider,
} from "antd";
import {
  PlusOutlined, SearchOutlined, SortAscendingOutlined, SortDescendingOutlined,
  TeamOutlined, UserOutlined, UserAddOutlined,
} from "@ant-design/icons";
import {
  createClass, searchClasses, updateClass, deleteClass,
} from "../../apis/classAPI";
import { searchClassStudents, createClassStudent } from "../../apis/classStudentAPI";
import { searchStudents, createStudent } from "../../apis/studentAPI";
import { searchStaffs } from "../../apis/staffAPI";
import { searchTenants } from "../../apis/tenantAPI";
import { useAuth } from "../../contexts";
import type {
  Filter, SearchRequest, SearchResponse,
  ClassRequest, ClassResponse, StaffResponse, TenantReponse, ClassStudentResponse,
  StudentResponse, StudentRequest,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

const DAY_OF_WEEK_OPTIONS = [
  { label: "Sunday", value: 0 },
  { label: "Monday", value: 1 },
  { label: "Tuesday", value: 2 },
  { label: "Wednesday", value: 3 },
  { label: "Thursday", value: 4 },
  { label: "Friday", value: 5 },
  { label: "Saturday", value: 6 },
];

const dayOfWeekLabel = (value?: number) =>
  DAY_OF_WEEK_OPTIONS.find((d) => d.value === value)?.label;

interface ClassFormValues {
  name: string;
  tenantId?: string;
  tuition: number;
  teacherId?: string;
  teachingDayOfWeek?: number;
  teachingTime?: [Dayjs, Dayjs];
}

interface NewStudentFormValues {
  name: string;
  email: string;
  phoneNumber: string;
  password: string;
}

const Classes = () => {
  const { user } = useAuth();
  const isSuperAdmin = user?.role === "SuperAdmin";

  // ===== Table state =====
  const [loading, setLoading] = useState(false);
  const [classes, setClasses] = useState<ClassResponse[]>([]);
  const [searchName, setSearchName] = useState("");
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
  const [form] = Form.useForm<ClassFormValues>();

  // ===== Roster modal state =====
  const [rosterOpen, setRosterOpen] = useState(false);
  const [rosterClass, setRosterClass] = useState<ClassResponse | undefined>(undefined);
  const [rosterLoading, setRosterLoading] = useState(false);
  const [roster, setRoster] = useState<ClassStudentResponse[]>([]);

  // ===== Add existing student state =====
  const [studentOptions, setStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [studentSearchLoading, setStudentSearchLoading] = useState(false);
  const [selectedStudentId, setSelectedStudentId] = useState<string | undefined>(undefined);
  const [addingStudent, setAddingStudent] = useState(false);

  // ===== New student modal state =====
  const [newStudentOpen, setNewStudentOpen] = useState(false);
  const [newStudentForm] = Form.useForm<NewStudentFormValues>();
  const [creatingStudent, setCreatingStudent] = useState(false);

  // ===== Options =====
  const [tenantOptions, setTenantOptions] = useState<{ label: string; value: string }[]>([]);
  const [teacherOptions, setTeacherOptions] = useState<{ label: string; value: string }[]>([]);
  const [teachersLoading, setTeachersLoading] = useState(false);

  // ===== Loaders =====
  const loadTenants = useCallback(async () => {
    if (!isSuperAdmin) return;
    const response = await searchTenants({ filters: [], pageIndex: 1, pageSize: 200 });
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load tenants");
      return;
    }
    setTenantOptions(
      (response.data.data.data || []).map((t: TenantReponse) => ({ label: t.name, value: t.id! }))
    );
  }, [isSuperAdmin]);

  const loadTeachers = useCallback(async () => {
    setTeachersLoading(true);
    const filters: Filter[] = [{ fieldName: "Role", value: "Teacher" }];
    const response = await searchStaffs({ filters, pageIndex: 1, pageSize: 200 });
    setTeachersLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load teachers");
      return;
    }
    const list = (response.data.data.data || []) as StaffResponse[];
    setTeacherOptions(list.map((s) => ({ label: s.name, value: s.id! })));
  }, []);

  useEffect(() => {
    loadTenants();
    loadTeachers();
  }, [loadTenants, loadTeachers]);

  // ===== Openers =====
  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
  };

  const openUpdate = (record: ClassResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setOpen(true);
    setTimeout(() => {
      form.setFieldsValue({
        name: record.name,
        tenantId: record.tenantId,
        tuition: record.tuition,
        teacherId: record.teacherId,
        teachingDayOfWeek: record.teachingDayOfWeek,
        teachingTime:
          record.teachingStartTime && record.teachingEndTime
            ? [dayjs(record.teachingStartTime, "HH:mm:ss"), dayjs(record.teachingEndTime, "HH:mm:ss")]
            : undefined,
      });
    }, 0);
  };

  // ===== Submit (Create / Update) =====
  const onFinish = async (values: ClassFormValues) => {
    try {
      setSubmitting(true);
      const payload: ClassRequest = {
        id: mode === "update" ? selectedId : undefined,
        name: values.name,
        tenantId: isSuperAdmin ? values.tenantId : user?.tenantId,
        tuition: values.tuition,
        teacherId: values.teacherId,
        teachingDayOfWeek: values.teachingDayOfWeek,
        teachingStartTime: values.teachingTime ? values.teachingTime[0].format("HH:mm:ss") : undefined,
        teachingEndTime: values.teachingTime ? values.teachingTime[1].format("HH:mm:ss") : undefined,
      };

      const res = mode === "create" ? await createClass(payload) : await updateClass(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchClasses(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  // ===== Roster =====
  const loadRoster = useCallback(async (classId: string) => {
    setRosterLoading(true);
    const response = await searchClassStudents({
      filters: [{ fieldName: "ClassId", value: classId }],
      pageIndex: 1,
      pageSize: 200,
    });
    setRosterLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load roster");
      setRoster([]);
      return;
    }
    setRoster(response.data.data.data || []);
  }, []);

  const openRoster = async (record: ClassResponse) => {
    setRosterClass(record);
    setRosterOpen(true);
    setSelectedStudentId(undefined);
    setStudentOptions([]);
    if (!record.id) return;
    loadRoster(record.id);
  };

  const handleSearchStudents = useCallback(async (name: string) => {
    if (!name) {
      setStudentOptions([]);
      return;
    }
    setStudentSearchLoading(true);
    const response = await searchStudents({
      filters: [{ fieldName: "Name", value: name }],
      pageIndex: 1,
      pageSize: 20,
    });
    setStudentSearchLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to search students");
      return;
    }
    const list = (response.data.data.data || []) as StudentResponse[];
    setStudentOptions(list.map((s) => ({ label: `${s.name} (${s.email})`, value: s.id })));
  }, []);

  const handleAddExistingStudent = async () => {
    if (!rosterClass?.id || !selectedStudentId) return;
    try {
      setAddingStudent(true);
      const res = await createClassStudent({ classId: rosterClass.id, studentId: selectedStudentId });
      if (!res.isSuccess) {
        message.error(res.message || "Failed to add student");
      } else {
        message.success(res.message || "Student added");
        setSelectedStudentId(undefined);
        loadRoster(rosterClass.id);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setAddingStudent(false);
    }
  };

  const onCreateNewStudent = async (values: NewStudentFormValues) => {
    if (!rosterClass?.id) return;
    try {
      setCreatingStudent(true);
      const payload: StudentRequest = {
        name: values.name,
        email: values.email,
        phoneNumber: values.phoneNumber,
        password: values.password,
        tenantId: isSuperAdmin ? rosterClass.tenantId : user?.tenantId,
      };
      const createRes = await createStudent(payload);
      if (!createRes.isSuccess || !createRes.data) {
        message.error(createRes.message || "Failed to create student");
        return;
      }

      const enrollRes = await createClassStudent({ classId: rosterClass.id, studentId: createRes.data.id });
      if (!enrollRes.isSuccess) {
        message.error(enrollRes.message || "Student created but failed to add to class");
        return;
      }

      message.success("Student created and added to class");
      setNewStudentOpen(false);
      loadRoster(rosterClass.id);
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setCreatingStudent(false);
    }
  };

  // ===== Table data loader =====
  const fetchClasses = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (searchName) filters.push({ fieldName: "Name", value: searchName });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchClasses(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<ClassResponse>;
          setClasses(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch classes");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch classes");
      } finally {
        setLoading(false);
      }
    },
    [searchName, sortField, sortDirection]
  );

  useEffect(() => {
    fetchClasses(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchClasses]);

  // ===== Handlers =====
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
    fetchClasses(newPage, newPageSize);
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
    },
    ...(isSuperAdmin
      ? [
          {
            title: "Tenant",
            dataIndex: "tenantName",
            key: "tenantName",
            render: (_: unknown, r: ClassResponse) => r.tenantName || "-",
          },
        ]
      : []),
    {
      title: "Teacher",
      dataIndex: "teacherName",
      key: "teacherName",
      render: (_: unknown, r: ClassResponse) => r.teacherName || <Tag>Unassigned</Tag>,
    },
    {
      title: "Schedule",
      key: "schedule",
      render: (_: unknown, r: ClassResponse) =>
        r.teachingDayOfWeek !== undefined && r.teachingStartTime && r.teachingEndTime
          ? `${dayOfWeekLabel(r.teachingDayOfWeek)} ${r.teachingStartTime.slice(0, 5)}-${r.teachingEndTime.slice(0, 5)}`
          : "-",
    },
    {
      title: "Tuition",
      dataIndex: "tuition",
      key: "tuition",
      render: (_: unknown, r: ClassResponse) => r.tuition.toLocaleString("vi-VN") + " đ",
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: ClassResponse) => (
        <div className="action-buttons">
          <Button type="link" icon={<TeamOutlined />} onClick={() => openRoster(record)}>
            Students
          </Button>
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this class?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteClass(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchClasses(currentPage, pageSize);
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
      <div className="class-page">
        <div className="class-header">
          <h2>Classes</h2>
          <div className="header-actions">
            <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
              Add Class
            </Button>
          </div>
        </div>

        <div className="class-search">
          <div className="search-container">
            <Input
              placeholder="Search by class name"
              prefix={<SearchOutlined />}
              value={searchName}
              onChange={(e) => setSearchName(e.target.value)}
              onPressEnter={() => fetchClasses(1, pageSize)}
              allowClear
              style={{ width: 260 }}
            />
          </div>
        </div>

        <Table
          className="class-table"
          columns={columns}
          dataSource={classes}
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
          title={mode === "create" ? "Add Class" : "Update Class"}
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
            if (mode === "create") form.resetFields();
          }}
        >
          <Form form={form} layout="vertical" onFinish={onFinish} preserve={false}>
            <Form.Item
              name="name"
              label="Class Name"
              rules={[{ required: true, message: "Class name is required" }]}
            >
              <Input placeholder="Enter class name" />
            </Form.Item>

            {isSuperAdmin && (
              <Form.Item
                name="tenantId"
                label="Tenant"
                rules={[{ required: true, message: "Tenant is required" }]}
              >
                <Select
                  options={tenantOptions}
                  placeholder="Select tenant"
                  showSearch
                  optionFilterProp="label"
                  onChange={() => form.setFieldValue("teacherId", undefined)}
                />
              </Form.Item>
            )}

            <Form.Item
              name="tuition"
              label="Tuition"
              rules={[{ required: true, message: "Tuition is required" }]}
            >
              <InputNumber
                min={0}
                step={100000}
                style={{ width: "100%" }}
                formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}
                parser={(value) => Number(value?.replace(/,/g, "")) as unknown as 0}
              />
            </Form.Item>

            <Form.Item name="teacherId" label="Teacher">
              <Select
                options={teacherOptions}
                loading={teachersLoading}
                placeholder="Select teacher (optional)"
                allowClear
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Form.Item name="teachingDayOfWeek" label="Day of Week">
              <Select options={DAY_OF_WEEK_OPTIONS} placeholder="Select day" allowClear />
            </Form.Item>

            <Form.Item name="teachingTime" label="Teaching Time">
              <TimePicker.RangePicker format="HH:mm" style={{ width: "100%" }} />
            </Form.Item>
          </Form>
        </Modal>

        <Modal
          title={`Students${rosterClass?.name ? " — " + rosterClass.name : ""}`}
          open={rosterOpen}
          onCancel={() => setRosterOpen(false)}
          footer={[<Button key="close" onClick={() => setRosterOpen(false)}>Close</Button>]}
          destroyOnHidden
        >
          <List
            loading={rosterLoading}
            locale={{ emptyText: <Empty description="No students enrolled yet" /> }}
            dataSource={roster}
            renderItem={(item) => (
              <List.Item>
                <List.Item.Meta
                  avatar={<UserOutlined />}
                  title={item.studentName || item.email || item.studentId}
                  description={item.email}
                />
              </List.Item>
            )}
          />

          <Divider />

          <div className="add-student-row">
            <Select
              showSearch
              placeholder="Search student by name to add"
              value={selectedStudentId}
              options={studentOptions}
              loading={studentSearchLoading}
              filterOption={false}
              onSearch={handleSearchStudents}
              onChange={setSelectedStudentId}
              notFoundContent={studentSearchLoading ? "Searching..." : "Type a name to search"}
              style={{ flex: 1 }}
              allowClear
            />
            <Button
              type="primary"
              disabled={!selectedStudentId}
              loading={addingStudent}
              onClick={handleAddExistingStudent}
            >
              Add
            </Button>
          </div>

          <Button
            type="link"
            icon={<UserAddOutlined />}
            className="new-student-link"
            onClick={() => {
              newStudentForm.resetFields();
              setNewStudentOpen(true);
            }}
          >
            This student doesn't have an account yet — create one
          </Button>
        </Modal>

        <Modal
          title="Create New Student"
          open={newStudentOpen}
          onCancel={() => setNewStudentOpen(false)}
          footer={[
            <Button key="cancel" onClick={() => setNewStudentOpen(false)}>Cancel</Button>,
            <Button key="submit" type="primary" loading={creatingStudent} onClick={() => newStudentForm.submit()}>
              Create & Add
            </Button>,
          ]}
          destroyOnHidden
        >
          <Form form={newStudentForm} layout="vertical" onFinish={onCreateNewStudent} preserve={false}>
            <Form.Item
              name="name"
              label="Full Name"
              rules={[{ required: true, message: "Name is required" }]}
            >
              <Input placeholder="Enter student's name" />
            </Form.Item>

            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: "Email is required" },
                { type: "email", message: "Please enter a valid email" },
              ]}
            >
              <Input placeholder="Enter email" />
            </Form.Item>

            <Form.Item
              name="phoneNumber"
              label="Phone Number"
              rules={[{ required: true, message: "Phone number is required" }]}
            >
              <Input placeholder="Enter phone number" />
            </Form.Item>

            <Form.Item
              name="password"
              label="Password"
              rules={[{ required: true, message: "Password is required" }]}
            >
              <Input.Password placeholder="Set an initial password" />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Classes;
