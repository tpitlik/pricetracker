import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductPriceChartComponent } from './product-price-chart.component';

describe('ProductPriceChartComponent', () => {
  let component: ProductPriceChartComponent;
  let fixture: ComponentFixture<ProductPriceChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductPriceChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductPriceChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
