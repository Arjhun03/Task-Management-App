import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category';
import { ConfirmationDialog } from '../../../../shared/components/confirmation-dialog/confirmation-dialog';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, ConfirmationDialog],
  templateUrl: './categories.html',
  styleUrl: './categories.css',
})
export class Categories {
  private readonly service = inject(CategoryService);
  private readonly fb = inject(FormBuilder);
  categories = signal<Category[]>([]);
  editing = signal<number | null>(null);
  loading = signal(false);
  error = signal('');
  deleting = signal<Category | null>(null);
  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    description: ['', Validators.maxLength(250)],
  });

  constructor() {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.service
      .getAll()
      .subscribe({
        next: (data) => this.categories.set(data),
        error: () => this.error.set('Could not load categories.'),
        complete: () => this.loading.set(false),
      });
  }
  edit(category: Category): void {
    this.editing.set(category.categoryId);
    this.form.setValue({ name: category.name, description: category.description ?? '' });
  }
  cancel(): void {
    this.editing.set(null);
    this.form.reset();
  }
  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const payload = { name: value.name.trim(), description: value.description.trim() || null };
    const id = this.editing();
    this.loading.set(true);
    this.error.set('');
    const onFailure = (e: { error?: { message?: string } }) => {
      this.error.set(e.error?.message ?? 'Could not save category.');
      this.loading.set(false);
    };
    if (id === null) {
      this.service.create(payload).subscribe({
        next: () => {
          this.cancel();
          this.load();
        },
        error: onFailure,
      });
    } else {
      this.service.update(id, payload).subscribe({
        next: () => {
          this.cancel();
          this.load();
        },
        error: onFailure,
      });
    }
  }
  remove(category: Category): void {
    this.deleting.set(category);
  }
  confirmDelete(): void {
    const category = this.deleting();
    if (!category) return;
    this.service.delete(category.categoryId).subscribe({
      next: () => {
        this.deleting.set(null);
        this.load();
      },
      error: (e) => {
        this.deleting.set(null);
        this.error.set(e.error?.message ?? 'Could not delete category.');
      },
    });
  }
}
