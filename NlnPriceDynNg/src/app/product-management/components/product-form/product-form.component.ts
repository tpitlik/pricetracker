import {Component, OnDestroy, OnInit} from '@angular/core';
import { CreateUpdateProductRequest} from '../../models/createupdateproductrequest';
import { ProductService } from '../../services/product.service';
import { Router } from '@angular/router';
import {MessageService} from '../../../user-management/services/message.service';
import {MatProgressButtonOptions} from 'mat-progress-buttons';
import { untilDestroyed } from 'ngx-take-until-destroy';
import {ErrorHandlerService} from '../../../shared/services/error-handler.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit, OnDestroy {

  request = new CreateUpdateProductRequest('', '', '');
  error: string;
  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'Добавить',
    spinnerSize: 19,
    raised: true,
    stroked: false,
    buttonColor: 'primary',
    spinnerColor: 'accent',
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
  };

  constructor(private productService: ProductService, private router: Router, private messageService: MessageService, private errorHandler: ErrorHandlerService) { }

  ngOnInit() {
  }

  onSubmit() {
    this.btnOpts.active = true;
    this.productService.addProduct(this.request)
      .pipe(untilDestroyed(this))
      .subscribe(
        (data: any) => {
          this.router.navigate(['/products']);
        },
        error => {
          this.btnOpts.active = false;
          this.errorHandler.handleError(error);
          this.error = error.error['Message'];
        }
      );
  }

  ngOnDestroy(): void {
  }

}
