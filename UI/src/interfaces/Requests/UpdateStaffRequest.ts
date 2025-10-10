import dayjs from "dayjs";

export enum Gender {
  Male = 0,
  Female = 1
};

export default interface UpdateStaffRequest {
  dateOfBirth: dayjs.Dayjs,
  gender: Gender,
  address: string,
  email: string,
  phoneNumber: string,
  password: string,
  role: string,
};