import { Form, Input } from "antd";
import type { Rule } from "antd/es/form";

interface FormInputProps {
  label: string;
  name: string;
  message?: string;
  rules?: Rule[];
  required?: boolean;
  placeholder?: string;
  type?: string
}

const FormInput = ({ label, name, message, rules = [], required = false, placeholder, type }: FormInputProps) => {
  const defaultRules: Rule[] = required
    ? [{ required: true, message: message || `${label} is required` }]
    : [];

  return (
    <Form.Item label={label} name={name} rules={[...defaultRules, ...rules]}>
      {type === "password" ? (
        <Input.Password placeholder={placeholder} />
      ) : (
        <Input type={type} placeholder={placeholder} />
      )}
    </Form.Item>
  );
};

export default FormInput;
