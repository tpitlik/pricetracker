import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Product} from '../../models/product';
import * as Highcharts from 'highcharts';
require('highcharts/highcharts-more')(Highcharts);
import * as HC_solidgauge from 'highcharts/modules/solid-gauge';
import {PriceChartDialogComponent} from '../price-chart-dialog/price-chart-dialog.component';
import {ProductNotificationDialogComponent} from '../product-notification-dialog/product-notification-dialog.component';
import {untilDestroyed} from 'ngx-take-until-destroy';
import {CreateUpdateProductNotificationRequest} from '../../models/createupdateproductnotificationrequest';
import {ProductDeleteDialogComponent} from '../product-delete-dialog/product-delete-dialog.component';
import {MatDialog, MatDialogRef} from '@angular/material';
import {HttpClient} from '@angular/common/http';
import {ProductService} from '../../services/product.service';
import {ErrorHandlerService} from '../../../shared/services/error-handler.service';
import {Router} from '@angular/router';
HC_solidgauge(Highcharts);

@Component({
  selector: 'app-product-list-item',
  templateUrl: './product-list-item.component.html',
  styleUrls: ['./product-list-item.component.css']
})
export class ProductListItemComponent implements OnInit, OnDestroy {

  @Input() product: Product;
  Highcharts = Highcharts;
  chartConstructor = 'chart';
  chart: any;
  chartOptions: any;
  productInStock = true;
  productTitle: string;
  updateFlag: any;

  priceChartDialogDialogRef: MatDialogRef<PriceChartDialogComponent>;
  productDeleteDialogRef: MatDialogRef<ProductDeleteDialogComponent>;
  productNotificationDialogRef: MatDialogRef<ProductNotificationDialogComponent>;


  constructor(private http: HttpClient, private productService: ProductService,
              private priceChartDialog: MatDialog, private productDeleteDialog: MatDialog, private productNotifivationDialog: MatDialog,
              private errorHandler: ErrorHandlerService, private router: Router) { }

  ngOnInit() {
    const minPrice = Math.floor(Number(this.product.MinPrice));
    let maxPrice = Math.ceil(Number(this.product.MaxPrice));
    const offersCount = Number(this.product.OffersCount);
    let meanPrice = Number(this.product.MeanPrice);
    if ((maxPrice - minPrice) <= 1 ) {
      meanPrice = Math.floor(minPrice);
      maxPrice = minPrice + 1.0;
    }
    this.product.OutOfStock === true ? this.productInStock = false : this.productInStock = true;

    this.chartOptions = {
      chart: {
        type: 'solidgauge',
        animation: false
      },
      title: null,
      pane: {
        center: ['50%', '85%'],
        size: '150%',
        startAngle: -90,
        endAngle: 90,
        background: {
          backgroundColor: '#EEE',
          innerRadius: '60%',
          outerRadius: '100%',
          shape: 'arc'
        }
      },
      tooltip: {
        enabled: false
      },
      yAxis: {
        stops: [
          [0.1, '#55BF3B'], // green
          [0.5, '#DDDF0D'], // yellow
          [0.9, '#DF5353'] // red
        ],
        lineWidth: 0,
        minorTickInterval: 0,
        tickAmount: 0,
        tickWidth: 0,
        title: {
          y: -60,
          text: 'Цена'
        },
        labels: {
          y: 18
        },
        min: minPrice,
        max: maxPrice,
        tickPositions: [minPrice, maxPrice]
      },
      plotOptions: {
        solidgauge: {
          dataLabels: {
            y: 0,
            borderWidth: 0,
            useHTML: true
          },
          animation: false
        }
      },
      credits: {
        enabled: false
      },
      series: [{
        name: 'Price',
        data: [meanPrice],
        dataLabels: {
          format: `<div style="text-align:center"><span style="font-size:14px;color: black">${this.product.MeanPrice}</span><br/>` +
                  '<span style="font-size:12px;color:silver">BYN</span></div>',
        },
        tooltip: {
          valueSuffix: ' BYN'
        }
      }]
    };
  }

  openPriceChartDialog(productId: string) {
    this.priceChartDialogDialogRef = this.priceChartDialog.open(PriceChartDialogComponent, {
      data: {
        id: productId
      },
      width: '75%',
      maxHeight: '90vh'
    });
  }

  openProductNotificationDialog(productId: string) {
    this.productNotificationDialogRef = this.productDeleteDialog.open(ProductNotificationDialogComponent, {
      disableClose: true,
      data: {
        id: productId
      }
    });
    this.productNotificationDialogRef.afterClosed()
      .pipe(untilDestroyed(this))
      .subscribe(data => {
        if (data) {
          // console.log(data);
          let request = new CreateUpdateProductNotificationRequest(data.Id, data.IsActive, data.TriggerOnce, data.LowPriceLimit);
          this.updateUserProductNotification(request);
        } else {
        }
      });
  }

  updateUserProductNotification(request: CreateUpdateProductNotificationRequest){
    this.productService.UpdateProductNotification(request)
      .pipe(untilDestroyed(this))
      .subscribe(
        result => {
          this.product.ProductNotificationActive = request.IsActive;
        },
        error => this.errorHandler.handleError(error)
      );
  }
  openProductDeleteDialog(productId: string, productTitle: string) {
    this.productDeleteDialogRef = this.productDeleteDialog.open(ProductDeleteDialogComponent,
      {
        disableClose: true,
        data: {
          productTitle: productTitle,
        }
      });
    this.productDeleteDialogRef.afterClosed()
      .pipe(untilDestroyed(this))
      .subscribe(result => {
          if (result) {
            this.productService.deleteProduct(productId)
              .pipe(untilDestroyed(this))
              .subscribe(result => {
                  this.getUserProducts();
                },
                error => this.errorHandler.handleError(error) );
          }
        },
        error => this.errorHandler.handleError(error) );
  }

  ngOnDestroy(): void {
  }

  getUserProducts() {
    this.router.navigate(['/products']);
  }
}


