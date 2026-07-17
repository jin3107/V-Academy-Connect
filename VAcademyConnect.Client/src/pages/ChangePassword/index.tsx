import { useState } from 'react';
import './style.scss';
import { useNavigate, Link, useLocation } from "react-router-dom";
import type { ChangePasswordRequest } from '../../interfaces';
import { changePasswordAsync } from '../../apis/accountAPI';
import { Form, message, Button } from 'antd';
import { AuthFormInput } from '../../components';
import { LockOutlined, MailOutlined, PhoneOutlined } from '@ant-design/icons';



const ChangePassword = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  // Lấy state từ /verify-email (cụ thể là email, phoneNumber)
  const { email, phoneNumber } = location.state || {};

  const onFinish = async (values: ChangePasswordRequest) => {
    try {
      setLoading(true);
      await changePasswordAsync(values);
      navigate('/login');
    } catch (error) {
      console.log('Change password failed: ', error);
      message.error('Change password failed. Please check your password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="change-password-container">
      <div className='change-password-form-wrapper'>
        <div className='change-password-logo'>
          <div className='change-password-circle'>
            <i className='fa-solid fa-envelope'></i>
          </div>
        </div>
        <h1 className='change-password-title'>Change Password</h1>

        <Form
          name='changePassword'
          onFinish={onFinish}
          layout='vertical'
          className='change-password-form'
          initialValues={{ email, phoneNumber }}
        >
          <AuthFormInput 
            label='Email'
            name='email'
            type='email'
            prefix={<span className='input-prefix'><MailOutlined /></span>}
            readonly
          />

          <AuthFormInput 
            type='text'
            name='phoneNumber'
            label='Phone Number'
            required
            message='Please enter your phone number!'
            prefix={<span className='input-prefix'><PhoneOutlined /></span>}
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
              Verify
            </Button>
          </Form.Item>

          <div className="back-link">
            <Link to="/verify-email">Back</Link>
          </div>
          {/* Dùng tạm */}
          {/* Sau này có thể dùng Redux hoặc Context nếu muốn lưu email, phoneNumber khi Back hay refresh page */}
        </Form>
      </div>
    </div>
  )
}

export default ChangePassword;