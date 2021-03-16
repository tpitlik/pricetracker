import { Component, OnInit, OnDestroy } from '@angular/core';
import {MatProgressButtonOptions} from 'mat-progress-buttons';
import {HttpClient} from '@angular/common/http';
import {UserService} from '../../services/user.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ErrorHandlerService} from '../../../shared/services/error-handler.service';
import {MessageService} from '../../services/message.service';
import {untilDestroyed} from 'ngx-take-until-destroy';

@Component({
  selector: 'app-user-email-confirm-token',
  templateUrl: './user-email-confirm-token.component.html',
  styleUrls: ['./user-email-confirm-token.component.css']
})
export class UserEmailConfirmTokenComponent implements OnInit, OnDestroy {

  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'Отправить',
    spinnerSize: 19,
    raised: true,
    stroked: false,
    buttonColor: 'primary',
    spinnerColor: 'accent',
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
  };

  userId: string;
  error: string;

  constructor(private http: HttpClient, private userService: UserService,
              private route: ActivatedRoute, private router: Router,
              private errorHandler: ErrorHandlerService,
              private messageService: MessageService) { }

  ngOnInit() {
    this.userId = this.route.snapshot.queryParamMap.get('id');
    console.log(this.userId);
  }

  ngOnDestroy(): void {
  }

  resendConfirmationEmail(): void {
    console.log(this.userId);
    this.btnOpts.active = true;
    this.userService.sendConfirmationEmail(this.userId)
      .pipe(untilDestroyed(this)).subscribe(
      data => {
        this.messageService.add('Ссылка для активации учетной записи повторно отправлена на ваш адрес электронной почты.');
      },
      error => {
        this.errorHandler.handleError(error);
        this.error = 'Произошла ошибка. Повторите процесс регистрации.';
      });
    this.btnOpts.active = false;
  }
}
