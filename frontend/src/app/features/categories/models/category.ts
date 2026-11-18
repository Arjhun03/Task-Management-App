export interface Category {
  categoryId: number;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface CategoryRequest {
  name: string;
  description: string | null;
}
