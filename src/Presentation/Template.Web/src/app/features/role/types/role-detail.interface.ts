export interface RoleDetail {
  id: number;
  name: string;
  resources: ResourceRole[];
}

export interface ResourceRole {
  id: number;
  name: string;
  permissions: Permission[];
}

export interface Permission {
  id: number;
  name: string;
}
