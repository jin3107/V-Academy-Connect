import dayjs from "dayjs";
import type { Gender } from "../../commons/enums";

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