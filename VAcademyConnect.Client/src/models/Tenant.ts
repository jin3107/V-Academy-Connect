export enum Status {
  Active = 0,
  Suspended = 1,
  Inactive = 2,
}

class Tenant {
  public id: string | null;
  public name: string;
  public contactEmail: string;
  public contactPhone: string;
  public status: Status;

  public constructor(id: string, name: string, contactEmail: string, contactPhone: string, status: Status) {
    this.id = id;
    this.name = name;
    this.contactEmail = contactEmail;
    this.contactPhone = contactPhone;
    this.status = status;
  }
}

export default Tenant;