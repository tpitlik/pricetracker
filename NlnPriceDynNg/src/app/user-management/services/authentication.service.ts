import {Injectable, OnInit} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {BaseUrl} from '../../shared/baseUrl';
import {map} from 'rxjs/operators';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private signinStatus: BehaviorSubject<boolean>;

  constructor(private http: HttpClient) {
    if (localStorage.getItem('access_token')) {
      this.signinStatus = new BehaviorSubject<boolean>(true);
    } else {
      this.signinStatus = new BehaviorSubject<boolean>(false);
    }
  }

  login(username: string, password: string) {
    return this.http.post<any>(BaseUrl.hosturl + 'api/auth/login', JSON.stringify({ username, password }), {
      headers: new HttpHeaders()
        .set('Content-Type', 'application/json')
    }).pipe(map(auth_result => {
      if (auth_result && auth_result.auth_token) {
        localStorage.setItem('access_token', auth_result.auth_token);
      }
      this.signinStatus.next(true);
      return auth_result;
    }));
  }

  logout() {
    localStorage.removeItem('access_token');
    this.signinStatus.next(false);
  }

  public isSignedIn(): Observable<boolean> {
    return this.signinStatus.asObservable();
  }

}
