import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable()
export class ErrorHandlerService {
  error: any;

  constructor(private router: Router) { }

  public handleError(error: HttpErrorResponse) {
    switch (error.status) {
      case 0:
        this.handleUnknownError(error);
        break;
      case 500:
        this.handle500Error(error);
        break;
      case 403:
        this.handle403Error(error);
        break;
      case 404:
        this.handle404Error(error);
        break;
      default:
        this.handleOtherError(error);
        break;
    }
  }

  private handleUnknownError(error: HttpErrorResponse){
    this.createErrorMessage(error);
    this.router.navigate(['/unknown']);
  }

  private handle500Error(error: HttpErrorResponse) {
    this.createErrorMessage(error);
    this.router.navigate(['/500']);
  }

  private handle404Error(error: HttpErrorResponse) {
    this.createErrorMessage(error);
    this.router.navigate(['/404']);
  }

  private handleOtherError(error: HttpErrorResponse) {
    this.createErrorMessage(error);
  }

  private handle403Error(error: HttpErrorResponse) {
    this.router.navigate(['/403']);
  }

  private createErrorMessage(error: HttpErrorResponse) {
    this.error = error.error;
  }
}
