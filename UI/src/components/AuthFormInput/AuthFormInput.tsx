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
  // Tạo rules cho validation
  const validationRules: Rule[] = [];
  
  // Nếu required thì thêm rule required
  if (required) {
    validationRules.push({
      required: true,
      message: message || `Please enter your ${label.toLowerCase()}!`
    });
  }
  
  // Thêm các rules custom
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