export enum Gender {
  Male = 0,
  Female = 1,
};

class Staff {
  public id: string | null;
  public dateOfBirth: Date;
  public age: number;
  public gender: Gender;
  public address: string;
  public email: string;
  public password: string;
  public phoneNumber: string;
  public role: string;

  public constructor(id: string, dateOfBirth: Date, age: number, gender: Gender, address: string, email: string, password: string, phoneNumber: string, role: string) {
    this.id = id;
    this.dateOfBirth = dateOfBirth;
    this.age = age;
    this.gender = gender;
    this.address = address;
    this.email = email;
    this.password = password;
    this.phoneNumber = phoneNumber;
    this.role = role;
  }
}

export default Staff;