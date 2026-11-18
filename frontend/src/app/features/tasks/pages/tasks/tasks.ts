import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../../categories/services/category.service';
import { Category } from '../../../categories/models/category';
import { TaskItem, TaskPriority, TaskStatus } from '../../models/task';
import { TaskService } from '../../services/task.service';
import { ConfirmationDialog } from '../../../../shared/components/confirmation-dialog/confirmation-dialog';

@Component({ standalone: true, imports: [CommonModule, ReactiveFormsModule, ConfirmationDialog], templateUrl: './tasks.html', styleUrl: './tasks.css' })
export class Tasks {
  private readonly fb = inject(FormBuilder); private readonly tasksApi = inject(TaskService); private readonly categoriesApi = inject(CategoryService);
  tasks = signal<TaskItem[]>([]); categories = signal<Category[]>([]); loading = signal(false); error = signal(''); editing = signal<TaskItem | null>(null);
  deleting = signal<TaskItem | null>(null);
  filters = this.fb.nonNullable.group({ search: [''], status: [''], priority: [''] });
  form = this.fb.nonNullable.group({ title: ['', [Validators.required, Validators.maxLength(150)]], description: [''], priority: [TaskPriority.Medium], status: [TaskStatus.Pending], dueDate: [''], categoryId: [''] });
  completed = computed(() => this.tasks().filter(task => task.status === TaskStatus.Completed).length);
  constructor() { this.categoriesApi.getAll().subscribe({ next: values => this.categories.set(values) }); this.load(); }
  load(): void { this.loading.set(true); this.error.set(''); this.tasksApi.getAll(this.filters.getRawValue()).subscribe({ next: items => this.tasks.set(items), error: e => { this.error.set(e.error?.message ?? 'Could not load tasks.'); this.loading.set(false); }, complete: () => this.loading.set(false) }); }
  edit(task: TaskItem): void { this.editing.set(task); this.form.setValue({ title: task.title, description: task.description ?? '', priority: task.priority, status: task.status, dueDate: task.dueDate ? task.dueDate.slice(0, 10) : '', categoryId: task.categoryId?.toString() ?? '' }); }
  reset(): void { this.editing.set(null); this.form.reset({ title: '', description: '', priority: TaskPriority.Medium, status: TaskStatus.Pending, dueDate: '', categoryId: '' }); }
  save(): void { if (this.form.invalid) { this.form.markAllAsTouched(); return; } const value = this.form.getRawValue(); const payload = { title: value.title.trim(), description: value.description.trim() || null, priority: value.priority, dueDate: value.dueDate ? new Date(`${value.dueDate}T23:59:59`).toISOString() : null, categoryId: value.categoryId ? Number(value.categoryId) : null }; const editing = this.editing(); this.loading.set(true); const call = editing ? this.tasksApi.update(editing.taskId, { ...payload, status: value.status }) : this.tasksApi.create(payload); call.subscribe({ next: () => { this.reset(); this.load(); }, error: e => { this.error.set(e.error?.message ?? 'Could not save task.'); this.loading.set(false); } }); }
  setStatus(task: TaskItem, status: TaskStatus): void { this.tasksApi.updateStatus(task.taskId, status).subscribe({ next: () => this.load(), error: e => this.error.set(e.error?.message ?? 'Could not update status.') }); }
  remove(task: TaskItem): void { this.deleting.set(task); }
  confirmDelete(): void { const task = this.deleting(); if (!task) return; this.tasksApi.delete(task.taskId).subscribe({ next: () => { this.deleting.set(null); this.load(); }, error: e => { this.deleting.set(null); this.error.set(e.error?.message ?? 'Could not delete task.'); } }); }
  status(value: string): TaskStatus { return Number(value) as TaskStatus; }
  priorityName(priority: TaskPriority): string { return TaskPriority[priority]; }
  statusName(status: TaskStatus): string { return TaskStatus[status]; }
}
