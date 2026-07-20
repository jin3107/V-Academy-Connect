import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, DatePicker, Popconfirm, Rate, Input, Tag,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined,
} from "@ant-design/icons";
import {
  createLessonEvaluation, searchLessonEvaluations, updateLessonEvaluation, deleteLessonEvaluation,
} from "../../apis/lessonEvaluationAPI";
import { searchClasses } from "../../apis/classAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import type {
  Filter, SearchRequest, SearchResponse,
  LessonEvaluationRequest, LessonEvaluationResponse, ClassResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

interface LessonEvaluationFormValues {
  classId: string;
  studentId: string;
  date: Dayjs;
  rating?: number;
  comment?: string;
}

const COMMENT_TEMPLATES = [
  "Học sinh tích cực phát biểu trong buổi học.",
  "Học sinh hoàn thành tốt bài tập được giao.",
  "Học sinh có thái độ học tập tốt.",
  "Học sinh tiến bộ rõ rệt so với buổi trước.",
  "Học sinh còn thiếu tập trung, cần chú ý hơn.",
  "Học sinh chưa hoàn thành bài tập về nhà.",
  "Học sinh cần luyện tập thêm ở nhà.",
  "Học sinh đi trễ, cần đi học đúng giờ hơn.",
];

const { TextArea } = Input;

const LessonEvaluations = () => {
  const [loading, setLoading] = useState(false);
  const [lessonEvaluations, setLessonEvaluations] = useState<LessonEvaluationResponse[]>([]);
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
  const [form] = Form.useForm<LessonEvaluationFormValues>();
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

  const openUpdate = (record: LessonEvaluationResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setFormClassId(record.classId);
    setOpen(true);
  };

  const applyCommentTemplate = (template: string) => {
    const current = form.getFieldValue("comment") as string | undefined;
    const next = current && current.trim().length > 0 ? `${current} ${template}` : template;
    form.setFieldValue("comment", next);
  };

  const onFinish = async (values: LessonEvaluationFormValues) => {
    try {
      setSubmitting(true);
      const payload: LessonEvaluationRequest = {
        id: mode === "update" ? selectedId : undefined,
        classId: values.classId,
        studentId: values.studentId,
        date: values.date.toISOString(),
        rating: values.rating,
        comment: values.comment,
      } as LessonEvaluationRequest;

      const res = mode === "create" ? await createLessonEvaluation(payload) : await updateLessonEvaluation(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchLessonEvaluations(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const fetchLessonEvaluations = useCallback(
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

        const response = await searchLessonEvaluations(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<LessonEvaluationResponse>;
          setLessonEvaluations(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch lesson evaluations");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch lesson evaluations");
      } finally {
        setLoading(false);
      }
    },
    [filterClassId, filterDate, sortField, sortDirection]
  );

  useEffect(() => {
    fetchLessonEvaluations(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchLessonEvaluations]);

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
    fetchLessonEvaluations(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: "Class",
      dataIndex: "className",
      key: "className",
      render: (_: unknown, r: LessonEvaluationResponse) => r.className || "-",
    },
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: LessonEvaluationResponse) => r.studentName || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Date")}>
          Date {renderSortIcon("Date")}
        </div>
      ),
      dataIndex: "date",
      key: "date",
      render: (_: unknown, r: LessonEvaluationResponse) => (r.date ? dayjs(r.date).format("YYYY-MM-DD") : "-"),
    },
    {
      title: "Rating",
      dataIndex: "rate",
      key: "rate",
      render: (_: unknown, r: LessonEvaluationResponse) => <Rate disabled value={r.rate || 0} />,
    },
    {
      title: "Comment",
      dataIndex: "comment",
      key: "comment",
      ellipsis: true,
      render: (_: unknown, r: LessonEvaluationResponse) => r.comment || "-",
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: LessonEvaluationResponse) => (
        <div className="action-buttons">
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this lesson evaluation?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteLessonEvaluation(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchLessonEvaluations(currentPage, pageSize);
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
      <div className="lesson-evaluation-page">
        <div className="lesson-evaluation-header">
          <h2>Lesson Evaluation</h2>
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
            Add Evaluation
          </Button>
        </div>

        <div className="lesson-evaluation-search">
          <div className="search-container">
            <Select
              placeholder="Filter by class"
              allowClear
              style={{ minWidth: 220 }}
              options={classOptions}
              value={filterClassId}
              onChange={(value) => {
                setFilterClassId(value);
                fetchLessonEvaluations(1, pageSize);
              }}
              showSearch
              optionFilterProp="label"
            />
            <DatePicker
              placeholder="Filter by date"
              value={filterDate}
              onChange={(value) => {
                setFilterDate(value);
                fetchLessonEvaluations(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="lesson-evaluation-table"
          columns={columns}
          dataSource={lessonEvaluations}
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
          title={mode === "create" ? "Add Evaluation" : "Update Evaluation"}
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
              rating: 5,
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

            <Form.Item name="rating" label="Rating">
              <Rate />
            </Form.Item>

            <Form.Item label="Comment templates" className="comment-templates">
              <div className="comment-template-list">
                {COMMENT_TEMPLATES.map((template) => (
                  <Tag
                    key={template}
                    className="comment-template-tag"
                    onClick={() => applyCommentTemplate(template)}
                  >
                    {template}
                  </Tag>
                ))}
              </div>
            </Form.Item>

            <Form.Item name="comment" label="Comment">
              <TextArea rows={4} placeholder="Click a template above or type your own comment" />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default LessonEvaluations;
