import axios from '../configs/axios';
import type { AppResponse } from '../helpers';
import type { RoleResponse } from '../interfaces/Responses';

export const getAllRoles = async () : Promise<AppResponse<RoleResponse[]>> => {
  const response = await axios.get<AppResponse<RoleResponse[]>>('/accounts/roles');
  return response.data;
};