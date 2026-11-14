import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import {
  StatusTransitionDialogComponent,
  StatusTransitionDialogData
} from './status-transition-dialog.component';

describe('StatusTransitionDialogComponent', () => {
  let dialogRef: jasmine.SpyObj<
    MatDialogRef<StatusTransitionDialogComponent>
  >;

  const data: StatusTransitionDialogData = {
    allowedStatuses: ['Triaged', 'Closed']
  };

  beforeEach(() => {
    dialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);

    TestBed.configureTestingModule({
      imports: [StatusTransitionDialogComponent],
      providers: [
        provideNoopAnimations(),
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: MatDialogRef, useValue: dialogRef }
      ]
    });
  });

  it('starts with the first allowed status selected', () => {
    const fixture = TestBed.createComponent(StatusTransitionDialogComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.form.controls.newStatus.value).toBe(
      'Triaged'
    );
  });

  it('requires a reason of at least five characters', () => {
    const fixture = TestBed.createComponent(StatusTransitionDialogComponent);
    const component = fixture.componentInstance;

    component.form.controls.reason.setValue('abc');
    expect(component.form.invalid).toBeTrue();

    component.form.controls.reason.setValue('valid reason');
    expect(component.form.valid).toBeTrue();
  });

  it('closes with the typed form value on submit', () => {
    const fixture = TestBed.createComponent(StatusTransitionDialogComponent);
    const component = fixture.componentInstance;
    component.form.controls.reason.setValue('Triaged by on-call');

    component.submit();

    expect(dialogRef.close).toHaveBeenCalledWith({
      newStatus: 'Triaged',
      reason: 'Triaged by on-call'
    });
  });

  it('closes without a value when cancelled', () => {
    const fixture = TestBed.createComponent(StatusTransitionDialogComponent);

    fixture.componentInstance.cancel();

    expect(dialogRef.close).toHaveBeenCalledWith();
  });
});
