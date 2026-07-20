import { useEffect, useState } from 'react';
import './style.scss';
import { useNavigate, Link, useLocation } from "react-router-dom";
import type { ChangePasswordRequest } from '../../interfaces';
import { changePasswordAsync } from '../../apis/accountAPI';
import { Form, message, Button } from 'antd';
import { AuthFormInput } from '../../components';
import { LockOutlined, MailOutlined } from '@ant-design/icons';

const ChangePassword = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  // Lấy email từ /verify-email sau khi OTP đã được xác thực
  const { email } = (location.state as { email?: string }) || {};

  useEffect(() => {
    // Không cho vào thẳng trang này nếu chưa qua bước xác thực OTP
    if (!email) {
      navigate('/verify-email', { replace: true });
    }
  }, [email, navigate]);

  const onFinish = async (values: Omit<ChangePasswordRequest, 'email'>) => {
    if (!email) return;

    try {
      setLoading(true);
      const response = await changePasswordAsync({ ...values, email });
      if (!response.isSuccess) {
        message.error(response.message || 'Change password failed.');
        return;
      }
      message.success('Password changed successfully. Please log in.');
      navigate('/login');
    } catch (error) {
      console.error('Change password failed: ', error);
      message.error('Change password failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (!email) return null;

  return (
    <div className="change-password-container">
      <div className='change-password-form-wrapper'>
        <div className='change-password-logo'>
          <div className='change-password-circle'>
            <LockOutlined />
          </div>
        </div>
        <h1 className='change-password-title'>Change Password</h1>
        <p className='change-password-subtitle'>Set a new password for your account.</p>

        <Form
          name='changePassword'
          onFinish={onFinish}
          layout='vertical'
          className='change-password-form'
          initialValues={{ email }}
        >
          <AuthFormInput
            label='Email'
            name='email'
            type='email'
            prefix={<span className='input-prefix'><MailOutlined /></span>}
            readonly
          />

          <AuthFormInput
            type='password'
            name='newPassword'
            label='New Password'
            required
            message='Please enter your password!'
            placeholder='Enter your password'
            prefix={<span className='input-prefix'><LockOutlined /></span>}
          />

          <AuthFormInput
            type='password'
            name='confirmNewPassword'
            label='Confirm New Password'
            required
            message='Please enter your password!'
            placeholder='Enter your password'
            prefix={<span className='input-prefix'><LockOutlined /></span>}
          />

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="change-password-button"
            >
              Change Password
            </Button>
          </Form.Item>

          <div className="back-link">
            <Link to="/verify-email">Back</Link>
          </div>
        </Form>
      </div>
    </div>
  )
}

export default ChangePassword;
