import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
//import { APP_ROUTING } from './app.routes';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { LoginService } from './login.service';

@NgModule({
  imports: [BrowserModule, HttpClientModule],
  declarations: [AppComponent],
  bootstrap: [AppComponent],
  providers: [LoginService]
})
export class AppModule {}
