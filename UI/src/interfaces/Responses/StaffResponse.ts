import dayjs from "dayjs";

export enum Gender {
  Male = 0,
  Female = 1
};

export default interface StaffResponse {
  id?: string,
  name: string,
  email: string,
  phoneNumber: string,
  role: string,
  gender?: Gender,
  dateOfBirth?: dayjs.Dayjs,
  address?: string,

  createdOn?: dayjs.Dayjs,
  createdBy?: string,
  password?: string
}