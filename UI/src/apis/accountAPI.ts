import axios from '../configs/axios';
import type { ChangePasswordRequest, ChangePasswordResponse, LogInRequest, LogInResponse, RegisterRequest, RegisterResponse, VerifyEmailRequest, VerifyEmailResponse } from '../interfaces';

export const logInAsync = async (data: LogInRequest) => {
  return await axios.post<LogInResponse>('/accounts/login', data).then(res => res.data);
}

export const registerAsync = async (data: RegisterRequest) => {
  return await axios.post<RegisterResponse>('/accounts/register', data).then(res => res.data);
}

export const logOutAsync = async () => {
  return await axios.post('/accounts/logout').then(res => res.data);
}

export const verifyEmailAsync = async (data: VerifyEmailRequest) => {
  return await axios.post<VerifyEmailResponse>('/accounts/verify-email', data).then(res => res.data);
}

export const changePasswordAsync = async (data: ChangePasswordRequest) => {
  return await axios.post<ChangePasswordResponse>('/accounts/change-password', data).then(res => res.data);
}