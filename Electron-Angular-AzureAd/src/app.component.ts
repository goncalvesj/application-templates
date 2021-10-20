import { Component, OnInit } from '@angular/core';
import { LoginService } from './login.service';

@Component({
  selector: 'App',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private readonly loginService: LoginService) {}

  ngOnInit(): void {}

  login() {
    this.loginService.logIn();
  }
}
