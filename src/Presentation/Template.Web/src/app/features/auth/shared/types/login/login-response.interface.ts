export interface LoginResponse {
  id: string;
  userName: string;
  email: string;
  roles: string[];
  resources: Resource[];
  accessToken: string;
}

export interface Resource {
  id: number;
  resource: string;
  permissions: Permission[];
}

export interface Permission {
  id: number;
  name: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
}
