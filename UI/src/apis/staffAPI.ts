import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { CreateStaffRequest, UpdateStaffRequest, StaffResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createStaff = async (data: CreateStaffRequest) : Promise<AppResponse<StaffResponse>> => {
  const response = await axios.post<AppResponse<StaffResponse>>("/staffs", data);
  return response.data;
};

export const updateStaff = async (data: UpdateStaffRequest) : Promise<AppResponse<StaffResponse>> => {
  const response = await axios.put<AppResponse<StaffResponse>>("/staffs", data);
  return response.data;
};

export const getStaffById = async (id: string) : Promise<AppResponse<StaffResponse>> => {
  const response = await axios.get<AppResponse<StaffResponse>>(`/staffs/${id}`);
  return response.data;
};

export const deleteStaff = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/staffs/${id}`);
  return response.data;
};

export const searchStaffs = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<StaffResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<StaffResponse>>>("/staffs/search", searchRequest);
  return response.data;
};
