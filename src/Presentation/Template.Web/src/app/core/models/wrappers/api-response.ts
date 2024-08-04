export interface ApiResponse<T> {
  statusCode: number;
  message?: string;
  data?: T;
  errors?: { [key: string]: string[] };
  success: boolean;
}

