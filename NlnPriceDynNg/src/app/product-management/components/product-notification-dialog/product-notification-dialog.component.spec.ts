import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductNotificationDialogComponent } from './product-notification-dialog.component';

describe('ProductNotificationDialogComponent', () => {
  let component: ProductNotificationDialogComponent;
  let fixture: ComponentFixture<ProductNotificationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductNotificationDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductNotificationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
