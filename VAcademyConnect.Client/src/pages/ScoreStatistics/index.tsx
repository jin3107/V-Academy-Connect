import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, InputNumber, Input, Popconfirm, DatePicker,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined, CalculatorOutlined,
} from "@ant-design/icons";
import {
  createScoreStatistic, searchScoreStatistics, updateScoreStatistic,
  deleteScoreStatistic, calculateScoreStatistics,
} from "../../apis/scoreStatisticAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  ScoreStatisticRequest, ScoreStatisticResponse, ClassResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

interface ScoreStatisticFormValues {
  studentId: string;
  month: Dayjs;
  avarageScore: number;
}

interface CalculateFormValues {
  month: Dayjs;
  classId?: string;
}

const ScoreStatistics = () => {
  const [loading, setLoading] = useState(false);
  const [scoreStatistics, setScoreStatistics] = useState<ScoreStatisticResponse[]>([]);
  const [filterMonth, setFilterMonth] = useState<Dayjs | null>(null);
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [form] = Form.useForm<ScoreStatisticFormValues>();

  const [calculateOpen, setCalculateOpen] = useState(false);
  const [calculating, setCalculating] = useState(false);
  const [calculateForm] = Form.useForm<CalculateFormValues>();

  const [studentOptions, setStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [studentsLoading, setStudentsLoading] = useState(false);

  const [classOptions, setClassOptions] = useState<{ label: string; value: string }[]>([]);

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

  useEffect(() => {
    loadStudents();
    loadClasses();
  }, [loadStudents, loadClasses]);

  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
  };

  const openUpdate = (record: ScoreStatisticResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setOpen(true);
    setTimeout(() => {
      form.setFieldsValue({
        studentId: record.studentId,
        month: dayjs(record.month + "-01"),
        avarageScore: record.avarageScore,
      });
    }, 0);
  };

  const onFinish = async (values: ScoreStatisticFormValues) => {
    try {
      setSubmitting(true);
      const payload: ScoreStatisticRequest = {
        id: mode === "update" ? selectedId : undefined,
        studentId: values.studentId,
        month: values.month.format("YYYY-MM"),
        avarageScore: values.avarageScore,
      } as ScoreStatisticRequest;

      const res = mode === "create" ? await createScoreStatistic(payload) : await updateScoreStatistic(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchScoreStatistics(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const onCalculate = async (values: CalculateFormValues) => {
    try {
      setCalculating(true);
      const res = await calculateScoreStatistics({
        month: values.month.format("YYYY-MM"),
        classId: values.classId,
      });
      if (!res.isSuccess) {
        message.error(res.message || "Calculate failed");
      } else {
        message.success(res.message || "Calculated");
        setCalculateOpen(false);
        fetchScoreStatistics(1, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setCalculating(false);
    }
  };

  const fetchScoreStatistics = useCallback(
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

        const response = await searchScoreStatistics(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<ScoreStatisticResponse>;
          setScoreStatistics(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch score statistics");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch score statistics");
      } finally {
        setLoading(false);
      }
    },
    [filterMonth, sortField, sortDirection]
  );

  useEffect(() => {
    fetchScoreStatistics(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchScoreStatistics]);

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
    fetchScoreStatistics(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: ScoreStatisticResponse) => r.studentName || "-",
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
        <div className="column-header" onClick={() => handleSortChange("AvarageScore")}>
          Average Score {renderSortIcon("AvarageScore")}
        </div>
      ),
      dataIndex: "avarageScore",
      key: "avarageScore",
      render: (_: unknown, r: ScoreStatisticResponse) => r.avarageScore.toFixed(2),
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: ScoreStatisticResponse) => (
        <div className="action-buttons">
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this score statistic?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteScoreStatistic(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchScoreStatistics(currentPage, pageSize);
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
      <div className="score-statistic-page">
        <div className="score-statistic-header">
          <h2>Score Statistics</h2>
          <div className="header-actions">
            <Button icon={<CalculatorOutlined />} onClick={() => setCalculateOpen(true)}>
              Calculate
            </Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
              Add Statistic
            </Button>
          </div>
        </div>

        <div className="score-statistic-search">
          <div className="search-container">
            <DatePicker
              picker="month"
              placeholder="Filter by month"
              value={filterMonth}
              onChange={(value) => {
                setFilterMonth(value);
                fetchScoreStatistics(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="score-statistic-table"
          columns={columns}
          dataSource={scoreStatistics}
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
          title={mode === "create" ? "Add Statistic" : "Update Statistic"}
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
              name="avarageScore"
              label="Average Score"
              rules={[{ required: true, message: "Average score is required" }]}
            >
              <InputNumber min={0} max={10} step={0.1} style={{ width: "100%" }} />
            </Form.Item>
          </Form>
        </Modal>

        <Modal
          title="Calculate Average Scores"
          open={calculateOpen}
          onCancel={() => setCalculateOpen(false)}
          footer={[
            <Button key="cancel" onClick={() => setCalculateOpen(false)}>Cancel</Button>,
            <Button key="submit" type="primary" loading={calculating} onClick={() => calculateForm.submit()}>
              Calculate
            </Button>,
          ]}
          destroyOnHidden
          afterOpenChange={(visible) => {
            if (!visible) return;
            calculateForm.resetFields();
          }}
        >
          <Form form={calculateForm} layout="vertical" onFinish={onCalculate} preserve={false}>
            <Form.Item
              name="month"
              label="Month"
              rules={[{ required: true, message: "Month is required" }]}
            >
              <DatePicker picker="month" style={{ width: "100%" }} />
            </Form.Item>

            <Form.Item name="classId" label="Class (optional)">
              <Select
                options={classOptions}
                placeholder="All classes"
                allowClear
                showSearch
                optionFilterProp="label"
              />
            </Form.Item>

            <Input.TextArea
              disabled
              value="Calculates the average score for each student from recorded Scores in the selected month (and class, if chosen), then creates or updates their Score Statistic."
              autoSize
              className="calculate-hint"
            />
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default ScoreStatistics;
