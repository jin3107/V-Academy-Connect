import { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import type { VerifyEmailRequest } from "../../interfaces";
import { verifyEmailAsync } from "../../apis/accountAPI";
import { Form, Button, message } from "antd"
import './style.scss'
import { AuthFormInput } from "../../components";
import { MailOutlined, PhoneOutlined } from "@ant-design/icons";


const VerifyEmail = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const onFinish = async (values: VerifyEmailRequest) => {
    try {
      setLoading(true);
      await verifyEmailAsync(values);
      navigate('/change-password', { 
        state: { 
          email: values.email, 
          phoneNumber: values.phoneNumber 
        } 
      });
    } catch (error) {
      console.log('Verify email failed: ', error);
      message.error('Verify email failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="verify-email-container">
      <div className="verify-email-form-wrapper">
        <div className="verify-email-logo">
          <div className="verify-email-circle">
            <i className="fa-solid fa-envelope"></i>
          </div>
        </div>
        <h1 className="verify-email-title">Verify Email</h1>

        <Form
          name="verifyEmail"
          onFinish={onFinish}
          layout="vertical"
          className="verify-email-form"
        >
          <AuthFormInput
            label="Email"
            name="email"
            type="email"
            required
            message="Please enter your email!"
            rules={[
              { type: "email", message: "Please enter a valid email!" }
            ]}
            placeholder="Enter your email"
            prefix={<span className="input-prefix"><MailOutlined /></span>}
          />

          <AuthFormInput
            label="Phone Number"
            name="phoneNumber"
            type="text"
            required
            message="Please enter your phone number!"
            rules={[
              { len: 10, message: "Phone number must be exactly 10 digits!" },
              { pattern: /^[0-9]+$/, message: "Phone number must contain only digits!" }
            ]}
            placeholder="Enter your phone number"
            prefix={<span className="input-prefix"><PhoneOutlined /></span>}
          />

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="verify-button"
            >
              Verify
            </Button>
          </Form.Item>

          <div className="back-link">
            <Link to="/login">Back</Link>
          </div>
        </Form>

      </div>
    </div>
  )
}

export default VerifyEmail 