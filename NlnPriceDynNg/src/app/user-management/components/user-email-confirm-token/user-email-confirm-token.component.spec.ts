import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserEmailConfirmTokenComponent } from './user-email-confirm-token.component';

describe('UserEmailConfirmTokenComponent', () => {
  let component: UserEmailConfirmTokenComponent;
  let fixture: ComponentFixture<UserEmailConfirmTokenComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserEmailConfirmTokenComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserEmailConfirmTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
