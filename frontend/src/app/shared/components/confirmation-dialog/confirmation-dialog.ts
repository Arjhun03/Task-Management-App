import { Component, input, output } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-confirmation-dialog',
  template: `
    <div class="backdrop" role="presentation" (click)="cancelled.emit()">
      <section class="dialog" role="alertdialog" aria-modal="true" aria-labelledby="confirm-title" (click)="$event.stopPropagation()">
        <h2 id="confirm-title">Confirm deletion</h2>
        <p>{{ message() }}</p>
        <div class="actions"><button type="button" class="secondary" (click)="cancelled.emit()">Cancel</button><button type="button" class="danger" (click)="confirmed.emit()">Delete</button></div>
      </section>
    </div>
  `,
  styles: [`.backdrop{position:fixed;inset:0;background:#0f172a88;display:grid;place-items:center;z-index:10;padding:1rem}.dialog{max-width:420px;width:100%;background:white;border-radius:14px;padding:1.5rem;box-shadow:0 20px 50px #0f172a33}.actions{display:flex;justify-content:flex-end;gap:.6rem;margin-top:1.5rem}button{border:0;border-radius:8px;padding:.65rem 1rem;cursor:pointer}.secondary{background:#e2e8f0}.danger{background:#dc2626;color:white}`]
})
export class ConfirmationDialog {
  message = input.required<string>();
  confirmed = output<void>();
  cancelled = output<void>();
}
