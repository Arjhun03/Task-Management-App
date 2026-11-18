export enum TaskStatus { Pending = 0, InProgress = 1, Completed = 2 }
export enum TaskPriority { Low = 0, Medium = 1, High = 2 }
export interface TaskItem { taskId: number; title: string; description: string | null; status: TaskStatus; priority: TaskPriority; dueDate: string | null; categoryId: number | null; categoryName: string | null; createdAt: string; updatedAt: string | null; }
export interface TaskRequest { title: string; description: string | null; status: TaskStatus; priority: TaskPriority; dueDate: string | null; categoryId: number | null; }
