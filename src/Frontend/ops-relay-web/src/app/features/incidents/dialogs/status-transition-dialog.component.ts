import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { IncidentStatus } from '../../../core/models/incident.model';

export interface StatusTransitionDialogData {
  allowedStatuses: IncidentStatus[];
}

export interface StatusTransitionResult {
  newStatus: IncidentStatus;
  reason: string;
}

@Component({
  selector: 'app-status-transition-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './status-transition-dialog.component.html',
  styleUrl: './status-transition-dialog.component.scss'
})
export class StatusTransitionDialogComponent {
  readonly data = inject<StatusTransitionDialogData>(MAT_DIALOG_DATA);

  private readonly dialogRef =
    inject<MatDialogRef<StatusTransitionDialogComponent, StatusTransitionResult>>(
      MatDialogRef
    );

  readonly form = new FormGroup<{
    newStatus: FormControl<IncidentStatus>;
    reason: FormControl<string>;
  }>({
    newStatus: new FormControl<IncidentStatus>(this.data.allowedStatuses[0], {
      nonNullable: true,
      validators: [Validators.required]
    }),
    reason: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(5)]
    })
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
