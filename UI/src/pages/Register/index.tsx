import { useState } from "react";
import { Form, Button, message } from 'antd';
import { useNavigate, Link } from 'react-router-dom';
import type { RegisterRequest } from "../../interfaces";
import { registerAsync } from "../../apis/accountAPI";
import { AuthFormInput } from "../../components";
import './style.scss'
import { LockOutlined, MailOutlined, PhoneOutlined, TeamOutlined, UserOutlined } from "@ant-design/icons";

const Register = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const onFinish = async (values: RegisterRequest) => {
    if (values.password !== values.confirmPassword) {
      message.error('Passwords do not match!');
      return;
    }

    try {
      setLoading(true);
      await registerAsync(values);
      message.success('Registration successful!');
      navigate('/login');
    } catch (error) {
      console.log('Registration failed: ', error);
      message.error('Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-container">
      <div className="register-form-wrapper">
        <div className="register-logo">
          <div className="logo-circle"></div>
        </div>

        <h1 className="register-title">Create an account</h1>
        <p className="register-subtitle">Please enter your details to register.</p>

        <Form
          name="register"
          onFinish={onFinish}
          layout="vertical"
          className="register-form"
        >
          <div className="form-row">
            <AuthFormInput 
              label="Name"
              name="name"
              type="text"
              message="Please enter your name!"
              placeholder="Enter your name"
              prefix={<span className="input-prefix"><UserOutlined /></span>}
              required
            />

            <AuthFormInput
              label="Email"
              name="email"
              type="email"
              placeholder="Enter your email"
              required
              prefix={<span className="input-prefix"><MailOutlined /></span>}
              rules={[
                { type: 'email', message: 'Please enter a valid email!' }
              ]}
            />
          </div>

          <div className="form-row">
            <AuthFormInput 
              label="Phone Number"
              name="phoneNumber"
              type="text"
              placeholder="Enter your phone number"
              prefix={<span className="input-prefix"><PhoneOutlined /></span>}
              rules={[
                { type: 'email', message: 'Please enter a valid phone number!' },
                { len: 10, message: 'Phone number must be exactly 10 digits!' },
                { pattern: /^[0-9]+$/, message: 'Phone number must contain only digits!' }
              ]}
              required
            />

            <AuthFormInput 
              label="Tenant Name"
              name="tenantName"
              type="text"
              placeholder="Enter your tenant name"
              prefix={<span className="input-prefix"><TeamOutlined /></span>}
              required
            />
          </div>

          <div className="form-row">
              <AuthFormInput
                label="Password"
                name="password"
                type="password"
                placeholder="Enter your password"
                prefix={<span className="input-prefix"><LockOutlined /></span>}
                required
              />

              <AuthFormInput
                label="Confirm Password"
                name="confirmPassword"
                type="password"
                placeholder="Confirm your password"
                prefix={<span className="input-prefix"><LockOutlined /></span>}
                required
              />
          </div>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} className="register-button">
              Register
            </Button>
          </Form.Item>

          <div className="register-divider">
            <span>OR</span>
          </div>

          <div className="login-link">
            Already have an account?
            <Link to="/login"> Login</Link>
          </div>
        </Form>
      </div>
    </div>
  )
}

export default Register;