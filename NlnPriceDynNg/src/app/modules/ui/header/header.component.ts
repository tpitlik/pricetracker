import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../../../user-management/services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  signedIn: Observable<boolean>;

  constructor(private router: Router, private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.signedIn = this.authenticationService.isSignedIn();
  }

  signout(): void {
    this.authenticationService.logout();
    this.router.navigate(['/main']);
  }

}
