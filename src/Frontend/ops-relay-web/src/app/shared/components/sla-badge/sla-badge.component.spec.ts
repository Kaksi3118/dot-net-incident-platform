import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SlaBadgeComponent } from './sla-badge.component';

describe('SlaBadgeComponent', () => {
  let fixture: ComponentFixture<SlaBadgeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SlaBadgeComponent]
    });

    fixture = TestBed.createComponent(SlaBadgeComponent);
  });

  it('uses green when more than half of the SLA remains', () => {
    setWindow(150, -60);

    expect(fixture.componentInstance.badgeClass()).toBe('sla-green');
    expect(fixture.componentInstance.badgeText()).toContain('left');
  });

  it('uses amber between ten and fifty percent remaining', () => {
    setWindow(60, -120);

    expect(fixture.componentInstance.badgeClass()).toBe('sla-amber');
  });

  it('uses red below ten percent remaining', () => {
    setWindow(10, -120);

    expect(fixture.componentInstance.badgeClass()).toBe('sla-red');
  });

  it('marks breached deadlines', () => {
    setWindow(-25, -145);

    expect(fixture.componentInstance.badgeClass()).toBe('sla-breached');
    expect(fixture.componentInstance.badgeText()).toContain('Breached by');
  });

  function setWindow(
    deadlineOffsetMinutes: number,
    reportedOffsetMinutes: number
  ): void {
    const now = Date.now();
    fixture.componentRef.setInput(
      'reportedAt',
      new Date(now + reportedOffsetMinutes * 60000).toISOString()
    );
    fixture.componentRef.setInput(
      'slaDeadline',
      new Date(now + deadlineOffsetMinutes * 60000).toISOString()
    );
    fixture.detectChanges();
  }
});
