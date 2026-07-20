import { useEffect, useState, useCallback } from "react";
import MainLayout from "../../components/MainLayout";
import {
  Button, Table, message, Select,
  Modal, Form, InputNumber, Input, Popconfirm, DatePicker, List, Empty,
} from "antd";
import {
  PlusOutlined, SortAscendingOutlined, SortDescendingOutlined,
  CheckCircleOutlined, CheckCircleFilled,
} from "@ant-design/icons";
import {
  createPayment, searchPayments, updatePayment, deletePayment,
} from "../../apis/paymentAPI";
import { createPaymentCheck, searchPaymentChecks } from "../../apis/paymentCheckAPI";
import { searchClassStudents } from "../../apis/classStudentAPI";
import { useAuth } from "../../contexts";
import type {
  Filter, SearchRequest, SearchResponse,
  PaymentRequest, PaymentResponse, PaymentCheckResponse, ClassStudentResponse,
} from "../../interfaces";
import "./style.scss";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";

type Mode = "create" | "update";

interface PaymentFormValues {
  studentId: string;
  amount: number;
  note?: string;
  transferTime?: Dayjs;
  qrInfo?: string;
}

const Payments = () => {
  const { user } = useAuth();

  const [loading, setLoading] = useState(false);
  const [payments, setPayments] = useState<PaymentResponse[]>([]);
  const [filterStudentId, setFilterStudentId] = useState<string | undefined>(undefined);
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [form] = Form.useForm<PaymentFormValues>();

  const [verifyOpen, setVerifyOpen] = useState(false);
  const [verifyPayment, setVerifyPayment] = useState<PaymentResponse | undefined>(undefined);
  const [verifyLoading, setVerifyLoading] = useState(false);
  const [verifySubmitting, setVerifySubmitting] = useState(false);
  const [paymentChecks, setPaymentChecks] = useState<PaymentCheckResponse[]>([]);
  const [verifyNote, setVerifyNote] = useState("");

  const [studentOptions, setStudentOptions] = useState<{ label: string; value: string }[]>([]);
  const [studentsLoading, setStudentsLoading] = useState(false);

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

  useEffect(() => {
    loadStudents();
  }, [loadStudents]);

  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
  };

  const openUpdate = (record: PaymentResponse) => {
    setMode("update");
    setSelectedId(record.id);
    setOpen(true);
    setTimeout(() => {
      form.setFieldsValue({
        studentId: record.studentId,
        amount: record.amount,
        note: record.note,
        transferTime: record.transferTime ? dayjs(record.transferTime) : undefined,
        qrInfo: record.qrInfo,
      });
    }, 0);
  };

  const onFinish = async (values: PaymentFormValues) => {
    try {
      setSubmitting(true);
      const payload: PaymentRequest = {
        id: mode === "update" ? selectedId : undefined,
        studentId: values.studentId,
        amount: values.amount,
        note: values.note,
        transferTime: values.transferTime ? values.transferTime.toISOString() : undefined,
        qrInfo: values.qrInfo,
      } as PaymentRequest;

      const res = mode === "create" ? await createPayment(payload) : await updatePayment(payload);
      if (!res.isSuccess) {
        message.error(res.message || `${mode === "create" ? "Create" : "Update"} failed`);
      } else {
        message.success(res.message || (mode === "create" ? "Created" : "Updated"));
        setOpen(false);
        fetchPayments(currentPage, pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  const loadPaymentChecks = useCallback(async (paymentId: string) => {
    setVerifyLoading(true);
    const response = await searchPaymentChecks({
      filters: [{ fieldName: "PaymentId", value: paymentId }],
      sortBy: { fieldName: "CheckTime", ascending: false },
      pageIndex: 1,
      pageSize: 50,
    });
    setVerifyLoading(false);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to load payment checks");
      setPaymentChecks([]);
      return;
    }
    setPaymentChecks(response.data.data.data || []);
  }, []);

  const openVerify = (record: PaymentResponse) => {
    setVerifyPayment(record);
    setVerifyNote("");
    setVerifyOpen(true);
    if (record.id) loadPaymentChecks(record.id);
  };

  const onConfirmPayment = async () => {
    if (!verifyPayment?.id) return;
    try {
      setVerifySubmitting(true);
      const res = await createPaymentCheck({
        paymentId: verifyPayment.id,
        checkedBy: user?.userId || "",
        checkTime: new Date().toISOString(),
        note: verifyNote || undefined,
      });
      if (!res.isSuccess) {
        message.error(res.message || "Confirm failed");
      } else {
        message.success(res.message || "Payment confirmed");
        setVerifyNote("");
        loadPaymentChecks(verifyPayment.id);
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setVerifySubmitting(false);
    }
  };

  const fetchPayments = useCallback(
    async (page = 1, pageSizeArg = 10) => {
      try {
        setLoading(true);
        const filters: Filter[] = [];
        if (filterStudentId) filters.push({ fieldName: "StudentId", value: filterStudentId });

        const searchRequest: SearchRequest = {
          filters,
          sortBy: sortField ? { fieldName: sortField, ascending: sortDirection } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchPayments(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<PaymentResponse>;
          setPayments(searchResponse.data.data || []);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch payments");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch payments");
      } finally {
        setLoading(false);
      }
    },
    [filterStudentId, sortField, sortDirection]
  );

  useEffect(() => {
    fetchPayments(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchPayments]);

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
    fetchPayments(newPage, newPageSize);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: "Student",
      dataIndex: "studentName",
      key: "studentName",
      render: (_: unknown, r: PaymentResponse) => r.studentName || "-",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Amount")}>
          Amount {renderSortIcon("Amount")}
        </div>
      ),
      dataIndex: "amount",
      key: "amount",
      render: (_: unknown, r: PaymentResponse) => r.amount.toLocaleString("vi-VN") + " đ",
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("TransferTime")}>
          Transfer Time {renderSortIcon("TransferTime")}
        </div>
      ),
      dataIndex: "transferTime",
      key: "transferTime",
      render: (_: unknown, r: PaymentResponse) => (r.transferTime ? dayjs(r.transferTime).format("YYYY-MM-DD HH:mm") : "-"),
    },
    {
      title: "Note",
      dataIndex: "note",
      key: "note",
      render: (_: unknown, r: PaymentResponse) => r.note || "-",
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right" as const,
      render: (_: unknown, record: PaymentResponse) => (
        <div className="action-buttons">
          <Button type="link" icon={<CheckCircleOutlined />} onClick={() => openVerify(record)}>
            Verify
          </Button>
          <Button type="link" onClick={() => openUpdate(record)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this payment record?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deletePayment(record.id);
              if (!res.isSuccess) return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchPayments(currentPage, pageSize);
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
      <div className="payment-page">
        <div className="payment-header">
          <h2>Payments</h2>
          <div className="header-actions">
            <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
              Add Payment
            </Button>
          </div>
        </div>

        <div className="payment-search">
          <div className="search-container">
            <Select
              options={studentOptions}
              loading={studentsLoading}
              placeholder="Filter by student"
              allowClear
              showSearch
              optionFilterProp="label"
              style={{ width: 260 }}
              value={filterStudentId}
              onChange={(value) => {
                setFilterStudentId(value);
                fetchPayments(1, pageSize);
              }}
            />
          </div>
        </div>

        <Table
          className="payment-table"
          columns={columns}
          dataSource={payments}
          rowKey={(record) => record.id || `${record.studentId}-${record.transferTime}`}
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
          title={mode === "create" ? "Add Payment" : "Update Payment"}
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

            <Form.Item
              name="transferTime"
              label="Transfer Time"
              rules={[{ required: true, message: "Transfer time is required" }]}
            >
              <DatePicker showTime style={{ width: "100%" }} />
            </Form.Item>

            <Form.Item name="qrInfo" label="QR Info">
              <Input placeholder="Bank transfer reference / QR content" />
            </Form.Item>

            <Form.Item name="note" label="Note">
              <Input.TextArea rows={2} />
            </Form.Item>
          </Form>
        </Modal>

        <Modal
          title={`Verify Payment${verifyPayment?.studentName ? " — " + verifyPayment.studentName : ""}`}
          open={verifyOpen}
          onCancel={() => setVerifyOpen(false)}
          footer={[
            <Button key="close" onClick={() => setVerifyOpen(false)}>Close</Button>,
          ]}
          destroyOnHidden
        >
          <div className="verify-summary">
            <div><strong>Amount:</strong> {verifyPayment?.amount.toLocaleString("vi-VN")} đ</div>
            <div><strong>Transfer Time:</strong> {verifyPayment?.transferTime ? dayjs(verifyPayment.transferTime).format("YYYY-MM-DD HH:mm") : "-"}</div>
            {verifyPayment?.qrInfo && <div><strong>QR Info:</strong> {verifyPayment.qrInfo}</div>}
          </div>

          <List
            className="payment-check-list"
            loading={verifyLoading}
            locale={{ emptyText: <Empty description="No verification yet" /> }}
            dataSource={paymentChecks}
            renderItem={(item) => (
              <List.Item>
                <List.Item.Meta
                  avatar={<CheckCircleFilled style={{ color: "#52c41a" }} />}
                  title={item.checkedByName || item.checkedBy || "Unknown"}
                  description={
                    <>
                      <div>{item.checkTime ? dayjs(item.checkTime).format("YYYY-MM-DD HH:mm") : "-"}</div>
                      {item.note && <div>{item.note}</div>}
                    </>
                  }
                />
              </List.Item>
            )}
          />

          <div className="verify-form">
            <Input.TextArea
              rows={2}
              placeholder="Note (optional)"
              value={verifyNote}
              onChange={(e) => setVerifyNote(e.target.value)}
            />
            <Button
              type="primary"
              icon={<CheckCircleOutlined />}
              loading={verifySubmitting}
              onClick={onConfirmPayment}
            >
              Confirm Payment
            </Button>
          </div>
        </Modal>
      </div>
    </MainLayout>
  );
};

export default Payments;
