import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  url: string;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.url = this.route.snapshot.url.join('/');
  }

}
