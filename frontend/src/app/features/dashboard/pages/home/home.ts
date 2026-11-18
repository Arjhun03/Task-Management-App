import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { Auth } from '../../../../core/services/auth';
import { TaskService } from '../../../tasks/services/task.service';
import { TaskItem, TaskStatus } from '../../../tasks/models/task';

@Component({
  imports: [CommonModule],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  private auth = inject(Auth);
  private tasksApi = inject(TaskService);
  tasks = signal<TaskItem[]>([]);
  loading = signal(true);
  error = signal('');
  completed = computed(() => this.tasks().filter(task => task.status === TaskStatus.Completed).length);
  pending = computed(() => this.tasks().filter(task => task.status === TaskStatus.Pending).length);
  inProgress = computed(() => this.tasks().filter(task => task.status === TaskStatus.InProgress).length);
  overdue = computed(() => this.tasks().filter(task => task.status !== TaskStatus.Completed && task.dueDate && new Date(task.dueDate) < new Date()).length);
  upcoming = computed(() => this.tasks().filter(task => task.status !== TaskStatus.Completed && task.dueDate && new Date(task.dueDate) >= new Date()).sort((a, b) => a.dueDate!.localeCompare(b.dueDate!)).slice(0, 5));

  constructor() { this.tasksApi.getAll().subscribe({ next: tasks => this.tasks.set(tasks), error: () => { this.error.set('Could not load dashboard tasks.'); this.loading.set(false); }, complete: () => this.loading.set(false) }); }

  logout(): void {
    this.auth.logout();
  }
}
