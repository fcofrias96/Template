export interface PermissionDetail {
  id: number;
  resource: string;
  resourceId: number;
  actionType: string;
  actionTypeId: number;
  roles: string[];
  name: string;
  description: string;
}
