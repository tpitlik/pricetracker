import { Component, OnInit } from '@angular/core';
import {ErrorHandlerService} from '../../../services/error-handler.service';

@Component({
  selector: 'app-unknown-error',
  templateUrl: './unknown-error.component.html',
  styleUrls: ['./unknown-error.component.css']
})
export class UnknownErrorComponent implements OnInit {
  public errorMessage: string;

  constructor(private errorHandler: ErrorHandlerService) { }

  ngOnInit() {
    this.errorMessage = this.errorHandler.error['Message'];
  }

}
