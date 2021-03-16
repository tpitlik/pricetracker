import {Component, OnDestroy, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { UserCredentials } from '../../models/user-credentials';
import { UserService } from '../../services/user.service';
import { MessageService} from '../../services/message.service';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { ErrorHandlerService } from '../../../shared/services/error-handler.service';

@Component({
  selector: 'app-user-register-form',
  templateUrl: './user-register-form.component.html',
  styleUrls: ['./user-register-form.component.css']
})
export class UserRegisterFormComponent implements OnInit, OnDestroy {

  userInfo = new UserCredentials('', '', '', '');
  errors: string[];
  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'Зарегистрироваться',
    spinnerSize: 19,
    raised: true,
    stroked: false,
    buttonColor: 'primary',
    spinnerColor: 'accent',
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
  };

  constructor(private userService: UserService, private router: Router, private messageService: MessageService, private errorHandler: ErrorHandlerService) { }

  ngOnInit() {
    this.messageService.clear();
  }

  onSubmit() {
    this.btnOpts.active = true;
    this.userService.create(this.userInfo)
      .pipe(untilDestroyed(this))
       .subscribe(
         data => {
           this.messageService.add('На указанный адрес электронной почты отправлено письмо со ссылкой для завершения регистрации и активации учетной записи.');
           this.router.navigate(['/login']);

        },
        error => {
         this.errorHandler.handleError(error);
         this.errors = [];
         for (const key in error.error) {
           this.errors.push(error.error[key][0]);
           this.btnOpts.active = false;
         }
       }
     );
  }

  ngOnDestroy(): void {
  }

}
