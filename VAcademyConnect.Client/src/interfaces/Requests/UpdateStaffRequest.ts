import dayjs from "dayjs";
import type { Gender } from "../../commons/enums";

export default interface UpdateStaffRequest {
  dateOfBirth: dayjs.Dayjs,
  gender: Gender,
  address: string,
  email: string,
  phoneNumber: string,
  password: string,
  role: string,
};