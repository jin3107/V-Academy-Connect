export default interface StaffResponse {
  data: {
    id?: string,
    name: string,
    email: string,
    phoneNumber: string,
    role: string
  },
  isSuccess: boolean,
  message: string,
}