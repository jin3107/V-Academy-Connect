import axios from '../configs/axios';
import type { AppResponse } from '../helpers';
import type { ChangePasswordRequest, ChangePasswordResponse, LogInRequest, LogInResponse, RegisterRequest, RegisterResponse, VerifyEmailRequest, VerifyEmailResponse } from '../interfaces';

export const logInAsync = async (data: LogInRequest) : Promise<AppResponse<LogInResponse>> => {
  const response = await axios.post<AppResponse<LogInResponse>>('/accounts/login', data);
  return response.data;
}

export const registerAsync = async (data: RegisterRequest) : Promise<AppResponse<RegisterResponse>> => {
  const response = await axios.post<AppResponse<RegisterResponse>>('/accounts/register', data);
  return response.data;
}

export const logOutAsync = async () => {
  return await axios.post('/accounts/logout').then(res => res.data);
}

export const getMeAsync = async (): Promise<LogInResponse> => {
  const response = await axios.get<LogInResponse>('/accounts/me');
  return response.data;
}

export const verifyEmailAsync = async (data: VerifyEmailRequest) : Promise<AppResponse<VerifyEmailResponse>> => {
  const response = await axios.post<AppResponse<VerifyEmailResponse>>('/accounts/verify-email', data);
  return response.data;
}

export const changePasswordAsync = async (data: ChangePasswordRequest) : Promise<AppResponse<ChangePasswordResponse>> => {
  const response = await axios.post<AppResponse<ChangePasswordResponse>>('/accounts/change-password', data);
  return response.data;
}