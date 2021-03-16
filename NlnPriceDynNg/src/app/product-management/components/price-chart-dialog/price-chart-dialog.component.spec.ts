import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceChartDialogComponent } from './price-chart-dialog.component';

describe('PriceChartDialogComponent', () => {
  let component: PriceChartDialogComponent;
  let fixture: ComponentFixture<PriceChartDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PriceChartDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PriceChartDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
