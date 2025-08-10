import axios from '../configs/axios';
import type { LogInRequest, LogInResponse, RegisterRequest, RegisterResponse } from '../interfaces';

export const logInAsync = async (data: LogInRequest) => {
  return await axios.post<LogInResponse>('/accounts/login', data).then(res => res.data);
}

export const registerAsync = async (data: RegisterRequest) => {
  return await axios.post<RegisterResponse>('/accounts/register', data).then(res => res.data);
}
