import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../../shared/baseUrl';
import { CreateUpdateProductRequest} from '../models/createupdateproductrequest';
import { Observable } from 'rxjs';
import { Product } from '../models/product';
import {ProductNotification} from '../models/productnotification';
import {CreateUpdateProductNotificationRequest} from '../models/createupdateproductnotificationrequest';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  addProduct(request: CreateUpdateProductRequest) {
    let result = this.http.post(BaseUrl.hosturl + 'api/products/add', request);
    return result;
  }

  deleteProduct(productId: string) {
    let result = this.http.delete(BaseUrl.hosturl + `api/products/delete/${productId}`);
    return result;
  }

  getUserProducts(): Observable<Array<Product>> {
    let result = this.http.get<Array<Product>>(BaseUrl.hosturl + 'api/products/list');
    return result;
  }

  getProductPriceData(id: string, period: string): Observable<any> {
    let result = this.http.get(BaseUrl.hosturl + `api/products/price/${id}?period=${period}`);
    return result;
  }

  getSecondsFromLastUpdate(): Observable<any> {
    let result = this.http.get(BaseUrl.hosturl + 'api/products/update/last');
    return result;
  }

  getUserProductNotification(id: string): Observable<ProductNotification> {
    let result = this.http.get<ProductNotification> (BaseUrl.hosturl + `api/products/${id}/notification`);
    return result;
  }

  UpdateProductNotification(request: CreateUpdateProductNotificationRequest) {
    let result = this.http.post(BaseUrl.hosturl + 'api/products/notification/update', request);
    return result;
  }
}
