import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, InputNumber, Popconfirm, DatePicker,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined, CalculatorOutlined,
} from "@ant-design/icons";
import {
  createTuitionFee, searchTuitionFees, updateTuitionFee,
  deleteTuitionFee, estimateTuitionFee,
} from "../../apis/tuitionFeeAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  TuitionFeeRequest, TuitionFeeResponse, ClassResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

interface TuitionFeeFormValues {
  studentId: string;
  month: Dayjs;
  amount: number;
  dueDate?: Dayjs;
}

interface EstimateFormValues {
  classId: string;
  studentId: string;
  cycleStartDate: Dayjs;
}

const TuitionFees = () => {
  // ===== Table state =====
  const [loading, setLoading] = useState(false);
  const [tuitionFees, setTuitionFees] = useState<TuitionFeeResponse[]>([]);
  const [filterMonth, setFilterMonth] = useState<Dayjs | null>(null);
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
  const [form] = Form.useForm<TuitionFeeFormValues>();

  // ===== Estimate modal state =====
  const [estimateOpen, setEstimateOpen] = useState(false);
  const [estimating, setEstimating] = useState(false);
  const [estimateForm] = Form.useForm<EstimateFormValues>();
  const [estimateClassId, setEstimateClassId] = useState<string | undefined>(undefined);
  const [estimateStudentOptions, setEstimateStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [estimateRosterLoading, setEstimateRosterLoading] = useState(false);

  // ===== Student options (all students across classes, for manual entry) =====
  const [studentOptions, setStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [studentsLoading, setStudentsLoading] = useState(false);

  // ===== Class options (for estimate) =====
  const [classOptions, setClassOptions] = useState<{ label: string; value: string }[]>([]);

  // ===== Loaders =====
  const loadStudents = useCallback(async () => {
    setStudentsLoading(true);
    const response = await searchClassStudents({ filters: [], pageIndex: 1, pageSize: 500 });
    setStudentsLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load students");
      return;
    }
    const seen = new Set<string>();
    const options: { label: string; value: string }[] = [];
    (response.data.data.data || []).forEach((cs: ClassStudentResponse) => {
      if (cs.studentId && !seen.has(cs.studentId)) {
        seen.add(cs.studentId);
        options.push({ label: cs.studentName || cs.email || cs.studentId, value: cs.studentId });
      }
    });
    setStudentOptions(options);
  }, []);

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

  const loadEstimateRoster = useCallback(async (classId: string) => {
    setEstimateRosterLoading(true);
    const response = await searchClassStudents({
      filters: [{ fieldName: "ClassId", value: classId }],
      pageIndex: 1,
      pageSize: 200,
    });
    setEstimateRosterLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load class roster");
      setEstimateStudentOptions([]);
      return;
    }
    setEstimateStudentOptions(
      (response.data.data.data || []).map((cs: ClassStudentResponse) => ({
        label: cs.studentName || cs.email || cs.studentId || "",
        value: cs.studentId!,
      }))
    );
  }, []);

  useEffect(() => {
    loadStudents();
    loadClasses();
  }, [loadStudents, loadClasses]);

  // ===== Openers =====
  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
  };

  const openUpdate = (record: TuitionFeeResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setOpen(true);
    setTimeout(() => {
      form.setFieldsValue({
        studentId: record.studentId,
        month: dayjs(record.month + "-01"),
        amount: record.amount,
        dueDate: record.dueDate ? dayjs(record.dueDate) : undefined,
      });
    }, 0);
  };

  // ===== Submit (Create / Update) =====
  const onFinish = async (values: TuitionFeeFormValues) => {
    try {
      setSubmitting(true);
      const payload: TuitionFeeRequest = {
        id: mode === "update" ? selectedId : undefined,
        studentId: values.studentId,
        month: values.month.format("YYYY-MM"),
        amount: values.amount,
        dueDate: values.dueDate ? values.dueDate.toISOString() : undefined,
      } as TuitionFeeRequest;

      const res = mode === "create" ? await createTuitionFee(payload) : await updateTuitionFee(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchTuitionFees(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  // ===== Estimate =====
  const onEstimate = async (values: EstimateFormValues) => {
    try {
      setEstimating(true);
      const res = await estimateTuitionFee({
        studentId: values.studentId,
        classId: values.classId,
        cycleStartDate: values.cycleStartDate.toISOString(),
      });
      if (!res.isSuccess) {
        message.error(res.message || "Estimate failed");
      } else {
        message.success(res.message || "Estimated");
        setEstimateOpen(false);
        fetchTuitionFees(1, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setEstimating(false);
    }
  };

  // ===== Table data loader =====
  const fetchTuitionFees = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (filterMonth) filters.push({ fieldName: "Month", value: filterMonth.format("YYYY-MM") });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchTuitionFees(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<TuitionFeeResponse>;
          setTuitionFees(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch tuition fees");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch tuition fees");
      } finally {
        setLoading(false);
      }
    },
    [filterMonth, sortField, sortDirection]
  );

  useEffect(() => {
    fetchTuitionFees(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchTuitionFees]);

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
    fetchTuitionFees(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  // ===== Columns =====
  const columns = [
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: TuitionFeeResponse) => r.studentName || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Month")}>
          Month {renderSortIcon("Month")}
        </div>
      ),
      dataIndex: "month",
      key: "month",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Amount")}>
          Amount {renderSortIcon("Amount")}
        </div>
      ),
      dataIndex: "amount",
      key: "amount",
      render: (_: unknown, r: TuitionFeeResponse) => r.amount.toLocaleString("vi-VN") + " đ",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("DueDate")}>
          Due Date {renderSortIcon("DueDate")}
        </div>
      ),
      dataIndex: "dueDate",
      key: "dueDate",
      render: (_: unknown, r: TuitionFeeResponse) => (r.dueDate ? dayjs(r.dueDate).format("YYYY-MM-DD") : "-"),
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: TuitionFeeResponse) => (
        <div className="action-buttons">
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this tuition fee record?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteTuitionFee(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchTuitionFees(currentPage, pageSize);
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
      <div className="tuition-fee-page">
        <div className="tuition-fee-header">
          <h2>Tuition Fees</h2>
          <div className="header-actions">
            <Button icon={<CalculatorOutlined />} onClick={() => setEstimateOpen(true)}>
              Estimate Due Date
            </Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
              Add Tuition Fee
            </Button>
          </div>
        </div>

        <div className="tuition-fee-search">
          <div className="search-container">
            <DatePicker
              picker="month"
              placeholder="Filter by month"
              value={filterMonth}
              onChange={(value) => {
                setFilterMonth(value);
                fetchTuitionFees(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="tuition-fee-table"
          columns={columns}
          dataSource={tuitionFees}
          rowKey={(record) => record.id || `${record.studentId}-${record.month}`}
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
          title={mode === "create" ? "Add Tuition Fee" : "Update Tuition Fee"}
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
              name="studentId"
              label="Student"
              rules={[{ required: true, message: "Student is required" }]}
            >
              <Select
                options={studentOptions}
                loading={studentsLoading}
                placeholder="Select student"
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Form.Item
              name="month"
              label="Month"
              rules={[{ required: true, message: "Month is required" }]}
            >
              <DatePicker picker="month" style={{ width: "100%" }} />
            </Form.Item>

            <Form.Item
              name="amount"
              label="Amount"
              rules={[{ required: true, message: "Amount is required" }]}
            >
              <InputNumber
                min={0}
                step={100000}
                style={{ width: "100%" }}
                formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}
                parser={(value) => Number(value?.replace(/,/g, "")) as unknown as 0}
              />
            </Form.Item>

            <Form.Item name="dueDate" label="Due Date">
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>
          </Form>
        </Modal>

        <Modal
          title="Estimate Due Date"
          open={estimateOpen}
          onCancel={() => setEstimateOpen(false)}
          footer={[
            <Button key="cancel" onClick={() => setEstimateOpen(false)}>Cancel</Button>,
            <Button key="submit" type="primary" loading={estimating} onClick={() => estimateForm.submit()}>
              Estimate
            </Button>,
          ]}
          destroyOnHidden
          afterOpenChange={(visible) => {
            if (!visible) return;
            estimateForm.resetFields();
            setEstimateClassId(undefined);
            setEstimateStudentOptions([]);
          }}
        >
          <Form form={estimateForm} layout="vertical" onFinish={onEstimate} preserve={false}>
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
                  setEstimateClassId(value);
                  estimateForm.setFieldValue("studentId", undefined);
                  loadEstimateRoster(value);
                }}
              />
            </Form.Item>

            <Form.Item
              name="studentId"
              label="Student"
              rules={[{ required: true, message: "Student is required" }]}
            >
              <Select
                options={estimateStudentOptions}
                loading={estimateRosterLoading}
                placeholder={estimateClassId ? "Select student" : "Select a class first"}
                disabled={!estimateClassId}
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Form.Item
              name="cycleStartDate"
              label="Cycle Start Date"
              rules={[{ required: true, message: "Cycle start date is required" }]}
              tooltip="The date this student's current billing cycle began (e.g. enrollment day or last payment date)"
            >
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default TuitionFees;
