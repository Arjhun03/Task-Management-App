import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TaskItem, TaskRequest, TaskStatus } from '../models/task';

export interface TaskFilters { search?: string; status?: string; priority?: string; categoryId?: number | null; }
@Injectable({ providedIn: 'root' })
export class TaskService {
  private http = inject(HttpClient); private readonly url = `${environment.apiUrl}/tasks`;
  getAll(filters: TaskFilters = {}): Observable<TaskItem[]> {
    let params = new HttpParams();
    if (filters.search) params = params.set('search', filters.search);
    if (filters.status !== undefined && filters.status !== '') params = params.set('status', filters.status);
    if (filters.priority !== undefined && filters.priority !== '') params = params.set('priority', filters.priority);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId);
    return this.http.get<TaskItem[]>(this.url, { params });
  }
  create(value: Omit<TaskRequest, 'status'>): Observable<TaskItem> { return this.http.post<TaskItem>(this.url, value); }
  update(id: number, value: TaskRequest): Observable<TaskItem> { return this.http.put<TaskItem>(`${this.url}/${id}`, value); }
  updateStatus(id: number, status: TaskStatus): Observable<TaskItem> { return this.http.patch<TaskItem>(`${this.url}/${id}/status`, { status }); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
