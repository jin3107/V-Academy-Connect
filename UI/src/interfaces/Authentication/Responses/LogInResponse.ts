export default interface LogInResponse {
  data: {
    email: string;
    token: string;
    phoneNumber: string;
    userId: string;  
    tenantId: string;
    role: string;
  },
  isSuccess: boolean,
  message: string,
};
