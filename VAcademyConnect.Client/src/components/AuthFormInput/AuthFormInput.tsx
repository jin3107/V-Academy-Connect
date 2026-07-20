import { Form, Input } from "antd";
import type { Rule } from "antd/es/form";

interface AuthFormInputProps {
  label: string;
  name: string;
  type: 'email' | 'password' | 'text';
  placeholder?: string;
  required?: boolean;
  message?: string;
  rules?: Rule[];
  prefix?: React.ReactNode;
  readonly?: boolean;
}

const AuthFormInput = ({ 
  label, 
  name, 
  type, 
  placeholder, 
  required = false, 
  message,
  rules = [],
  prefix,
  readonly = false
}: AuthFormInputProps) => {
  const validationRules: Rule[] = [];

  if (required) {
    validationRules.push({
      required: true,
      message: message || `Please enter your ${label.toLowerCase()}!`
    });
  }

  validationRules.push(...rules);

  return (
    <Form.Item label={label} name={name} rules={validationRules}>
      {type === 'password' ? (
        <Input.Password placeholder={placeholder} prefix={prefix} readOnly={readonly} />
      ) : (
        <Input type={type} placeholder={placeholder} prefix={prefix} readOnly={readonly} />
      )}
    </Form.Item>
  );
};

export default AuthFormInput;