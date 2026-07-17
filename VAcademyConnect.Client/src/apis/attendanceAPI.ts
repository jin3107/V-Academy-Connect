import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { AttendanceRequest, AttendanceResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createAttendance = async (data: AttendanceRequest) : Promise<AppResponse<AttendanceResponse>> => {
  const response = await axios.post<AppResponse<AttendanceResponse>>("/attendances", data);
  return response.data;
};

export const updateAttendance = async (data: AttendanceRequest) : Promise<AppResponse<AttendanceResponse>> => {
  const response = await axios.put<AppResponse<AttendanceResponse>>("/attendances", data);
  return response.data;
};

export const getAttendanceById = async (id: string) : Promise<AppResponse<AttendanceResponse>> => {
  const response = await axios.get<AppResponse<AttendanceResponse>>(`/attendances/${id}`);
  return response.data;
};

export const deleteAttendance = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/attendances/${id}`);
  return response.data;
};

export const searchAttendances = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<AttendanceResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<AttendanceResponse>>>("/attendances/search", searchRequest);
  return response.data;
};
