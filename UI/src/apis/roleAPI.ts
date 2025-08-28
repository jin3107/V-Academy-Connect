import axios from '../configs/axios';
import type { RoleResponse } from '../interfaces/Responses';

export const getRoles = async () => {
  return await axios.get<RoleResponse>('accounts/roles').then(res => res.data);
};