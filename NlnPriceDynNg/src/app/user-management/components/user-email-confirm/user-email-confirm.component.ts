import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { ErrorHandlerService } from '../../../shared/services/error-handler.service';
import { MessageService } from '../../services/message.service';

@Component({
  selector: 'app-user-email-confirm',
  templateUrl: './user-email-confirm.component.html',
  styleUrls: ['./user-email-confirm.component.css']
})
export class UserEmailConfirmComponent implements OnInit, OnDestroy {

  isLoading: boolean;
  error: string;

  constructor(private http: HttpClient, private userService: UserService,
              private route: ActivatedRoute, private router: Router,
              private errorHandler: ErrorHandlerService,
              private messageService: MessageService) {}

  ngOnInit() {
    this.isLoading = true;
    const userId = this.route.snapshot.queryParamMap.get('id');
    const confirmationCode = this.route.snapshot.queryParamMap.get('code');
    this.userService.confirmEmail(userId, confirmationCode)
      .pipe(untilDestroyed(this)).subscribe(
      data => {
        this.messageService.add('Учетная запись активирована');
        this.router.navigate(['/login']);
      },
      error => {
        this.isLoading = false;
        this.errorHandler.handleError(error);
        this.error = 'Не удалось подтвердить адрес электронной почты, указанный при регистрации';
      });
  }

  ngOnDestroy(): void {

  }

}
