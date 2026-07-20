import { useEffect, useRef, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import type { SendOtpRequest, VerifyOtpRequest } from "../../interfaces";
import { sendOtpAsync, verifyOtpAsync } from "../../apis/accountAPI";
import { Form, Button, message, Input } from "antd";
import './style.scss'
import { AuthFormInput } from "../../components";
import { MailOutlined } from "@ant-design/icons";
import { OtpPurpose } from "../../commons";

const RESEND_COOLDOWN_SECONDS = 60;

type Step = "email" | "otp";

const VerifyEmail = () => {
  const [step, setStep] = useState<Step>("email");
  const [email, setEmail] = useState("");
  const [code, setCode] = useState("");
  const [sending, setSending] = useState(false);
  const [verifying, setVerifying] = useState(false);
  const [cooldown, setCooldown] = useState(0);
  const navigate = useNavigate();
  const timerRef = useRef<ReturnType<typeof setInterval> | undefined>(undefined);

  useEffect(() => {
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, []);

  const startCooldown = () => {
    setCooldown(RESEND_COOLDOWN_SECONDS);
    if (timerRef.current) clearInterval(timerRef.current);
    timerRef.current = setInterval(() => {
      setCooldown((prev) => {
        if (prev <= 1) {
          clearInterval(timerRef.current);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
  };

  const sendCode = async (targetEmail: string) => {
    const request: SendOtpRequest = { email: targetEmail, purpose: OtpPurpose.ChangePassword };
    const response = await sendOtpAsync(request);
    if (!response.isSuccess) {
      message.error(response.message || "Failed to send verification code.");
      return false;
    }
    message.success("Verification code sent to your email.");
    startCooldown();
    return true;
  };

  const onSendCode = async (values: { email: string }) => {
    try {
      setSending(true);
      const ok = await sendCode(values.email);
      if (ok) {
        setEmail(values.email);
        setCode("");
        setStep("otp");
      }
    } catch (error) {
      console.error('Send OTP failed: ', error);
      message.error('Failed to send verification code. Please try again.');
    } finally {
      setSending(false);
    }
  };

  const onResend = async () => {
    if (cooldown > 0) return;
    try {
      setSending(true);
      await sendCode(email);
    } catch (error) {
      console.error('Resend OTP failed: ', error);
      message.error('Failed to resend verification code. Please try again.');
    } finally {
      setSending(false);
    }
  };

  const onVerify = async () => {
    if (code.length !== 6) {
      message.error('Please enter the 6-digit code.');
      return;
    }

    try {
      setVerifying(true);
      const request: VerifyOtpRequest = { email, code, purpose: OtpPurpose.ChangePassword };
      const response = await verifyOtpAsync(request);
      if (!response.isSuccess) {
        message.error(response.message || 'Invalid or expired code.');
        return;
      }
      navigate('/change-password', { state: { email } });
    } catch (error) {
      console.error('Verify OTP failed: ', error);
      message.error('Verification failed. Please try again.');
    } finally {
      setVerifying(false);
    }
  };

  return (
    <div className="verify-email-container">
      <div className="verify-email-form-wrapper">
        <div className="verify-email-logo">
          <div className="verify-email-circle">
            <MailOutlined />
          </div>
        </div>

        {step === "email" ? (
          <>
            <h1 className="verify-email-title">Forgot Password</h1>
            <p className="verify-email-subtitle">Enter your email and we'll send you a verification code.</p>

            <Form
              name="sendOtp"
              onFinish={onSendCode}
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

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  loading={sending}
                  className="verify-button"
                >
                  Send Code
                </Button>
              </Form.Item>
            </Form>

            <div className="back-link">
              <Link to="/login">Back to Login</Link>
            </div>
          </>
        ) : (
          <>
            <h1 className="verify-email-title">Enter Verification Code</h1>
            <p className="verify-email-subtitle">
              We sent a 6-digit code to <strong>{email}</strong>. It expires in 5 minutes.
            </p>

            <div className="otp-input-wrapper">
              <Input.OTP
                length={6}
                value={code}
                onChange={setCode}
                autoFocus
              />
            </div>

            <Button
              type="primary"
              loading={verifying}
              disabled={code.length !== 6}
              onClick={onVerify}
              className="verify-button"
            >
              Verify Code
            </Button>

            <div className="resend-row">
              {cooldown > 0 ? (
                <span>Resend code in {cooldown}s</span>
              ) : (
                <button type="button" className="resend-link" onClick={onResend} disabled={sending}>
                  Resend code
                </button>
              )}
            </div>

            <div className="back-link">
              <button type="button" className="link-button" onClick={() => setStep("email")}>
                Use a different email
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  )
}

export default VerifyEmail
