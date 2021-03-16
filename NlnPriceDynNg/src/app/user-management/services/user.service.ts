import { Injectable } from '@angular/core';
import { UserCredentials } from '../models/user-credentials';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../../shared/baseUrl';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  create(userCredentials: UserCredentials) {
    const result = this.http.post(BaseUrl.hosturl + 'api/accounts/register', userCredentials);
    return result;
  }

  confirmEmail(id: string, code: string) {
    const result = this.http.get(BaseUrl.hosturl + `api/accounts/confirmemail?id=${id}&code=${code}`);
    return result;
  }

  sendConfirmationEmail(id: string) {
    const result = this.http.get(BaseUrl.hosturl + `api/accounts/resendtoken?id=${id}`);
    return result;
  }
}
