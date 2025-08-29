import axios from "../configs/axios";
import type { StaffRequest, StaffResponse } from "../interfaces";

export const getAllStaffs = async () => {
  return await axios
    .get<{ isSuccess: boolean; data: StaffResponse["data"][]; message: string }>("/staffs")
    .then(res => res.data);
};

export const createStaff = async (data: StaffRequest) => {
  return await axios
    .post<StaffResponse>("/staffs", data)
    .then((res) => res.data);
};

export const updateStaff = async (data: StaffRequest) => {
  return await axios
    .put<StaffResponse>("/staffs", data)
    .then((res) => res.data);
};

export const getStaffById = async (id: string) => {
  return await axios.get(`/staffs/${id}`).then((res) => res.data);
};

export const deleteStaff = async (id: string) => {
  return await axios.delete(`/staffs/${id}`).then((res) => res.data);
};
