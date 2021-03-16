import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserEmailConfirmComponent } from './user-email-confirm.component';

describe('UserEmailConfirmComponent', () => {
  let component: UserEmailConfirmComponent;
  let fixture: ComponentFixture<UserEmailConfirmComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserEmailConfirmComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserEmailConfirmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
