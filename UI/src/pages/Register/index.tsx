import { useState } from "react";
import { Form, Button, message } from 'antd';
import { useNavigate, Link } from 'react-router-dom';
import type { RegisterRequest } from "../../interfaces";
import { registerAsync } from "../../apis/accountAPI";
import { AuthFormInput } from "../../components";
import './style.scss'

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
              required
            />

            <AuthFormInput
              label="Email"
              name="email"
              type="email"
              placeholder="Enter your email"
              required
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
              rules={[
                { type: 'email', message: 'Please enter a valid phone number!' }
              ]}
              required
            />

            <AuthFormInput 
              label="Tenant Name"
              name="tenantName"
              type="text"
              placeholder="Enter your tenant name"
              required
            />
          </div>

          <div className="form-row">
              <AuthFormInput
                label="Password"
                name="password"
                type="password"
                placeholder="Enter your password"
                required
              />

              <AuthFormInput
                label="Confirm Password"
                name="confirmPassword"
                type="password"
                placeholder="Confirm your password"
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
            <Link to="/login">Already have an account? Login</Link>
          </div>
        </Form>
      </div>
    </div>
  )
}

export default Register;