import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';
import {ProductPriceChartComponent} from '../product-price-chart/product-price-chart.component';

@Component({
  selector: 'app-price-chart-dialog',
  templateUrl: './price-chart-dialog.component.html',
  styleUrls: ['./price-chart-dialog.component.css']
})
export class PriceChartDialogComponent implements OnInit {

  productId: string;
  constructor(private dialogRef: MatDialogRef<ProductPriceChartComponent>, @Inject(MAT_DIALOG_DATA) private data: any) { }

  ngOnInit() {
    this.productId = this.data.id;
  }

}
