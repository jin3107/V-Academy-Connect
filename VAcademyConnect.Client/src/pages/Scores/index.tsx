import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, Input, InputNumber, Popconfirm,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined,
} from "@ant-design/icons";
import {
  createScore, searchScores, updateScore, deleteScore,
} from "../../apis/scoreAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  ScoreRequest, ScoreResponse, ClassResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";

type Mode = "create" | "update";

interface ScoreFormValues {
  classId: string;
  studentId: string;
  testName: string;
  mark: number;
}

const Scores = () => {
  const [loading, setLoading] = useState(false);
  const [scores, setScores] = useState<ScoreResponse[]>([]);
  const [filterClassId, setFilterClassId] = useState<string | undefined>(undefined);
  const [filterTestName, setFilterTestName] = useState<string>("");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [form] = Form.useForm<ScoreFormValues>();
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

  const openUpdate = (record: ScoreResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setFormClassId(record.classId);
    setOpen(true);
  };

  const onFinish = async (values: ScoreFormValues) => {
    try {
      setSubmitting(true);
      const payload: ScoreRequest = {
        id: mode === "update" ? selectedId : undefined,
        classId: values.classId,
        studentId: values.studentId,
        testName: values.testName,
        mark: values.mark,
      } as ScoreRequest;

      const res = mode === "create" ? await createScore(payload) : await updateScore(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchScores(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const fetchScores = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (filterClassId) filters.push({ fieldName: "ClassId", value: filterClassId });
        if (filterTestName) filters.push({ fieldName: "TestName", value: filterTestName });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchScores(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<ScoreResponse>;
          setScores(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch scores");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch scores");
      } finally {
        setLoading(false);
      }
    },
    [filterClassId, filterTestName, sortField, sortDirection]
  );

  useEffect(() => {
    fetchScores(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchScores]);

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
    fetchScores(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: "Class",
      dataIndex: "className",
      key: "className",
      render: (_: unknown, r: ScoreResponse) => r.className || "-",
    },
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: ScoreResponse) => r.studentName || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("TestName")}>
          Test {renderSortIcon("TestName")}
        </div>
      ),
      dataIndex: "testName",
      key: "testName",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Mark")}>
          Mark {renderSortIcon("Mark")}
        </div>
      ),
      dataIndex: "mark",
      key: "mark",
      render: (_: unknown, r: ScoreResponse) => r.mark.toFixed(2),
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: ScoreResponse) => (
        <div className="action-buttons">
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this score record?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteScore(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchScores(currentPage, pageSize);
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
      <div className="score-page">
        <div className="score-header">
          <h2>Scores</h2>
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
            Add Score
          </Button>
        </div>

        <div className="score-search">
          <div className="search-container">
            <Select
              placeholder="Filter by class"
              allowClear
              style={{ minWidth: 220 }}
              options={classOptions}
              value={filterClassId}
              onChange={(value) => {
                setFilterClassId(value);
                fetchScores(1, pageSize);
              }}
              showSearch
              optionFilterProp="label"
            />
            <Input.Search
              placeholder="Filter by test name"
              allowClear
              style={{ minWidth: 220 }}
              onSearch={(value) => {
                setFilterTestName(value);
                fetchScores(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="score-table"
          columns={columns}
          dataSource={scores}
          rowKey={(record) => record.id || `${record.classId}-${record.studentId}-${record.testName}`}
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
          title={mode === "create" ? "Add Score" : "Update Score"}
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
              mark: 10,
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
              name="testName"
              label="Test Name"
              rules={[{ required: true, message: "Test name is required" }]}
            >
              <Input placeholder="e.g. Midterm, Quiz 1" />
            </Form.Item>

            <Form.Item
              name="mark"
              label="Mark"
              rules={[{ required: true, message: "Mark is required" }]}
            >
              <InputNumber min={0} max={10} step={0.1} style={{ width: "100%" }} />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Scores;
