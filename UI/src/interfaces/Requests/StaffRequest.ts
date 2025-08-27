export enum Gender {
  Male = 0,
  Female = 1
};

export default interface StaffRequest {
  name: string,
  dateOfBirth: Date,
  age: number,
  gender: Gender,
  address: string,
  email: string,
  phoneNumber: string,
  password: string,
  role: string,
};