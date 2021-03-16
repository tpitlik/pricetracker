import {Component, OnDestroy, OnInit} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { interval } from 'rxjs';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { ErrorHandlerService } from '../../../shared/services/error-handler.service';
import {NavigationEnd, Router} from '@angular/router';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit, OnDestroy {

  products: Product[];
  updateInterval = 60000;
  initialTimestamp: number;
  isLoading: boolean;
  navigationSubscription;

  constructor(private http: HttpClient, private productService: ProductService, private errorHandler: ErrorHandlerService, private router: Router) {
  }

  ngOnInit() {
    this.isLoading = true;
    this.getUserProducts();
    interval(this.updateInterval)
      .pipe(untilDestroyed(this))
      .subscribe(res => this.productService.getSecondsFromLastUpdate().subscribe(
        timeRes => {  console.log(timeRes);
                      const currentTimestamp = Number(timeRes['timestamp']);
                      if (currentTimestamp !== this.initialTimestamp) {
                        this.getUserProducts();
                        this.initialTimestamp = currentTimestamp;
                      }
                    }),
        error => this.errorHandler.handleError(error) );

    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.getUserProducts();
      }
    });
  }

  getUserProducts() {
    this.productService.getSecondsFromLastUpdate()
      .pipe(untilDestroyed(this))
      .subscribe( res => this.initialTimestamp = Number(res['timestamp']),
                  error => this.errorHandler.handleError(error) );

    this.productService.getUserProducts()
      .pipe(untilDestroyed(this))
      .subscribe(p => { this.products = p;
                        console.log(this.products);
                        this.isLoading = false;
                      },
        error => this.errorHandler.handleError(error) );
  }

  ngOnDestroy(): void {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

}
