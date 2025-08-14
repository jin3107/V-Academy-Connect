export default interface ChangePasswordRequest {
  email: string,
  phoneNumber: string,
  newPassword: string,
  confirmNewPassword: string
}