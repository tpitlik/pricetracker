import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
require('highcharts/highcharts-more')(Highcharts);
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { ErrorHandlerService } from '../../../shared/services/error-handler.service';
import { MatSelectChange } from '@angular/material';

@Component({
  selector: 'app-product-price-chart',
  templateUrl: './product-price-chart.component.html',
  styleUrls: ['./product-price-chart.component.css']
})
export class ProductPriceChartComponent implements OnInit, OnDestroy {

  priceRangeData: Array<[number, number, number]>;
  priceAverageData: Array<[number, number]>;
  @Input() productId: string;
  chart: any;
  isLoading: boolean;

  constructor(private http: HttpClient, private productService: ProductService, private errorHandler: ErrorHandlerService) { }

  Highcharts = Highcharts;
  chartConstructor = 'chart';

  chartOptions = {
    chart: {
      zoomType: 'x',
      spacingBottom: 0,
      spacingTop: 5,
      spacingLeft: 5,
      spacingRight: 5
    },
    title: {
      text: '',
      style: {
        color: '#333333',
        fontSize: '16px'
      },
      floating: false
    },
    credits: {
      enabled: false
    },
    xAxis: {
      type: 'datetime'
    },
    yAxis: {
      title: {
        text: 'BYN',
        style: {
          color: '#333333',
          fontSize: '14px'
        },
      }
    },
    tooltip: {
      crosshairs: true,
      shared: true,
    },
    series: [{
      name: 'Средняя',
      data: this.priceRangeData,
      zIndex: 1,
      lineWidth: 3,
      marker: {
        fillColor: 'white',
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[0],
        enabled: false
      }
    }, {
      name: 'Мин.-Макс.',
      data: this.priceAverageData,
      type: 'arearange',
      lineWidth: 0,
      linkedTo: ':previous',
      color: Highcharts.getOptions().colors[0],
      fillOpacity: 0.2,
      zIndex: 0,
      marker: {
        enabled: false
      }
    }]
  };
  updateFlag: boolean;
  periodSelected: string;


  ngOnInit() {
    this.isLoading = true;
    this.periodSelected = 'week';
    const timeZoneOffset = new Date().getTimezoneOffset();

    this.Highcharts.setOptions({
      time: {
        timezoneOffset: timeZoneOffset
      },
      lang: {
        months: [
          'Январь', 'Февраль', 'Март', 'Апрель',
          'Май', 'Июнь', 'Июль', 'Август',
          'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
        ],
        shortMonths: [
          'Янв', 'Фев', 'Мар', 'Апр',
          'Май', 'Июн', 'Июл', 'Авг',
          'Сен', 'Окт', 'Ноя', 'Дек'
        ],
        weekdays: [
          'Воскресенье', 'Понедельник', 'Вторник', 'Среда',
          'Четверг', 'Пятница', 'Суббота'
        ]
      }
    });
    this.getProductPriceData();
  }

  getProductPriceData() {
    this.productService.getProductPriceData(this.productId, this.periodSelected)
      .pipe(untilDestroyed(this))
      .subscribe(p => {
      this.priceAverageData = this.processPriceAverageArray(p['priceAverage']);
      this.priceRangeData = this.processPriceRangeArray(p['priceRange']);
      this.chartOptions.series[0].data = this.priceAverageData;
      this.chartOptions.series[1].data = this.priceRangeData;
      this.updateFlag = true;
      this.isLoading = false;
    },
        error => this.errorHandler.handleError(error));
  }

  private processPriceAverageArray(data: Array<[string, string]>): Array<[number, number]> {
    return data.map(([t, v]): [number, number] => {
      return [Number(t), Number(v)];
    });
  }

  private processPriceRangeArray(data: Array<[string, string, string]>): Array<[number, number, number]> {
    return data.map(([t, v1, v2]): [number, number, number] => {
      return [Number(t), Number(v1), Number(v2)];
    });
  }

  ngOnDestroy(): void {
  }

  updateChart($event: MatSelectChange) {
    this.isLoading = true;
    this.getProductPriceData();
  }
}

