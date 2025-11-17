export interface ApiResponse<T> {
  statusCode: number;
  message: string;
  data: T;
  errors?: Record<string, string[]>;
  success: boolean;
}
