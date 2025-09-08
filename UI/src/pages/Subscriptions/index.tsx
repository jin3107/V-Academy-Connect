import MainLayout from '../../components/MainLayout';
import './style.scss'
import { Button, Input, Table, message, Select, Modal, Form, Popconfirm, Descriptions, Spin, InputNumber, } from "antd";
import { PlusOutlined, SearchOutlined, SortAscendingOutlined, SortDescendingOutlined } from "@ant-design/icons";
import type { Filter, SearchRequest, SearchResponse, SubscriptionRequest, SubscriptionResponse, } from "../../interfaces";
import { Typography } from "antd";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";
import { useState, useCallback, useEffect } from 'react';
import { createSubscription, deleteSubscription, getSubscriptionById, searchSubscriptions, updateSubscription } from '../../apis/subscriptionAPI';

const { Option } = Select;
const { Text } = Typography;

type Mode = "create" | "detail" | "update";

interface SubscriptionData {
  id?: string,
  name: string,
  price: number,
  billingCycleName: string, // Fixed typo: billingClycleName -> billingCycleName
  trialDays: number
}

type BackendSubscriptionResponse = SubscriptionResponse & { createBy?: string };

const Subscriptions = () => {
  const [loading, setLoading] = useState(false);
  const [subscriptions, setSubscriptions] = useState<SubscriptionData[]>([]);
  const [searchText, setSearchText] = useState("");
  const [searchField, setSearchField] = useState("Name");
  const [sortField, setSortField] = useState<string | undefined>(undefined);
  const [sortDirection, setSortDirection] = useState<boolean>(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [open, setOpen] = useState(false);
  const [mode, setMode] = useState<Mode>("create");
  const [selectedId, setSelectedId] = useState<string | undefined>(undefined);
  const [submitting, setSubmitting] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);
  const [form] = Form.useForm<SubscriptionResponse>();
  const [detailData, setDetailData] = useState<SubscriptionResponse | null>(null);

  const billingCycle = [
    { label: "None", value: 0 },
    { label: "Monthly", value: 1 },
    { label: "Yearly", value: 2 },
  ]

  const toDayjs = (v: string | Dayjs | undefined): Dayjs | undefined => {
    if (!v) return undefined;
    return typeof v === "string" ? dayjs(v) : v;
  };

  const fmtDate = (v?: string | Dayjs, withTime = false) => {
    if (!v) return "-";
    const d = typeof v === "string" ? dayjs(v) : v;
    return d ? d.format(withTime ? "YYYY-MM-DD HH:mm" : "YYYY-MM-DD") : "-";
  };

  const fmtBillingCycle = (b?: number) => {
    if (b === 0) return "None";
    if (b === 1) return "Monthly";
    if (b === 2) return "Yearly";
    return "-";
  };

  const openCreate = () => {
    setMode("create");
    setSelectedId(undefined);
    setOpen(true);
  };

  const openDetail = (id?: string) => {
    if (!id) return;
    setMode("detail");
    setSelectedId(id);
    setOpen(true);
  };

  const openUpdate = (id?: string) => {
    if (!id) return;
    setMode("update");
    setSelectedId(id);
    setOpen(true);
  };

  const loadDetail = async (id: string) => {
    setDetailLoading(true);
    const response = await getSubscriptionById(id);
    setDetailLoading(false);

    if (!response.isSuccess) {
      message.error(response.message || "Failed to load subscription");
      return;
    }

    const raw = response.data as BackendSubscriptionResponse;
    const mapped: SubscriptionResponse = {
      ...raw,
      createdBy: raw.createdBy ?? raw.createBy,
      createdOn: toDayjs(raw.createdOn),
    };

    setDetailData(mapped);

    if (mode === "update") {
      let billingCycleValue = 0;
      if (mapped.billingCycle) {
        const cycleMap: { [key: string]: number } = {
          "None": 0,
          "Monthly": 1,
          "Yearly": 2
        };
        billingCycleValue = cycleMap[mapped.billingCycle] ?? 0;
      }

      form.setFieldsValue({
        id: mapped.id,
        name: mapped.name,
        price: mapped.price,
        billingCycle: billingCycleValue,
        trialDays: mapped.trialDays,
        createdOn: mapped.createdOn,
        createdBy: mapped.createdBy,
      } as Partial<SubscriptionResponse>);
    }
  };

  const onFinish = async (values: SubscriptionResponse) => {
    try {
      setSubmitting(true);
      
      const common = {
        name: values.name,
        price: values.price,
        billingCycle: typeof values.billingCycle === "number" ? values.billingCycle : Number(values.billingCycle ?? 0),
        trialDays: values.trialDays
      };

      if (mode === "create") {
        const payloadCreate: SubscriptionRequest = {
          ...common,
        } as unknown as SubscriptionRequest;
        
        const response = await createSubscription(payloadCreate);
        if (!response.isSuccess) {
          message.error(response.message || 'Create failed')
        } else {
          message.success(response.message || 'Created')
          setOpen(false);
          fetchSubscriptions(currentPage, pageSize, searchText);
        }
      } else if (mode === "update") {
        if (!selectedId) return message.error('Missing subscription id');
        
        const payloadUpdate = {
          ...common,
          id: selectedId,
        } as unknown as SubscriptionRequest;
        
        const response = await updateSubscription(payloadUpdate);
        if (!response.isSuccess) {
          message.error(response.message || 'Update failed');
        } else {
          message.success(response.message || 'Updated');
          setOpen(false);
          fetchSubscriptions(currentPage, pageSize, searchText);
        }
      }
    } catch (err) {
      console.log(err);
      message.error('Unexpected error');
    } finally {
      setSubmitting(false);
    }
  };

  const fetchSubscriptions = useCallback(
    async (page = 1, pageSizeArg = 10, searchValue = "") => {
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
          filters,
          sortBy: sortField ? {
            fieldName: sortField,
            ascending: sortDirection,
          } : undefined,
          pageIndex: page,
          pageSize: pageSizeArg,
        };

        const response = await searchSubscriptions(searchRequest);
        if (response.isSuccess) {
          const searchResponse = response as unknown as SearchResponse<SubscriptionData>;
          let subscriptionData: SubscriptionData[] = [];
          if (searchResponse.data && searchResponse.data.data) {
            subscriptionData = searchResponse.data.data.map((item: SubscriptionData | { data: SubscriptionData }) =>
              "data" in item ? item.data : item
            );
          }

          setSubscriptions(subscriptionData);
          setCurrentPage(searchResponse.data.currentPage);
          setPageSize(searchResponse.data.rowsPerPage);
          setTotal(searchResponse.data.totalRows);
        } else {
          message.error(response.message || "Failed to fetch subscriptions");
        }
      } catch (error) {
        console.error("Exception error:", error);
        message.error("Failed to fetch subscriptions");
      } finally {
        setLoading(false);
      }
    },
    [searchField, sortField, sortDirection]
  );

  useEffect(() => {
    fetchSubscriptions(currentPage, pageSize, searchText);
  }, [fetchSubscriptions, currentPage, pageSize, searchText])

  const handleSearch = () => {
    fetchSubscriptions(1, pageSize, searchText);
  };

  const handleSortChange = (field: string) => {
    if (sortField === field) {
      setSortDirection(!sortDirection);
    } else {
      setSortField(field);
      setSortDirection(true);
    }
    fetchSubscriptions(currentPage, pageSize, searchText);
  };

  const handleTableChange = (newPagination: { current?: number; pageSize?: number }) => {
    const newPage = newPagination.current || currentPage;
    const newPageSize = newPagination.pageSize || pageSize;
    setCurrentPage(newPage);
    setPageSize(newPageSize);
    fetchSubscriptions(newPage, newPageSize, searchText);
  };

  const renderSortIcon = (field: string) =>
    sortField === field ? (sortDirection ? <SortAscendingOutlined /> : <SortDescendingOutlined />) : null;

  const columns = [
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Name")}>
          Name {renderSortIcon("Name")}
        </div>
      ),
      dataIndex: "name",
      key: "name",
      width: 120,
      ellipsis: true,
      render: (_: unknown, r: SubscriptionData) => (
        <Text ellipsis={{ tooltip: r.name }}>{r.name || "-"}</Text>
      ),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("Price")}>
          Price {renderSortIcon("Price")}
        </div>
      ),
      dataIndex: "price",
      key: "price",
      width: 100,
      ellipsis: true,
      render: (v: number | null | undefined) => (v ?? "-"),
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("BillingCycle")}>
          Billing Cycle {renderSortIcon("BillingCycle")}
        </div>
      ),
      dataIndex: "billingCycleName",
      key: "billingCycleName",
      width: 90,
      ellipsis: true,
    },
    {
      title: (
        <div className="column-header" onClick={() => handleSortChange("TrialDays")}>
          Trial Days {renderSortIcon("TrialDays")}
        </div>
      ),
      dataIndex: "trialDays",
      key: "trialDays",
      width: 80,
      ellipsis: true,
    },
    {
      title: "Actions",
      key: "actions",
      width: 260,
      fixed: "right" as const,
      render: (_: unknown, record: SubscriptionData) => (
        <div className="action-buttons" style={{ whiteSpace: "nowrap" }}>
          <Button type="link" onClick={() => openDetail(record.id)}>
            Detail
          </Button>
          <Button type="link" onClick={() => openUpdate(record.id)}>
            Edit
          </Button>
          <Popconfirm
            title="Delete this subscription?"
            okText="Yes"
            cancelText="No"
            onConfirm={async () => {
              if (!record.id) return;
              const res = await deleteSubscription(record.id);
              if (!res.isSuccess)
                return message.error(res.message || "Delete failed");
              message.success(res.message || "Deleted");
              fetchSubscriptions(currentPage, pageSize, searchText);
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
      <div className='subscription-page'>
        <div className='subscription-header'>
          <h2>Subscription Management</h2>
          <Button type='primary' icon={<PlusOutlined />} onClick={openCreate}>
            Add Subscription
          </Button>
        </div>

        <div className="subscription-search">
          <div className="search-container">
            <Select
              defaultValue="Name"
              style={{ width: 120 }}
              onChange={(value) => setSearchField(value)}
              value={searchField}
            >
              <Option value="Name">Name</Option>
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
          className="subscription-table"
          columns={columns}
          dataSource={subscriptions}
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
          title={
            mode === "create"
              ? "Create Subscription"
              : mode === "update"
              ? "Update Subscription"
              : "Subscription Detail"
          }
          open={open}
          onCancel={() => setOpen(false)}
          footer={
            mode === "detail"
              ? [<Button key="close" onClick={() => setOpen(false)}>Close</Button>]
              : [
                  <Button key="cancel" onClick={() => setOpen(false)}>Cancel</Button>,
                  <Button key="submit" type="primary" loading={submitting} onClick={() => form.submit()}>
                    {mode === "create" ? "Submit" : "Save"}
                  </Button>,
                ]
          }
          destroyOnHidden
          afterOpenChange={async (visible) => {
            if (!visible) return;
            
            if (mode !== "detail") {
              form.resetFields();
            }
            
            if ((mode === "detail" || mode === "update") && selectedId) {
              await loadDetail(selectedId);
            }
          }}
        >
          {mode === "detail" ? (
            <Spin spinning={detailLoading}>
              <Descriptions column={1} bordered size="middle">
                <Descriptions.Item label="Name">{detailData?.name ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Price">{detailData?.price ?? "-"}</Descriptions.Item>
                {typeof detailData?.billingCycle !== "undefined" && (
                  <Descriptions.Item label="Billing Cycle">
                    {fmtBillingCycle(detailData?.billingCycle as number)}
                  </Descriptions.Item>
                )}
                <Descriptions.Item label="Trial Days">{detailData?.trialDays ?? "-"}</Descriptions.Item>
                <Descriptions.Item label="Created On">
                  {fmtDate(detailData?.createdOn as Dayjs | string, true)}
                </Descriptions.Item>
                <Descriptions.Item label="Created By">
                  {detailData?.createdBy ?? "-"}
                </Descriptions.Item>
              </Descriptions>
            </Spin>
          ) : (
            <Form form={form} layout="vertical" onFinish={onFinish} preserve={false}>
              <Form.Item 
                name="name" 
                label="Name" 
                rules={[{ required: true, message: "Name is required"}]}
              >
                <Input prefix={<span className="input-prefix"></span>} />
              </Form.Item>

              <Form.Item 
                name="price" 
                label="Price" 
                rules={[
                  { required: true, message: "Price is required" },
                  { type: "number", min: 0 }
                ]}
              >
                <InputNumber style={{ width: "100%" }} />
              </Form.Item>

              <Form.Item 
                name="trialDays" 
                label="Trial Days" 
                rules={[
                  { required: true, message: "Trial Day is required" },
                  { type: "number", min: 0 }
                ]}
              >
                <InputNumber style={{ width: "100%" }} />
              </Form.Item>

              <Form.Item
                name="billingCycle" 
                label="Billing Cycle" 
                rules={[{ required: true, message: "Billing Cycle is required" }]}
              >
                <Select options={billingCycle} />
              </Form.Item>
            </Form>
          )}
        </Modal>
      </div>
    </MainLayout>
  )
}

export default Subscriptions;