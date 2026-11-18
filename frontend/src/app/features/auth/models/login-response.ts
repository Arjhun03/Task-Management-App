export interface LoginResponse {
  message: string;
  token: string;

  user: {
    userId: number;
    fullName: string;
    email: string;
  };
}
