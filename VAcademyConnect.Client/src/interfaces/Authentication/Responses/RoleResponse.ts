export default interface Role {
  id: string;
  name: string;
}

export default interface RoleResponse {
  data: Role[];
  isSuccess: boolean;
  message: string;
}