import { Form, Button, Checkbox, message } from "antd";
import { useState } from "react";
import { useNavigate, Link } from "react-router";
import type { LogInRequest } from "../../interfaces";
import { logInAsync } from "../../apis/accountAPI";
import './style.scss'
import { AuthFormInput } from "../../components";
import { LockOutlined, UserOutlined } from "@ant-design/icons";


const LogIn = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const onFinish = async (values: LogInRequest) => {
    try {
      setLoading(true);
      await logInAsync(values);
      message.success('Login successful!');
      navigate('/home');
    } catch (error) {
      console.error('Login failed: ', error);
      message.error('Login failed. PLease check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-form-wrapper">
        <div className="login-logo">
          <div className="login-circle"></div>
        </div>
        <h1 className="login-title">Welcome back</h1>
        <p className="login-subtitle">Please enter your details to login.</p>

        <Form
          name="login"
          initialValues={{ rememberMe: false }}
          onFinish={onFinish}
          layout="vertical"
          className="login-form"
        >
          <AuthFormInput
            label="Email"
            name="email"
            required
            message="Please enter your email!"
            type="email"
            placeholder="Enter your email"
            prefix={<span className="input-prefix"><UserOutlined /></span>}
            rules={[
              { type: "email", message: "Please enter a valid email!" }
            ]}
          />

          <AuthFormInput
            label="Password"
            name="password"
            required
            message="Please enter your password!"
            type="password"
            placeholder="Enter your password"
            prefix={<span className="input-prefix"><LockOutlined /></span>}
          />

          <Form.Item>
            <div className="login-options">
              <Form.Item name="rememberMe" valuePropName="checked" noStyle>
                <Checkbox>Remember me</Checkbox>
              </Form.Item>
              <Link to="/verify-email" className="forgot-password">
                Forgot Password?
              </Link>
            </div>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} className="login-button">
              Login
            </Button>
          </Form.Item>

          <div className="login-divider">
            <span>OR</span>
          </div>

          <div className="register-link">
            Don't have an account?
            <Link to="/register"> Register</Link>
          </div>
        </Form>
      </div>
    </div>
  )
}

export default LogIn;