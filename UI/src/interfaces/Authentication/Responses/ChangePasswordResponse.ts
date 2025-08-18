export default interface ChangePasswordRepsonse {
  data: {
    email: string,
    phoneNumber: string,
    newPassword: string,
    confirmNewPassword: string;
  },
  isSuccess: boolean,
  message: string,
}