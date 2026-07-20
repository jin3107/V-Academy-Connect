import { useState } from "react";
import MainLayout from "../../components/MainLayout";
import { Avatar, Button, Form, Input, Tag, message } from "antd";
import { MailOutlined, PhoneOutlined, UserOutlined } from "@ant-design/icons";
import { updateProfileAsync } from "../../apis/accountAPI";
import { useAuth } from "../../contexts";
import { TenantType } from "../../commons";
import type { UpdateProfileRequest } from "../../interfaces";
import "./style.scss";

interface ProfileFormValues {
  email: string;
  phoneNumber: string;
}

const tenantTypeLabel = (type?: TenantType) => {
  if (type === TenantType.Center) return "Center";
  if (type === TenantType.Personal) return "Personal";
  return undefined;
};

const Profile = () => {
  const { user, updateUser } = useAuth();
  const [form] = Form.useForm<ProfileFormValues>();
  const [submitting, setSubmitting] = useState(false);

  const initials = (user?.fullName || user?.email || "?").charAt(0).toUpperCase();

  const onFinish = async (values: ProfileFormValues) => {
    try {
      setSubmitting(true);
      const payload: UpdateProfileRequest = {
        email: values.email,
        phoneNumber: values.phoneNumber,
      };
      const res = await updateProfileAsync(payload);
      if (!res.isSuccess || !res.data) {
        message.error(res.message || "Update failed");
      } else {
        message.success(res.message || "Profile updated");
        updateUser({ email: res.data.email, phoneNumber: res.data.phoneNumber });
      }
    } catch (err) {
      console.error(err);
      message.error("Unexpected error");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <MainLayout>
      <div className="profile-page page">
        <div className="page-header">
          <div className="page-title">
            <h2>Profile</h2>
            <p>Your account information</p>
          </div>
        </div>

        <div className="profile-layout">
          <div className="page-card profile-summary">
            <Avatar size={72} className="profile-avatar">{initials}</Avatar>
            <div className="profile-name">{user?.fullName || user?.userName}</div>
            {user?.email && user.email !== (user?.fullName || user?.userName) && (
              <div className="profile-email">{user.email}</div>
            )}
            <div className="profile-tags">
              {user?.role && <Tag color="blue">{user.role}</Tag>}
              {tenantTypeLabel(user?.tenantType) && <Tag>{tenantTypeLabel(user?.tenantType)}</Tag>}
            </div>
          </div>

          <div className="page-card profile-form-card">
            <h3>Edit profile</h3>
            <Form
              form={form}
              layout="vertical"
              onFinish={onFinish}
              initialValues={{
                email: user?.email,
                phoneNumber: user?.phoneNumber,
              }}
            >
              <Form.Item
                name="email"
                label="Email"
                rules={[
                  { required: true, message: "Email is required" },
                  { type: "email", message: "Please enter a valid email" },
                ]}
              >
                <Input prefix={<MailOutlined />} placeholder="Enter your email" />
              </Form.Item>

              <Form.Item
                name="phoneNumber"
                label="Phone Number"
                rules={[{ required: true, message: "Phone number is required" }]}
              >
                <Input prefix={<PhoneOutlined />} placeholder="Enter your phone number" />
              </Form.Item>

              <Form.Item label="Username">
                <Input prefix={<UserOutlined />} value={user?.userName} disabled />
              </Form.Item>

              <Button type="primary" htmlType="submit" loading={submitting}>
                Save changes
              </Button>
            </Form>
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default Profile;
