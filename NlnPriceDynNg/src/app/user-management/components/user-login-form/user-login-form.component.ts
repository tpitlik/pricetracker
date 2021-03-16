import { Component, OnDestroy, OnInit } from '@angular/core';
import { UserCredentials } from '../../models/user-credentials';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { MessageService } from '../../services/message.service';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { ErrorHandlerService } from '../../../shared/services/error-handler.service';
import {DomSanitizer} from '@angular/platform-browser';

@Component({
  selector: 'app-user-login-form',
  templateUrl: './user-login-form.component.html',
  styleUrls: ['./user-login-form.component.css']
})
export class UserLoginFormComponent implements OnInit, OnDestroy {
  userInfo = new UserCredentials('', '', '', '');
  errors: string[];
  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'Войти',
    spinnerSize: 19,
    raised: true,
    stroked: false,
    buttonColor: 'primary',
    spinnerColor: 'accent',
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
  };
  private userSignedIn: boolean;

  constructor(private router: Router,  private authenticationService: AuthenticationService,
              private messageService: MessageService, private errorHandler: ErrorHandlerService) {

  }

  ngOnInit() {
    // this.authenticationService.logout();

    this.authenticationService.isSignedIn()
      .pipe(untilDestroyed(this))
      .subscribe(res => this.userSignedIn = res);

    if (this.userSignedIn) {
      this.router.navigate(['/products']);
    }
  }

  onSubmit() {
    this.messageService.clear();
    this.btnOpts.active = true;
    this.authenticationService.login(this.userInfo.UserName, this.userInfo.Password)
      .pipe(untilDestroyed(this))
      .subscribe(
        data => {
          this.router.navigate(['/products']);
          this.messageService.clear();
        },
        error => {
          this.errorHandler.handleError(error);
          this.errors = [];
          for (const key in error.error) {
            this.errors.push(error.error[key][0]);
            this.btnOpts.active = false;
          }
        });

  }

  ngOnDestroy(): void {
  }
}
