export default interface RegisterResponse {
  name: string,
  email: string,
  phoneNumber: string,
  tenantName: string,
  subscriptionPlanId: string,
  token: string;
}