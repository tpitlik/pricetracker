import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';
import {ProductNotification} from '../../models/productnotification';
import {MatProgressButtonOptions} from 'mat-progress-buttons';
import {untilDestroyed} from 'ngx-take-until-destroy';
import {HttpClient} from '@angular/common/http';
import {ProductService} from '../../services/product.service';
import {ErrorHandlerService} from '../../../shared/services/error-handler.service';

@Component({
  selector: 'app-product-notification-dialog',
  templateUrl: './product-notification-dialog.component.html',
  styleUrls: ['./product-notification-dialog.component.css']
})
export class ProductNotificationDialogComponent implements OnInit, OnDestroy {

  productId: string;
  notification: ProductNotification = new ProductNotification('', '', false, false, 0, 0);
  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'Сохранить',
    spinnerSize: 19,
    raised: true,
    stroked: false,
    buttonColor: 'primary',
    spinnerColor: 'accent',
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
  };
  error: string;

  constructor(private http: HttpClient, private productService: ProductService, private errorHandler: ErrorHandlerService,
              @Inject(MAT_DIALOG_DATA) private data: any) { }

  ngOnInit() {
    this.productId = this.data.id;
    this.getUserProductNotification();
  }

  private getUserProductNotification() {
    this.productService.getUserProductNotification(this.productId)
      .pipe(untilDestroyed(this))
      .subscribe(p => { this.notification = p;
          // console.log(this.notification);
        },
        error => this.errorHandler.handleError(error) );
  }

  ngOnDestroy(): void {
  }

}
