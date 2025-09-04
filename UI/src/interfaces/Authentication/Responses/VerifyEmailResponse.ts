export default interface VerifyEmailResponse {
  data: {
  email: string,
  phoneNumber: string
  },
  isSuccess: boolean,
  message: string,
}