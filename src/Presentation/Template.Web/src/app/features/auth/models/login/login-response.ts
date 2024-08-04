export interface LoginResponse {
  id: string;
  userName: string;
  email: string;
  roles: string[];
  accessToken: string;
}
