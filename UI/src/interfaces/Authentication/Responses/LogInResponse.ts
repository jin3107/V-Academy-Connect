export default interface LogInResponse {
  email: string;
  token: string;
  phoneNumber: string;
  userId: string;  
  tenantId: string;
  role: string;
};