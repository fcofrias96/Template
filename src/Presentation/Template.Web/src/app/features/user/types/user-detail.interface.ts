import { Role } from "@features/role/types/role.interface";

export interface UserDetail {
  id: number;
  name: string;
  resources: ResourceUser[];
  roles: Role[];
  email: string;
  firstName: string;
  lastName: string;
  isVerified: boolean;
}

export interface ResourceUser {
  id: number;
  resource: string;
  permissions: Permission[];
}

export interface Permission {
  id: number;
  name: string;
}
