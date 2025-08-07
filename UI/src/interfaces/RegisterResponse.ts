export default interface RegisterResponse {
  data: {
    name: string,
    email: string,
    phoneNumber: string,
    tenantName: string,
    subscriptionPlanId: string,
    token: string
  },
  message: string,
  isSuccess: boolean,
}