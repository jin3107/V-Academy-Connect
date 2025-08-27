export default interface StaffResponse {
  data: {
    name: string,
    email: string,
    phoneNumber: string,
    role: string
  },
  isSuccess: boolean,
  message: string,
}