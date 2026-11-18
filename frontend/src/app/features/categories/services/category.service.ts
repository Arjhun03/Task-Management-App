import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Category, CategoryRequest } from '../models/category';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/category`;

  getAll(): Observable<Category[]> { return this.http.get<Category[]>(this.url); }
  create(request: CategoryRequest): Observable<Category> { return this.http.post<Category>(this.url, request); }
  update(id: number, request: CategoryRequest): Observable<void> { return this.http.put<void>(`${this.url}/${id}`, request); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
