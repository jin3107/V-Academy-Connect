export default interface RegisterResponse {
  data: {
    name: string,
    email: string,
    phoneNumber: string,
    tenantName: string,
    subscriptionPlanId: string,
    token: string;
  },
  isSuccess: boolean,
  message: string,
}